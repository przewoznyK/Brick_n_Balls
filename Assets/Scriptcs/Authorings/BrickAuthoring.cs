using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class BrickAuthoring : MonoBehaviour
{
    [SerializeField] private int health = 3;

    private class Baker : Baker<BrickAuthoring>
    {
        public override void Bake(BrickAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new BrickComponent { Health = authoring.health });
            AddComponent(entity, new ColorOverrideComponent { Value = BrickLogic.GetColorForHealth(authoring.health) });
        }
    }
}

public struct BrickComponent : IComponentData
{
    public int Health;
}

public struct BrickHealthChangedComponent : IComponentData 
{
    public int Value;
}

[MaterialProperty("_Color")]
public struct ColorOverrideComponent : IComponentData
{
    public float4 Value;
}