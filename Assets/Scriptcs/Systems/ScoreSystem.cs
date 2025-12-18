using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct ScoreSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ScoreComponent>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (scoreComponent, scoreIncrementComponent, entity) in
                 SystemAPI.Query<
                     RefRW<ScoreComponent>,
                     RefRO<ScoreIncrementComponent>
                 >()
                 .WithEntityAccess())
        {
            scoreComponent.ValueRW.Score += scoreIncrementComponent.ValueRO.Value;
            ecb.RemoveComponent<ScoreIncrementComponent>(entity);
        }
    }
}