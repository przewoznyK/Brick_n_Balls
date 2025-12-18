using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct BrickSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (brickComponent, colorOverrideComponent, brickHealthChangedComponent, brickEntity) in
                    SystemAPI.Query<
                        RefRW<BrickComponent>,
                        RefRW<ColorOverrideComponent>,
                        RefRO<BrickHealthChangedComponent>
                    >()
                    .WithEntityAccess())
        {
            brickComponent.ValueRW.Health += brickHealthChangedComponent.ValueRO.Value;

            Entity soundEventEntity = ecb.CreateEntity();
            ecb.AddComponent(soundEventEntity, new BrickHitSoundTag());

            if (brickComponent.ValueRO.Health > 0)
            {
                colorOverrideComponent.ValueRW.Value = BrickLogic.GetColorForHealth(brickComponent.ValueRO.Health);
                ecb.RemoveComponent<BrickHealthChangedComponent>(brickEntity);
            }
            else
                ecb.DestroyEntity(brickEntity);
        }
    }
}
