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

    [SerializeField] private Astar astar;

    public float rotateButtonLeftPos;
    public float rotateButtonRightPos;
    public float rotateButtonBottomPos;
    public float rotateButtonUpperPos;

    private void Awake()
    {
        cameraParent = transform.parent;

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

    public bool isTouched = false;
    public bool cameraMove = false;

    private Vector3 positionValue;

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        if (context.performed && isTouched && !buildingManager.buildingWindow.gameObject.activeSelf)
        {
            cameraMove = true;

            Vector3 deltaValue = new Vector3(-context.ReadValue<Vector2>().x, 0f, -context.ReadValue<Vector2>().y) * 0.01f;

            Vector3 newCameraParentPos = deltaValue + new Vector3(cameraParent.position.x, 0f, cameraParent.position.z);

            if (astar.bottomLeftPos.x <= newCameraParentPos.x
                && astar.bottomLeftPos.z <= newCameraParentPos.z
                && astar.upperRightPos.x >= newCameraParentPos.x
                && astar.upperRightPos.z >= newCameraParentPos.z)
            {
                cameraParent.Translate(deltaValue);
            }
        }
    }

    [SerializeField] private LayerMask chooseLayerMask;
    public Text buildingNodePos;
    public Text buildingName;

    [SerializeField] private string beforeHitPos;

    public void OnTouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isTouched = true;
        }

        if (context.canceled)
        {
            if (!cameraMove)
            {
                Ray ray = screenCamera.ScreenPointToRay(positionValue);
                Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, chooseLayerMask);

                if (hit.transform != null)
                {
                    cameraParent.position = new Vector3(hit.point.x, cameraParent.position.y, hit.point.z);

                    Node node = astar.GetNodeByPosition(hit.transform.position);

                    if (hit.transform.gameObject.layer == (int)GameLayer.building
                        || hit.transform.gameObject.layer == (int)GameLayer.road)
                    {
                        //건물 정보 가져오기
                        buildingNodePos.text = $"({node.xPosition}, {node.yPosition})";
                        buildingName.text = $"{node.buildingName}";
                    }
                    else
                    {
                        buildingName.text = "Tile";
                        beforeHitPos = $"({node.xPosition}, {node.yPosition})";
                    }
                    
                    if (beforeHitPos == buildingNodePos.text)
                    {
                        buildingManager.build = true;
                        buildingManager.demolition = true;
                    }

                }
                else
                {
                    beforeHitPos = null;
                }
            }

            isTouched = false;

            if (cameraMove)
            {
                cameraMove = false;    
            }
        }
    }

    public void OnChooseBuilding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            positionValue = context.ReadValue<Vector2>();
        }
    }
}
