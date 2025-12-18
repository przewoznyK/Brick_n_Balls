using Unity.Entities;
using UnityEngine;

public class ShooterAuthoring : MonoBehaviour
{
    [SerializeField] private int ballCount;
    [SerializeField] private float movementSpeed;
    private class Baker : Baker<ShooterAuthoring>
    {
        public override void Bake(ShooterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShooterComponent
            { 
                CanShoot = true, 
                BallCount = authoring.ballCount,
                MovementSpeed = authoring.movementSpeed
            });
        }
    }
}

public struct ShooterComponent : IComponentData
{
    public bool CanShoot;
    public int BallCount;
    public float MovementSpeed;
}

public struct GameStartedTag : IComponentData { }