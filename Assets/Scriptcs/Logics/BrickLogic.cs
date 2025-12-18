using Unity.Mathematics;

public static class BrickLogic
{
    public static float4 GetColorForHealth(int health)
    {
        return health switch
        {
            3 => new float4(1f, 0f, 0f, 1f),
            2 => new float4(0f, 0f, 1f, 1f),
            1 => new float4(0f, 1f, 0f, 1f),
            _ => new float4(1f, 1f, 1f, 1f)
        };
    }
}