using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, GameInputSystem.IMouseActions
{
    public delegate void PauseGame(bool pauseOrNot);
    public PauseGame pauseGame;

    public Button rotateButton;
    public RectTransform rotateRect;

    public Transform cameraParent;
    [SerializeField] private Camera screenCamera;

    private GameInputSystem gameInputSystem;

    [SerializeField] private Astar astar;

    public RectTransform clickableArea;

    [SerializeField] private float isClickableBottomValue;
    [SerializeField] private float isClickableUpperValue;

    [SerializeField] private Transform dungeonEnterTransform;

    [SerializeField] private Image dungeonEnterImage;

    [SerializeField] private Sprite woodEnterMessageImage;
    [SerializeField] private Sprite abyssEnterMessageImage;
    [SerializeField] private Sprite cellarEnterMessageImage;

    public delegate void CallACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine(Transform dungeonTransform);
    public CallACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine callACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine;

    private void Awake()
    {
        isClickableBottomValue = clickableArea.anchoredPosition.y;
        isClickableUpperValue = clickableArea.anchoredPosition.y + clickableArea.rect.height;

        cameraParent = transform.parent;

        screenCamera = cameraParent.GetChild(0).GetComponent<Camera>();

        gameInputSystem = new GameInputSystem();
        gameInputSystem.Mouse.SetCallbacks(this);
    }

    private void Start()
    {
        if (isMainCamera)
        {
            SetCameraLimitMethod();
        }
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

    [SerializeField] private float enemySpawnPointLeftLimit;
    [SerializeField] private float enemySpawnPointRightLimit;
    [SerializeField] private float enemySpawnPointBottomLimit;
    [SerializeField] private float enemySpawnPointUpperLimit;

    [SerializeField] private float worldLeftLimit;
    [SerializeField] private float worldRightLimit;
    [SerializeField] private float worldBottomLimit;
    [SerializeField] private float worldUpperLimit;

    // bottom, upper, right, left
    public delegate float GetCameraLimitValueEachVertexInMain(string vertexName);
    public delegate float GetCameraLimitValueEachVertexInDungeon(string vertexName, string dungeonName);
    public GetCameraLimitValueEachVertexInMain getCameraLimitValueEachVertexInMain;
    public GetCameraLimitValueEachVertexInDungeon getCameraLimitValueEachVertexInDungeon;

    public delegate bool IsDungeonEntrance();
    public IsDungeonEntrance isDungeonEntrance;

    public delegate bool GetBuildingWindowActiveSelf();
    public GetBuildingWindowActiveSelf getBuildingWindowActiveSelf;

    public void SetCameraLimitMethod(string dungeonName)
    {
        enemySpawnPointLeftLimit = getCameraLimitValueEachVertexInDungeon("left", dungeonName);
        enemySpawnPointBottomLimit = getCameraLimitValueEachVertexInDungeon("bottom", dungeonName);
        enemySpawnPointRightLimit = getCameraLimitValueEachVertexInDungeon("right", dungeonName);
        enemySpawnPointUpperLimit = getCameraLimitValueEachVertexInDungeon("upper", dungeonName);
    }

    private void SetCameraLimitMethod()
    {
        worldLeftLimit = getCameraLimitValueEachVertexInMain("left");
        worldRightLimit = getCameraLimitValueEachVertexInMain("right");
        worldBottomLimit = getCameraLimitValueEachVertexInMain("bottom");
        worldUpperLimit = getCameraLimitValueEachVertexInMain("upper");
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        //메인 필드 카메라
        if (context.performed && isTouched && !getBuildingWindowActiveSelf())
        {
            cameraMove = true;

            Vector3 deltaValue = new Vector3(-context.ReadValue<Vector2>().x, 0f, -context.ReadValue<Vector2>().y) * 0.01f;

            Vector3 newCameraParentPos = deltaValue + new Vector3(cameraParent.position.x, 0f, cameraParent.position.z);

            if (isMainCamera
                && worldLeftLimit <= newCameraParentPos.x
                && worldRightLimit >= newCameraParentPos.x
                && worldBottomLimit <= newCameraParentPos.z
                && worldUpperLimit >= newCameraParentPos.z)
            {
                cameraParent.Translate(deltaValue);
            }
            else if (!isMainCamera
                && enemySpawnPointLeftLimit <= newCameraParentPos.x
                && enemySpawnPointRightLimit >= newCameraParentPos.x
                && enemySpawnPointBottomLimit <= newCameraParentPos.z
                && enemySpawnPointUpperLimit >= newCameraParentPos.z)
            {
                cameraParent.Translate(deltaValue);
            }
        }
    }

    [SerializeField] private LayerMask chooseLayerMask;
    public Text buildingNodePos;
    public Text buildingName;

    [SerializeField] private string beforeHitPos;

    public bool isMainCamera;

    public delegate bool GetNowBuilding();
    public GetNowBuilding getNowBuilding;

    public delegate bool GetNowSelectingBuilding();
    public GetNowSelectingBuilding getNowSelectingBuilding;

    public delegate void SetBuildingValue(bool value);
    public SetBuildingValue setBuildingValue;

    public delegate bool GetNpcListIsActive();
    public GetNpcListIsActive getNpcListIsActive;

    public void OnTouch(InputAction.CallbackContext context)
    {
        if (context.started && !dungeonEnterTransform.gameObject.activeSelf && !getNowSelectingBuilding() && !getNpcListIsActive())
        {
            isTouched = true;
        }

        if (context.canceled&& !dungeonEnterTransform.gameObject.activeSelf && !getNowSelectingBuilding() && !getNpcListIsActive())
        {

            isTouched = false;

            if (isMainCamera && positionValue.y > isClickableBottomValue && positionValue.y < isClickableUpperValue)
            {
                if (!cameraMove)
                {
                    Node node;
                    Ray ray = screenCamera.ScreenPointToRay(positionValue);
                    Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, chooseLayerMask);

                    if (hit.transform != null)
                    {
                        node = astar.GetNodeByPosition(hit.transform.position, false, null);

                        if (hit.transform.gameObject.layer == (int)GameLayer.Building
                            || hit.transform.gameObject.layer == (int)GameLayer.Road)
                        {
                            //건물 정보 가져오기
                            buildingNodePos.text = $"({node.xPosition}, {node.yPosition})";
                            buildingName.text = $"{node.buildingName}";
                        
                            if (beforeHitPos == buildingNodePos.text && getNowBuilding())
                            {
                                setBuildingValue(true);
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

                            string[] names = hit.transform.name.Split('_');

                            buildingName.text = $"Dungeon : {names[1]}";

                            if (!isDungeonEntrance())
                            {
                                StartCoroutine(CheckPushEntranceDungeonButton(hit.transform)); 
                            }
                        }

                        //건설용 같은곳 눌렀는지 확인
                        beforeHitPos = $"({node.xPosition}, {node.yPosition})";
                    }
                    else
                    {
                        node = astar.GetNodeByPosition(hit.point, false, null);
                        beforeHitPos = null;
                    }

                    cameraParent.position = new Vector3(node.nodePosition.x, cameraParent.position.y, node.nodePosition.z);
                }
            }    

            if (cameraMove)
            {
                cameraMove = false;    
            }
        }
    }

    public bool enterDungeon;
    public bool cancel;

    public delegate void SetDungeonBuildingNumber(int value);
    public SetDungeonBuildingNumber setDungeonBuildingNumber;

    //pause 추가
    IEnumerator CheckPushEntranceDungeonButton(Transform dungeonTransform)
    {
        pauseGame(true);

        string[] names = dungeonTransform.name.Split('_');

        switch (names[1])
        {
            case "Wood":
                dungeonEnterImage.sprite = woodEnterMessageImage;
                break;
            case "Abyss":
                dungeonEnterImage.sprite = abyssEnterMessageImage;
                break;
            case "Cellar":
                dungeonEnterImage.sprite = cellarEnterMessageImage;
                break;
        }

        dungeonEnterTransform.gameObject.SetActive(true);

        while (true)
        {
            //던전을 골랐을때 캔버스에서 들어갈지 안들어갈지 기다림
            if (enterDungeon)
            {
                setDungeonBuildingNumber(int.Parse(names[0]));
                callACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine(dungeonTransform);
                enterDungeon = false;
                break;
            }

            if (cancel)
            {
                pauseGame(false);
                cancel = false;
                break;
            }

            yield return new WaitForSeconds(1f);
        }

        dungeonEnterTransform.gameObject.SetActive(false);

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
