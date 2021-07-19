using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, GameInputSystem.IMouseActions
{
    public Button rotateButton;
    public RectTransform rotateRect;

    [SerializeField] private BuildingManager buildingManager;

    public Transform cameraParent;
    [SerializeField] private Camera screenCamera;

    private GameInputSystem gameInputSystem;

    [SerializeField] private Astar astar;

    public RectTransform clickableArea;

    [SerializeField] private float isClickableBottomValue;
    [SerializeField] private float isClickableUpperValue;

    public delegate void AddToDungeonQueue(Transform dungeon);
    public AddToDungeonQueue addToDungeonQueue;

    [SerializeField] private Transform dungeonEnterImageTransform;

    private void Awake()
    {
        isClickableBottomValue = clickableArea.anchoredPosition.y;
        isClickableUpperValue = clickableArea.anchoredPosition.y + clickableArea.rect.height;

        cameraParent = transform.parent;

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

    public bool isTouched = false;
    public bool cameraMove = false;

    private Vector3 positionValue;

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        //메인 필드 카메라
        if (isMainCamera && context.performed && isTouched && !buildingManager.buildingWindow.gameObject.activeSelf)
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
        else // 던전 카메라
        {
            //델리게이트로 가져와야할듯 -> 지금 몇번째 켜져있는지 알아야됨
            //던전마다 카메라 움직이는거 범위 제한해야됨
            //leftx = enemySpawnPoint.x - 2
            //leftZ = enemySpawnPoint.y - 2
            //rightx = enemySpawnPoint.x + 2
            //rightY = enemySpawnPoint.y + 2

        }
    }

    [SerializeField] private LayerMask chooseLayerMask;
    public Text buildingNodePos;
    public Text buildingName;

    [SerializeField] private string beforeHitPos;

    public bool isMainCamera;

    public void OnTouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isTouched = true;
        }

        if (context.canceled && isMainCamera)
        {
            if (positionValue.y > isClickableBottomValue && positionValue.y < isClickableUpperValue)
            {
                if (!cameraMove)
                {
                    Node node;
                    Ray ray = screenCamera.ScreenPointToRay(positionValue);
                    Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, chooseLayerMask);

                    if (hit.transform != null)
                    {
                        node = astar.GetNodeByPosition(hit.transform.position);


                        if (hit.transform.gameObject.layer == (int)GameLayer.Building
                            || hit.transform.gameObject.layer == (int)GameLayer.Road)
                        {
                            //건물 정보 가져오기
                            buildingNodePos.text = $"({node.xPosition}, {node.yPosition})";
                            buildingName.text = $"{node.buildingName}";
                        
                            if (beforeHitPos == buildingNodePos.text && buildingManager.nowBuilding)
                            {
                                buildingManager.build = true;
                                buildingManager.demolition = true;
                            }

                        }
                        else if (hit.transform.gameObject.layer == (int)GameLayer.Ground)
                        {
                            buildingName.text = "Tile";
                            buildingNodePos.text = $"{node.xPosition}, {node.yPosition}";
                        }
                        else if (hit.transform.gameObject.layer == (int)GameLayer.Dungeon)
                        {
                            //캔버스에서 던전 갈건지 띄워줌
                            //여기서 addToDungeonQueue에 넣어줌 
                            //캔버스에서 누르는거 체크하는거 여기서 코루틴 돌려줘야할듯

                            StartCoroutine(CheckPushEntranceDungeonButton(hit.transform));
                        }

                        //건설용 같은곳 눌렀는지 확인
                        beforeHitPos = $"({node.xPosition}, {node.yPosition})";
                    }
                    else
                    {
                        node = astar.GetNodeByPosition(hit.point);
                        beforeHitPos = null;
                    }

                    cameraParent.position = new Vector3(node.nodePosition.x, cameraParent.position.y, node.nodePosition.z);
                }
            }    

            isTouched = false;

            if (cameraMove)
            {
                cameraMove = false;    
            }
        }
    }

    public bool enterDungeon;
    public bool cancel;

    IEnumerator CheckPushEntranceDungeonButton(Transform enqueueToDungeonQueue)
    {
        dungeonEnterImageTransform.gameObject.SetActive(true);

        while (true)
        {
            //던전을 골랐을때 캔버스에서 들어갈지 안들어갈지 기다림
            if (enterDungeon)
            {
                addToDungeonQueue(enqueueToDungeonQueue);
                enterDungeon = false;
                break;
            }

            if (cancel)
            {
                cancel = false;
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        dungeonEnterImageTransform.gameObject.SetActive(false);

        yield return null;
    }

    public void DungeonEnterMassageButton(string buttonName)
    {
        switch (buttonName)
        {
            case "enter":
                enterDungeon = true;
                break;
            case "cancel":
                cancel = true;
                break;
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
