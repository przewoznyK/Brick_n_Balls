using Unity.Entities;
using UnityEngine;

public class SpawnBricksConfigAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;

    private class Baker : Baker<SpawnBricksConfigAuthoring>
    {
        public override void Bake(SpawnBricksConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnBricksConfigComponent
            {
                BrickEntity = GetEntity(authoring.brickPrefab, TransformUsageFlags.Renderable)
            });
            AddComponent(entity, new RequestSpawnBricksComponent());
        }
    }
}

public struct SpawnBricksConfigComponent : IComponentData
{
    public Entity BrickEntity;
}