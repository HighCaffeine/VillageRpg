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
        //���� �ʵ� ī�޶�
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
        else // ���� ī�޶�
        {
            //��������Ʈ�� �����;��ҵ� -> ���� ���° �����ִ��� �˾ƾߵ�
            //�������� ī�޶� �����̴°� ���� �����ؾߵ�
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
                            //�ǹ� ���� ��������
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
                            //ĵ�������� ���� ������ �����
                            //���⼭ addToDungeonQueue�� �־��� 
                            //ĵ�������� �����°� üũ�ϴ°� ���⼭ �ڷ�ƾ ��������ҵ�

                            StartCoroutine(CheckPushEntranceDungeonButton(hit.transform));
                        }

                        //�Ǽ��� ������ �������� Ȯ��
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
            //������ ������� ĵ�������� ���� �ȵ��� ��ٸ�
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
