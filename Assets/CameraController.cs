using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, GameInputSystem.IMouseActions
{
    [SerializeField] private Transform cameraParent;
    [SerializeField] private Camera screenCamera;

    private GameInputSystem gameInputSystem;

    private Astar astar;
    private void Awake()
    {
        cameraParent = transform.parent;

        astar = GetComponent<Astar>();
        screenCamera = cameraParent.GetChild(0).GetComponent<Camera>();

        gameInputSystem = new GameInputSystem();
        gameInputSystem.Mouse.SetCallbacks(this);
    }

    private void OnEnable()
    {
        gameInputSystem.Mouse.Enable();
    }

    private void OnDisable()
    {
        gameInputSystem.Mouse.Disable();
    }

    private bool isTouched = false;

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            context.action.ChangeBinding(1);
            Debug.Log(screenCamera.ScreenToWorldPoint(context.ReadValue<Vector2>()));
        }

        if (context.performed && isTouched)
        {
            context.action.ChangeBinding(0);
            Vector3 contextValue = new Vector3(-context.ReadValue<Vector2>().x, 0f, -context.ReadValue<Vector2>().y) * 0.1f;
            cameraParent.Translate(contextValue);
            Debug.Log(contextValue);
        }
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isTouched = true;
        }

        if (context.canceled)
        {
            isTouched = false;
        }
    }
}
