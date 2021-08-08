using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CameraController dungeonCameraController;
    private BuildingManager buildingManager;
    private PathFinding pathFinding;
    private Astar astar;

    //npc

    private Dictionary<string, NpcController> npcControllerDictionary;

    [SerializeField] private Transform woodNpcSpawnPoint;
    [SerializeField] private Transform abyssNpcSpawnPoint;
    [SerializeField] private Transform cellarNpcSpawnPoint;

    [SerializeField] private Transform npcStartTransform;
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform npcPrefab;
    private List<Transform> npcPool;

    private Queue<Transform> setTargetQueue;
    private Queue<Transform> goTargetQueue;
    //npc
    //enemy
    [SerializeField] private Transform woodEnemySpawnPoint;
    [SerializeField] private Transform abyssEnemySpawnPoint;
    [SerializeField] private Transform cellarEnemySpawnPoint;

    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform enemyPrefab;
    private List<Transform> enemyPool;
    [SerializeField] private int maxEnemyPoolCount = 6;
    //enemy

    [SerializeField] private Transform buildingPrefab;

    //gameInfo
    [SerializeField] private bool pause = false;
    [SerializeField] private int frameRate = 60;
    [SerializeField] private int gameSpeed = 5;

    [SerializeField] private Text timeText;
    [SerializeField] private Text moneyText;

    private float second = 0f;
    private int week = 0;
    private int month = 0;
    private int year = 0;
    //gameInfo
    //Dungeon
    [SerializeField] private Transform dungeonCameraParent;
    [SerializeField] private Transform dungeonCamera;
    [SerializeField] private Transform mainCamera;

    [SerializeField] private Image dungeonTapAlphaImage;
    [SerializeField] private Image mainTapAlphaImage;

    [SerializeField] private Transform[] dungeonTransforms;

    public Dictionary<int, DungeonController> dungeonDictionary;

    private bool isDungeonEntrance;

    //Dungeon

    private void Awake()
    {
        npcControllerDictionary = new Dictionary<string, NpcController>();
        npcEntranceSelectedCheckImage = new Dictionary<string, Transform>();

        buildingManager = GetComponent<BuildingManager>();

        setTargetQueue = new Queue<Transform>();
        goTargetQueue = new Queue<Transform>();

        dungeonDictionary = new Dictionary<int, DungeonController>();

        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();

        dungeonEntranceNpcList = new List<Transform>();
        npcPool = new List<Transform>();
        enemyPool = new List<Transform>();

        dungeonCameraController.getCameraLimitValueEachVertexInDungeon += astar.GetCameraLimitValueEachVertexInDungeon;

        buildingManager.pauseGame += PauseGame;
        
        cameraController.getCameraLimitValueEachVertexInMain += astar.GetCameraLimitValueEachVertexInMain;
        cameraController.callACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine += CallACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine;
        cameraController.isDungeonEntrance += IsDungeonEntrance;

        cameraController.pauseGame += PauseGame;

        cameraController.setBuildingValue += buildingManager.SetBuildingValue;
        cameraController.getNowBuilding += buildingManager.GetNowBuilding;
        cameraController.getBuildingWindowActiveSelf += buildingManager.GetBuildingWindowActiveSelf;

        cameraController.getNpcListIsActive = GetNpcListIsActive;
    }

    private void Start()
    {
        //for (int i = 0; i < dungeonTransforms.Length; i++)
        //{
        //    DungeonController dungeonController = dungeonTransforms[i].GetComponent<DungeonController>();
        //    dungeonTransforms[i].name = $"{i}_{GameData.Instance.dungeonActiveNumber[i]}";
        //    dungeonDictionary.Add(i, dungeonController);
        //}

        //설정 저장같은거 만들어서 옮길거
        //GameData.Instance.gameSpeed = gameSpeed;
        Application.targetFrameRate = frameRate;

        moneyText.text = GameData.Instance.money.ToString() + "$";

        StartCoroutine(NpcPooling());
        StartCoroutine(EnemyPooling());
        
        StartCoroutine(CalculateTime());
        StartCoroutine(DungeonActiveWhenRandomTime());

        StartCoroutine(DungeonEntranceListCoroutine());
    }

    //gameInfo

    //시간 계산해줄거
    // 60second = 1week
    // 4week(240second) = 1month
    // 12month(2800second) = 1year

    private void ChangeMoney(int value)
    {
        GameData.Instance.money += value;

        moneyText.text = GameData.Instance.money + "$";
    }

    private void PauseGame(bool pauseOrNot)
    {
        pause = pauseOrNot;
    }

    IEnumerator CalculateTime()
    {
        while (true)
        {
            while (pause)
            {
                if (!pause)
                {
                    break;
                }

                yield return new WaitForSeconds(1f);
            }

            second += 1f;
            week = (int)second / 60 - Mathf.FloorToInt((int)second / 240 * 4);
            month = (int)second / 240 - Mathf.FloorToInt((int)second / 2880 * 12);
            year = (int)second / 2880;

            timeText.text = $"{year + 1}Year{month + 1}Month{week + 1}Week";

            yield return new WaitForSeconds(1f / GameData.Instance.gameSpeed);
        }
    }
    //gameInfo

    //enemy

    private IEnumerator EnemyPooling()
    {
        for (int i = 0; i < maxEnemyPoolCount; i++)
        {
            Transform enemy = Instantiate(enemyPrefab, enemyParent);

            enemy.gameObject.SetActive(false);
            enemyPool.Add(enemy);
        }

        yield return null;
    }

    //number = 번호
    //count = 갯수
    IEnumerator SpawnEnemy(int enemyCount, string dungeonName)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemy = GetEnemy();
            Transform childTransform;
            int enemyNumber = 0;

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            Vector3 spawnPoint = Vector3.zero;
            int dungeonNumber = 0;

            switch (dungeonName)
            {
                case "Wood":
                    spawnPoint = woodEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                    enemyNumber = int.Parse(GameData.Instance.woodDungeonEnemy[Random.Range(0, GameData.Instance.woodDungeonEnemy.Count - 1)].Split('_')[1]);

                    dungeonNumber = (int)Dungeons.Wood;
                    break;
                case "Abyss":
                    spawnPoint = abyssEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                    enemyNumber = int.Parse(GameData.Instance.abyssDungeonEnemy[Random.Range(0, GameData.Instance.abyssDungeonEnemy.Count - 1)].Split('_')[1]);
                    dungeonNumber = (int)Dungeons.Abyss;
                    break;
                case "Cellar":
                    spawnPoint = cellarEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                    enemyNumber = int.Parse(GameData.Instance.cellarDungeonEnemy[Random.Range(0, GameData.Instance.cellarDungeonEnemy.Count - 1)].Split('_')[1]);
                    dungeonNumber = (int)Dungeons.Cellar;
                    break;
            }

            childTransform = enemy.GetChild(enemyNumber);
            
            childTransform.gameObject.SetActive(true);

            enemy.position = spawnPoint;

            enemyController.isSpawned = true;
            enemyController.dropMoney = GameData.Instance.enemyHealthList[enemyNumber];
            enemyController.health = GameData.Instance.enemyHealthList[enemyNumber];
            enemyController.nowDungeonParentNumber = dungeonNumber;

            if (enemyController.calculateEnemyCountInDungeon == null)
            {
                enemyController.calculateEnemyCountInDungeon += CalculateEnemyCountInEachDungeon;
                enemyController.getNodeByPosition += astar.GetNodeByPosition;
            }

            enemy.gameObject.SetActive(true);
        }

        yield return null;
    }

    private Transform GetEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (enemy.gameObject.activeSelf == false)
            {
                return enemy;
            }
        }

        Transform newTransform = Instantiate(enemyPrefab, enemyParent);
        enemyPool.Add(newTransform);

        return newTransform;
    }

    //enemy

    //NPC

    private void SetTargetQueueMethod(Transform npc)
    {
        setTargetQueue.Enqueue(npc);
    }

    IEnumerator NpcPooling()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            Transform npc = Instantiate(npcPrefab, npcParent);
            NpcController npcController = npc.GetComponent<NpcController>();
            npc.position = npcStartTransform.position;
            npc.name = GameData.Instance.npcNameList[i];

            npcController.firstEntrance = true;

            npcController.npcTransform = npc.GetChild(Random.Range(0, npc.childCount - 1));

            npcController.npcTransform.gameObject.SetActive(false);

            setTargetQueue.Enqueue(npc);

            npcPool.Add(npc);

            npcControllerDictionary.Add(npc.name, npcController);
            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);

            if (npcController.setTargetQueueMethod == null)
            {
                npcController.setTargetQueueMethod += SetTargetQueueMethod;
                npcController.getNodeByPosition += astar.GetNodeByPosition;
            }
        }

        //Transform npc = Instantiate(npcPrefab, npcParent);
        //NpcController npcController = npc.GetComponent<NpcController>();
        //npc.position = npcStartTransform.position;
        //npc.name = GameData.Instance.npcNameList[0];

        //npcController.firstEntrance = true;

        //npcController.npcTransform = npc.GetChild(Random.Range(0, npc.childCount - 1));

        //npcController.npcTransform.gameObject.SetActive(false);

        //setTargetQueue.Enqueue(npc);

        //npcPool.Add(npc);

        //GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[0], npc);

        //if (npcController.setTargetQueueMethod == null)
        //{
        //    npcController.setTargetQueueMethod += SetTargetQueueMethod;
        //    npcController.getNodeByPosition += astar.GetNodeByPosition;
        //}

        StartCoroutine(SetTarget());
        StartCoroutine(NpcGoToTarget());

        yield return null;
    }

    private Transform nowDungeonTransform;

    private void GetTarget(NpcController npcController)
    { 
        if (npcController.npcGoToDungeon)
        {
            npcController.targetTransform = nowDungeonTransform;
            npcController.target = nowDungeonTransform.position;
        }
        else
        {
            astar.GetRandomNodeByLayer(npcController, (int)GameLayer.Building, BuildingType.Shop.ToString());
        }
    }

    private IEnumerator SetTarget()
    {
        while (true)
        {
            if (setTargetQueue.Count == 0)
            {
                while (true)
                {
                    if (setTargetQueue.Count != 0)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(1f);
                }
            }

            Transform npcTransform = setTargetQueue.Dequeue();
            NpcController npcController = npcTransform.GetComponent<NpcController>();
            
            GetTarget(npcController);

            goTargetQueue.Enqueue(npcTransform);
        }
    }

    IEnumerator NpcGoToTarget()
    {
        while (true)
        {
            if (goTargetQueue.Count == 0)
            {
                while (true)
                {
                    if (goTargetQueue.Count != 0)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(1f);
                }
            }

            Transform npcTransform = goTargetQueue.Dequeue();
            NpcController npcController = npcTransform.GetComponent<NpcController>();

            npcController.targetTransform = astar.GetNodeByPosition(npcController.target).nodeTransform;

            StartCoroutine(Go(npcTransform, npcController.targetTransform, npcController));
            
            yield return new WaitForSeconds(1f / GameData.Instance.gameSpeed);
        }
    }

    IEnumerator Go(Transform npcTransform, Transform targetTransform, NpcController npcController)
    {
        npcController.endToDo = false;

        while (true)
        {
            if (npcController.endOfSetTarget)
            {
                npcController.endOfSetTarget = false;

                break;
            }

            yield return new WaitForSeconds(1f);
        }

        Stack<Vector3> path = pathFinding.pathFindDelegate(npcTransform.position, npcController.target);
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        bool targetIsActive = true;

        if (path != null)
        {
            int count = path.Count;

            npcController.npcTransform.gameObject.SetActive(true);

            npcAnimator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                Vector3 nextPos = path.Pop();

                npcTransform.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
                {
                    if (pause)
                    {
                        npcAnimator.SetFloat("Speed", 0f);

                        while (true)
                        {
                            if (!pause)
                            {
                                npcAnimator.SetFloat("Speed", 1f);
                                break;
                            }
                        
                            yield return new WaitForSeconds(1f);
                        }
                    }

                    npcTransform.Translate(Vector3.forward * GameData.Instance.gameSpeed * Time.deltaTime);

                    if (!targetTransform.gameObject.activeSelf)
                    {
                        targetIsActive = false;
                        break;
                    }

                    yield return new WaitForSeconds(0.02f / GameData.Instance.gameSpeed);
                }

                if (!targetIsActive)
                {
                    break;
                }

                yield return null;
            }

            if (targetIsActive)
            {
                npcController.npcTransform.gameObject.SetActive(false);

                if (npcController.target == astar.npcStartPosTransformForReturnRandomNode.position)
                {
                    npcTransform.position = astar.npcStartPosTransformForReturnRandomNode.position;
                }
            }

            npcAnimator.SetFloat("Speed", 0f);
        }

        if (!npcController.npcGoToDungeon)
        {
            npcController.endToDo = true;
            setTargetQueue.Enqueue(npcTransform);
        }
        else
        {
            NpcMoveToNowActiveDungeon(npcTransform);
        }
    }
    //NPC
    //Dungeon 던전 클리어때 이름 0~2아무거나 지정해줌

    private void NpcMoveToNowActiveDungeon(Transform npcTransform)
    {
        string[] names = nowDungeonTransform.name.Split('_');

        switch (names[1])
        {
            case "Wood":
                npcTransform.position = woodNpcSpawnPoint.position;
                break;
            case "Abyss":
                npcTransform.position = abyssNpcSpawnPoint.position;
                break;
            case "Cellar":
                npcTransform.position = cellarNpcSpawnPoint.position;
                break;
        }
    }

    IEnumerator DungeonActiveWhenRandomTime()
    {
        while (true)
        {
            foreach (var dungeon in dungeonTransforms)
            {
                yield return new WaitForSeconds(1f);

                string[] names = dungeon.name.Split('_');

                Transform dungeonChild = dungeon.GetChild(int.Parse(names[1]));

                if (!dungeonChild.gameObject.activeSelf)
                {
                    Node node = astar.GetNodeByPosition(dungeon.position);

                    node.layerNumber = (int)GameLayer.Dungeon;
                    node.nodeTransform = dungeonChild;

                    dungeonChild.gameObject.SetActive(true);

                    DungeonController dungeonController = dungeon.GetComponent<DungeonController>();

                    dungeonController.beforeChildCount = int.Parse(names[1]);
                    dungeonController.dungeonName = dungeonChild.name;

                    if (!dungeonDictionary.ContainsKey(int.Parse(names[0])))
                    {
                        dungeonDictionary.Add(int.Parse(names[0]), dungeonController);
                    }
                }
            }
        }
    }

    private void CalculateEnemyCountInEachDungeon(int dungeonParentNumber, int value)
    {
        dungeonDictionary.TryGetValue(dungeonParentNumber, out DungeonController dungeonController);
        dungeonController.activeTrueEnemyCount += value;
    }

    public void CallACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine(Transform dungeonTransform)
    {
        StartCoroutine(ActiveFalseDungeonSettingAfterDungeonActivetrue(dungeonTransform));
    }

    IEnumerator ActiveFalseDungeonSettingAfterDungeonActivetrue(Transform dungeonTransform)
    {
        isDungeonEntrance = true;

        nowDungeonTransform = dungeonTransform;
        Transform dungeonParent = dungeonTransform.parent;

        string[] names = dungeonParent.name.Split('_');
        string dungeonName = null;

        dungeonDictionary.TryGetValue(int.Parse(names[0]), out DungeonController dungeonController);

        switch (int.Parse(names[1]))
        {
            case 0:
                dungeonController.dungeonName = Dungeons.Wood.ToString();
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Wood.ToString());
                dungeonName = "Wood";

                dungeonCameraParent.position = woodEnemySpawnPoint.position;
                break;
            case 1:
                dungeonController.dungeonName = Dungeons.Abyss.ToString();
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Abyss.ToString());
                dungeonName = "Abyss";

                dungeonCameraParent.position = abyssEnemySpawnPoint.position;
                break;
            case 2:
                dungeonController.dungeonName = Dungeons.Cellar.ToString();
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Cellar.ToString());
                dungeonName = "Cellar";

                dungeonCameraParent.position = cellarEnemySpawnPoint.position;
                break;
        }

        dungeonTapAlphaImage.gameObject.SetActive(false);

        StartCoroutine(SpawnEnemy(Random.Range(3, 7), dungeonName));

        StartCoroutine(DungeonEnterNpcCheck());

        yield return null;
    }

    public void SwitchMainCameraAndDungeonCamera(string thisCameraName)
    {
        switch (thisCameraName)
        {
            case "main":
                if (!mainTapAlphaImage.gameObject.activeSelf)
                {
                    dungeonTapAlphaImage.gameObject.SetActive(false);
                    mainTapAlphaImage.gameObject.SetActive(true);

                    dungeonCamera.gameObject.SetActive(false);
                    mainCamera.gameObject.SetActive(true);
                }

                break;
            case "dungeon":
                if (!dungeonTapAlphaImage.gameObject.activeSelf)
                {
                    dungeonTapAlphaImage.gameObject.SetActive(true);
                    mainTapAlphaImage.gameObject.SetActive(false);

                    dungeonCamera.gameObject.SetActive(true);
                    mainCamera.gameObject.SetActive(false);
                }

                break;
        }
    }

    private bool IsDungeonEntrance()
    {
        return isDungeonEntrance;
    }

    [SerializeField] private Transform dungeonEntranceNpcButtonPrefab;

    private Dictionary<string, Transform> npcEntranceSelectedCheckImage;
    private List<Transform> dungeonEntranceNpcList;

    [SerializeField] private Transform npcEnterListTransform;

    [SerializeField] private RectTransform npcEnterListRectTransform;

    private bool enterTheDungeon = false;
    private int currentEntranceNpcCount = 0;

    [SerializeField] private Transform dungeonEnterButtonAlphaImageTransform;

    private bool GetNpcListIsActive()
    {
        return npcEnterListTransform.parent.parent.gameObject.activeSelf;
    }

    IEnumerator DungeonEnterNpcCheck()
    {
        PauseGame(true);
        npcEnterListTransform.parent.parent.gameObject.SetActive(true);

        while (true)
        {
            if (enterTheDungeon)
            {
                break;
            }

            yield return new WaitForSeconds(1f);
        }

        PauseGame(false);

        dungeonEnterButtonAlphaImageTransform.gameObject.SetActive(true);
        npcEnterListTransform.parent.parent.gameObject.SetActive(false);

        //List에 있는거 던전으로 target정하고 던전 안에있는 enemy를 enemyTarget으로 정하고
        //던전 내부에 enemy는 처음 들어온애를 target으로함 
        
        foreach (var npcTransform in dungeonEntranceNpcList)
        {
            StartCoroutine(CheckNpcEndToDo(npcTransform.name));
        }
    }

    IEnumerator CheckNpcEndToDo(string npcName)
    {
        NpcController npcController = npcControllerDictionary[npcName];

        while (!npcController.endToDo)
        {
            if (npcController.endToDo)
            {
                break;
            }

            yield return new WaitForSeconds(1f);
        }

        npcController.npcGoToDungeon = true;
    }

    public void EnterTheDungeon()
    {
        enterTheDungeon = true;
    }

    IEnumerator DungeonEntranceListCoroutine()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            if (i >= 6)
            {
                npcEnterListRectTransform.sizeDelta = new Vector2(npcEnterListRectTransform.sizeDelta.x, npcEnterListRectTransform.sizeDelta.y + 130f);
            }

            MakeDungeonEntranceNpcList(GameData.Instance.npcNameList[i]);
        }

        npcEnterListTransform.parent.parent.gameObject.SetActive(false);

        yield return null;
    }


    private void MakeDungeonEntranceNpcList(string npcName)
    {
        Transform newNpcEntranceButton = Instantiate(dungeonEntranceNpcButtonPrefab, npcEnterListTransform);

        npcEntranceSelectedCheckImage.Add(npcName, newNpcEntranceButton.GetChild(1));

        Button button = newNpcEntranceButton.GetComponent<Button>();

        Text nameInEntranceButton = newNpcEntranceButton.GetChild(2).GetComponent<Text>();
        Text maxHealthInEntranceButton = newNpcEntranceButton.GetChild(3).GetComponent<Text>();
        Text damageInEntranceButton = newNpcEntranceButton.GetChild(4).GetComponent<Text>();
        Text armorInEntranceButton = newNpcEntranceButton.GetChild(5).GetComponent<Text>();
        Text fatigueInEntranceButton = newNpcEntranceButton.GetChild(6).GetComponent<Text>();

        nameInEntranceButton.text = npcName;
        maxHealthInEntranceButton.text = GameData.Instance.npcMaxHealthDictionary[npcName].ToString();
        damageInEntranceButton.text = GameData.Instance.npcDamageDictionary[npcName].ToString();
        armorInEntranceButton.text = GameData.Instance.npcArmorDictionary[npcName].ToString();
        fatigueInEntranceButton.text = GameData.Instance.npcFatigueDictionary[npcName].ToString();

        button.onClick.AddListener(delegate { NpcAddToEntranceQueue(npcName); });
    }


    private void NpcAddToEntranceQueue(string npcName)
    {
        Transform npcTransform = null;

        Debug.Log(npcName);

        npcTransform = GameData.Instance.npcTransformDictionary[npcName];

        if (dungeonEntranceNpcList.Contains(npcTransform))
        {
            npcEntranceSelectedCheckImage[npcName].gameObject.SetActive(false);
            dungeonEntranceNpcList.Remove(npcTransform);

            currentEntranceNpcCount--;
        }
        else if (currentEntranceNpcCount < 6)
        {
            if (dungeonEnterButtonAlphaImageTransform.gameObject.activeSelf)
            {
                dungeonEnterButtonAlphaImageTransform.gameObject.SetActive(false);
            }

            npcEntranceSelectedCheckImage[npcName].gameObject.SetActive(true);
            dungeonEntranceNpcList.Add(npcTransform);

            currentEntranceNpcCount++;
        }

        if (currentEntranceNpcCount == 0)
        {
            dungeonEnterButtonAlphaImageTransform.gameObject.SetActive(true);
        }
    }

    //Dungeon
}
