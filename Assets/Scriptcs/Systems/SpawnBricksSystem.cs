using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnBricksSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnBricksConfigComponent>();
        state.RequireForUpdate<RequestSpawnBricksComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<SpawnBricksConfigComponent>();

        const int Width = 13;
        const int Height = 10;
        const float BrickWidth = 1.2f;
        const float BrickHeight = 0.7f;

        float totalWidth = Width * BrickWidth;
        float totalHeight = Height * BrickHeight;

        float startX = 0f - (totalWidth / 2f);
        float startY = 6f - (totalHeight / 2f);
        int centerColumn = Width / 2;

        Span<int> diagonalOffsets = stackalloc int[] { 0, 1, 2, 1, 0, 1, 2, 1, 0, 1 };

        var positions = new NativeList<float3>(Width * Height, Allocator.Temp);
        var healths = new NativeList<int>(Width * Height, Allocator.Temp);

        var random = Unity.Mathematics.Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime + 1);

        for (int x = 0; x < Width; x++)
        {
            if (x == centerColumn) continue;

            for (int y = 0; y < Height; y++)
            {
                if ((y + diagonalOffsets[y]) % 5 == x % 5) continue;

                float posX = startX + (x * BrickWidth) + (BrickWidth / 2f);
                float posY = startY + (y * BrickHeight) + (BrickHeight / 2f);

                positions.Add(new float3(posX, posY, 0));
                healths.Add(random.NextInt(1, 4));
            }
        }

        if (positions.Length > 0)
        {
            var entities = new NativeArray<Entity>(positions.Length, Allocator.Temp);
            state.EntityManager.Instantiate(config.BrickEntity, entities);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                int hp = healths[i];
                float3 pos = positions[i];

                state.EntityManager.SetComponentData(e, new LocalTransform
                {
                    Position = pos,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                state.EntityManager.SetComponentData(e, new BrickComponent { Health = hp });
                state.EntityManager.SetComponentData(e, new ColorOverrideComponent { Value = BrickLogic.GetColorForHealth(hp) });
            }

            entities.Dispose();
        }

        positions.Dispose();
        healths.Dispose();

        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<RequestSpawnBricksComponent>());
    }
}
public struct RequestSpawnBricksComponent : IComponentData { }