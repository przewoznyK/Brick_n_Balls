using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Burst;

[BurstCompile]
public partial struct ShooterSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShooterComponent>();
        state.RequireForUpdate<InputComponent>();
        state.RequireForUpdate<GameStartedTag>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputComponent = SystemAPI.GetSingleton<InputComponent>();
        var shooterEntity = SystemAPI.GetSingletonEntity<ShooterComponent>();

        var shooterTransform = SystemAPI.GetComponentRW<LocalTransform>(shooterEntity);
        var shooterComponent = SystemAPI.GetComponentRW<ShooterComponent>(shooterEntity);
        var shooterVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(shooterEntity);

        ApplyRotation(shooterTransform, inputComponent.MouseWorldPosition);
        ApplyMovement(shooterVelocity, shooterComponent, inputComponent.HorizontalMovement);
        ApplyShoot(ref state, inputComponent, shooterComponent, shooterTransform);
    }

    private void ApplyRotation(RefRW<LocalTransform> transform, float3 mouseWorldPos)
    {
        float3 direction = mouseWorldPos - transform.ValueRO.Position;
        float angleRadians = math.atan2(-direction.x, direction.y);
        float clampLimit = math.radians(85f);
        transform.ValueRW.Rotation = quaternion.Euler(0, 0, math.clamp(angleRadians, -clampLimit, clampLimit));
    }

    private void ApplyMovement(RefRW<PhysicsVelocity> velocity, RefRW<ShooterComponent> shooterComponent, float inputDirection)
    {
        if (shooterComponent.ValueRO.CanShoot)
            velocity.ValueRW.Linear = new float3(inputDirection * shooterComponent.ValueRO.MovementSpeed, 0, 0);
        else
            velocity.ValueRW.Linear = float3.zero;
    }

    private void ApplyShoot(
        ref SystemState state,
        InputComponent inputComponent,
        RefRW<ShooterComponent> shooterComponent,
        RefRW<LocalTransform> shooterTransform)
    {
        if (inputComponent.PressingLMB == false) return;
        if (shooterComponent.ValueRO.CanShoot == false || shooterComponent.ValueRO.BallCount <= 0) return;

        shooterComponent.ValueRW.CanShoot = false;
        shooterComponent.ValueRW.BallCount -= 1;
        shooterTransform.ValueRW.Scale = 0.001f;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        Entity eventEntity = ecb.CreateEntity();

        ecb.AddComponent(eventEntity, new SpawnBallEventComponent
        {
            Position = shooterTransform.ValueRO.Position,
            Rotation = shooterTransform.ValueRO.Rotation
        });
    }
}

