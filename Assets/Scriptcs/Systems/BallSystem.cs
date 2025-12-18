using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct BallSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        float deltaTime = SystemAPI.Time.DeltaTime;
        var brickLookup = SystemAPI.GetComponentLookup<BrickComponent>(true);
        var wallDownLookup = SystemAPI.GetComponentLookup<WallDownTag>(true);
        
        brickLookup.Update(ref state);
        wallDownLookup.Update(ref state);

        NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

        foreach (var (ballVelocity, ballComponent, ballTransform, ballEntity) in
                 SystemAPI.Query<
                     RefRW<PhysicsVelocity>,
                     RefRW<BallComponent>,
                     RefRW<LocalTransform>>()
                 .WithNone<Prefab>()
                 .WithEntityAccess())
        {
            if (ballComponent.ValueRO.IgnoreCollisionsTimer > 0)
            {
                ballComponent.ValueRW.IgnoreCollisionsTimer -= deltaTime;
                continue;
            }

            float ballRadius = ballComponent.ValueRO.Size / 2f;

            float3 ballLinearVelocity = ballVelocity.ValueRO.Linear;
            float ballSpeed = math.length(ballLinearVelocity);
            float ballDistance = ballSpeed * deltaTime + 0.1f;
            float3 ballDirection = ballSpeed < 0.001f ? float3.zero : math.normalize(ballLinearVelocity);

            if (ballSpeed < 0.001f) ballDistance = 0.05f;

            physicsWorld.SphereCastAll(
                ballTransform.ValueRO.Position,
                ballRadius,
                ballDirection,
                ballDistance,
                ref hits,
                new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Ball,
                    CollidesWith = (uint)CollisionLayer.Brick | (uint)CollisionLayer.Wall | (uint)CollisionLayer.WallDown
                });

            if (hits.Length == 0) continue;

            ColliderCastHit closestHit = default;
            float minFraction = float.MaxValue;
            bool foundValidHit = false;

            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.Entity == Entity.Null) continue;
                if (hit.Entity == ballEntity) continue;

                if (ballComponent.ValueRO.LastCollisionEntity == hit.Entity) continue;

                if (hit.Fraction < minFraction)
                {
                    minFraction = hit.Fraction;
                    closestHit = hit;
                    foundValidHit = true;
                }
            }

            if (foundValidHit == false) continue;

            Entity targetEntity = closestHit.Entity;

            ballComponent.ValueRW.IgnoreCollisionsTimer = ballComponent.ValueRO.CollisionIgnoreTime;
            ballComponent.ValueRW.LastCollisionEntity = targetEntity;

            if (brickLookup.HasComponent(targetEntity))
                HandleBrickCollision(ecb, targetEntity);
            else if (wallDownLookup.HasComponent(targetEntity))
            {
                HandleWallDownCollision(ref state, ecb, ballEntity);
                continue;
            }

            ReflectVelocity(ref ballVelocity.ValueRW, ref ballTransform.ValueRW, ballRadius, closestHit);
        }

        hits.Dispose();
    }

    private void HandleBrickCollision(EntityCommandBuffer ecb, Entity hitEntity)
    {
        var scoreEntity = SystemAPI.GetSingletonEntity<ScoreComponent>();
        ecb.AddComponent(hitEntity, new BrickHealthChangedComponent { Value = -1 });
        ecb.AddComponent(scoreEntity, new ScoreIncrementComponent { Value = 1 });
    }

    private void HandleWallDownCollision(ref SystemState systemState, EntityCommandBuffer ecb, Entity ballEntity)
    {
        ecb.DestroyEntity(ballEntity);

        var shooterEntity = SystemAPI.GetSingletonEntity<ShooterComponent>();
        var shooterComponent = SystemAPI.GetComponentRW<ShooterComponent>(shooterEntity);
        shooterComponent.ValueRW.CanShoot = true;

        if (shooterComponent.ValueRO.BallCount <= 0)
            ecb.AddComponent(shooterEntity, new GameOverTag());
        else
        {
            var shooterTransform = SystemAPI.GetComponentRW<LocalTransform>(shooterEntity);
            shooterTransform.ValueRW.Scale = 1;
        }
    }
    private void ReflectVelocity(ref PhysicsVelocity ballVelocity, ref LocalTransform ballTransform, float ballRadius, ColliderCastHit closestHit)
    {
        if (math.lengthsq(closestHit.SurfaceNormal) < 0.001f) return;

        float3 fixedNormal = closestHit.SurfaceNormal;
        if (math.abs(fixedNormal.x) > math.abs(fixedNormal.y))
            fixedNormal = new float3(math.sign(fixedNormal.x), 0, 0);
        else
            fixedNormal = new float3(0, math.sign(fixedNormal.y), 0);

        ballTransform.Position = closestHit.Position + (fixedNormal * (ballRadius * 1.01f));

        float3 reflected = math.reflect(ballVelocity.Linear, fixedNormal);
        float speed = math.length(ballVelocity.Linear);
        ballVelocity.Linear = math.normalize(reflected) * speed;
        ballVelocity.Linear.z = 0;
    }
}

public struct GameOverTag : IComponentData { }

public enum CollisionLayer
{
    Ball = 1 << 3,
    Brick = 1 << 6,
    Wall = 1 << 7,
    WallDown = 1 << 8
}