using Unity.Entities;
using UnityEngine;

public partial class ScoreAuthoring : MonoBehaviour
{
    private class Baker : Baker<ScoreAuthoring>
    {
        public override void Bake(ScoreAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ScoreComponent { Score = 0 });
        }
    }
}

public struct ScoreComponent : IComponentData
{
    public int Score;
}

public struct ScoreIncrementComponent : IComponentData
{
    public int Value;
}