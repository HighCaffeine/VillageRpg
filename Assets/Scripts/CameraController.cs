using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, GameInputSystem.IMouseActions
{
    //test
    public Button rotateButton;
    public RectTransform rotateRect;

    [SerializeField] private BuildingManager buildingManager;

    public Transform cameraParent;
    [SerializeField] private Camera screenCamera;

    private GameInputSystem gameInputSystem;

    private Astar astar;

    public float rotateButtonLeftPos;
    public float rotateButtonRightPos;
    public float rotateButtonBottomPos;
    public float rotateButtonUpperPos;

    private void Awake()
    {
        cameraParent = transform.parent;

        astar = GetComponent<Astar>();
        screenCamera = cameraParent.GetChild(0).GetComponent<Camera>();

        gameInputSystem = new GameInputSystem();
        gameInputSystem.Mouse.SetCallbacks(this);

        rotateButtonLeftPos = rotateRect.transform.position.x - rotateRect.rect.width * 0.5f;
        rotateButtonRightPos = rotateRect.transform.position.x + rotateRect.rect.width * 0.5f;
        rotateButtonBottomPos = rotateRect.transform.position.y - rotateRect.rect.height * 0.5f;
        rotateButtonUpperPos = rotateRect.transform.position.y + rotateRect.rect.height * 0.5f;
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
            Vector2 checkButton = context.ReadValue<Vector2>();

            isTouched = false;

            if (!cameraMove)
            {
                if (checkButton.x < rotateButtonLeftPos && checkButton.x > rotateButtonRightPos
                    && checkButton.y < rotateButtonBottomPos && checkButton.y > rotateButtonUpperPos)
                {
                    buildingManager.build = true;
                }
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
            
            if (hit.transform != null)
            {
                cameraParent.position = new Vector3(hit.point.x, cameraParent.position.y, hit.point.z);

                if (hit.transform.gameObject.layer == 7) // 7 -> Building
                {
                    //건물 정보 가져오기
                }
            }
        }
    }
}
