using Unity.Entities;
using UnityEngine;

public class WallDownAuthoring : MonoBehaviour
{
    private class Baker : Baker<WallDownAuthoring>
    {
        public override void Bake(WallDownAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new WallDownTag { });
        }
    }
}

public struct WallDownTag : IComponentData { }