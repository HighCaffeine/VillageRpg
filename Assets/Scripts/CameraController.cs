using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, GameInputSystem.IMouseActions
{
    [SerializeField] private BuildingManager buildingManager;

    public Transform cameraParent;
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
    public bool cameraMove = false;

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        if (context.performed && isTouched)
        {
            cameraMove = true;

            Vector3 contextValue = new Vector3(-context.ReadValue<Vector2>().x, 0f, -context.ReadValue<Vector2>().y) * 0.01f;
            
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

            if (!cameraMove)
            {
                buildingManager.build = true;
            }
            else
            {
                cameraMove = false;
            }
        }
    }

    //[SerializeField] private Vector3 choosedBuildingPosition;
    [SerializeField] private LayerMask chooseLayerMask;

    public void OnChooseBuilding(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 contextValue = context.ReadValue<Vector2>();
            Ray ray = screenCamera.ScreenPointToRay(contextValue);

            Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, chooseLayerMask);
            
            cameraParent.position = new Vector3(hit.point.x, cameraParent.position.y, hit.point.z);

            if (hit.transform.gameObject.layer == 7) // 7 -> Building
            {
                //건물 정보 가져오기
            }
        }
    }

}
