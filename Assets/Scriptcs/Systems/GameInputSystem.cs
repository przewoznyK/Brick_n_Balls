using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public partial class GameInputSystem : SystemBase
{
    private InputSystem_Actions _inputActions;
    private Camera _mainCamera;

    protected override void OnCreate()
    {
        if(SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input) == false)
            EntityManager.CreateEntity(typeof(InputComponent));

        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
    }

    protected override void OnStartRunning()
    {
        _mainCamera = Camera.main;
    }

    protected override void OnUpdate()
    {
        Vector2 mouseScreen = _inputActions.Player.MousePosition.ReadValue<Vector2>();
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, -_mainCamera.transform.position.z));

        bool isPressingLMB = _inputActions.Player.Attack.ReadValue<float>() == 1 ? true : false;
        Vector2 moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new InputComponent
        {
            MouseWorldPosition = worldPos,
            PressingLMB = isPressingLMB,
            HorizontalMovement = moveInput.x
        });
    }

    protected override void OnStopRunning()
    {
        _inputActions.Disable();
    }
}

public struct InputComponent : IComponentData
{
    public float3 MouseWorldPosition;
    public bool PressingLMB;
    public float HorizontalMovement;
}