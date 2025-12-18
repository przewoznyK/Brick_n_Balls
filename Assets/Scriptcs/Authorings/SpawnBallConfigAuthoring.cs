using Unity.Entities;
using UnityEngine;
public class SpawnBallConfigAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    private class Baker : Baker<SpawnBallConfigAuthoring>
    {
        public override void Bake(SpawnBallConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnBallConfigComponent
            {
                BallEntity = GetEntity(authoring.ballPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct SpawnBallConfigComponent : IComponentData
{
    public Entity BallEntity;
}