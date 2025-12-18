using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[BurstCompile]
public partial struct SpawnBallSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnBallEventComponent>();
        state.RequireForUpdate<SpawnBallConfigComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spawnBallConfigComponent = SystemAPI.GetSingleton<SpawnBallConfigComponent>();
        var ballComponent = SystemAPI.GetComponent<BallComponent>(spawnBallConfigComponent.BallEntity);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (spawnEvent, eventEntity) in SystemAPI.Query<RefRO<SpawnBallEventComponent>>().WithEntityAccess())
        {
            var newBall = ecb.Instantiate(spawnBallConfigComponent.BallEntity);
            float finalSize = ballComponent.Size > 0.01f ? ballComponent.Size : 0.5f;

            ecb.SetComponent(newBall, LocalTransform.FromPositionRotationScale(
                spawnEvent.ValueRO.Position,
                spawnEvent.ValueRO.Rotation,
                finalSize
            ));

            float3 shootDirection = math.mul(spawnEvent.ValueRO.Rotation, math.up());

            ecb.SetComponent(newBall, new PhysicsVelocity
            {
                Linear = shootDirection * ballComponent.MovementSpeed,
                Angular = float3.zero
            });

            ecb.DestroyEntity(eventEntity);
        }
    }
}

public struct SpawnBallEventComponent : IComponentData
{
    public float3 Position;
    public quaternion Rotation;
}