using UnityEngine;
using Unity.Entities;

public class BallAuthoring : MonoBehaviour
{
    [SerializeField] float size = 1;
    [SerializeField] float movementSpeed = 10;
    [SerializeField] float collisionIgnoreTime = 0.04f;
    private class Baker : Baker<BallAuthoring>
    {
        public override void Bake(BallAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BallComponent
            {
                Size = authoring.size,
                MovementSpeed = authoring.movementSpeed,
                CollisionIgnoreTime = authoring.collisionIgnoreTime
            });
        }
    }
}

public struct BallComponent : IComponentData
{
    public Entity LastCollisionEntity;
    public float Size;
    public float MovementSpeed;
    public float IgnoreCollisionsTimer;
    public float CollisionIgnoreTime;
}

public struct BrickHitSoundTag : IComponentData { }