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
        if (context.performed && isTouched)
        {
            Vector3 contextValue = new Vector3(-context.ReadValue<Vector2>().x, 0f, -context.ReadValue<Vector2>().y) * 0.1f;
            
            cameraParent.Translate(contextValue);
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

    [SerializeField] private Vector3 choosedBuildingPosition;
    public LayerMask chooseLayerMask;

    public Transform hitTransform;

    public void OnChooseBuilding(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 contextValue = context.ReadValue<Vector2>();

            Debug.Log(context.ReadValue<Vector2>());

            Vector3 cameraPosition = new Vector3(contextValue.x, contextValue.y, 0f);
            Vector3 forward = screenCamera.transform.TransformDirection(Vector3.forward);

            Physics.Raycast(screenCamera.transform.position, forward, out RaycastHit hit, 100f, chooseLayerMask);
            Debug.Log(hit.transform.name);
        }
    }

    IEnumerator Choose()
    {
        //위에 choosebuilding에서 ray쏘기 전에 누른곳으로 카메라를 이동시킴

        yield return null;
    }
}
