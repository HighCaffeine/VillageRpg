using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    private PathFinding pathFinding;
    private Astar astar;

    //npc
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
    [SerializeField] private Transform dungeonCamera;
    [SerializeField] private Transform mainCamera;

    public Queue<Transform> setDungeonWhenEnqueueDungeomFromCameraController;

    [SerializeField] private Image dungeonTapAlphaImage;
    [SerializeField] private Image mainTapAlphaImage;

    [SerializeField] private Transform[] dungeonTransforms;

    private Dictionary<int, DungeonController> dungeonDictionary;

    //Dungeon

    private void Awake()
    {
        setTargetQueue = new Queue<Transform>();
        goTargetQueue = new Queue<Transform>();

        setDungeonWhenEnqueueDungeomFromCameraController = new Queue<Transform>();

        dungeonDictionary = new Dictionary<int, DungeonController>();

        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();

        npcPool = new List<Transform>();
        enemyPool = new List<Transform>();

        cameraController.addToDungeonQueue += AddToDungeonQueue;
        cameraController.pauseGame += PauseGame;

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
        //Application.targetFrameRate = frameRate;

        StartCoroutine(NpcPooling());
        StartCoroutine(EnemyPooling());
        
        StartCoroutine(CalculateTime());
        StartCoroutine(DungeonActiveWhenRandomTime());
        StartCoroutine(ActiveFalseDungeonSettingAfterDungeonActivetrue());
    }

    //gameInfo

    //시간 계산해줄거
    // 60second = 1week
    // 4week(240second) = 1month
    // 12month(2800second) = 1year

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

                yield return new WaitForFixedUpdate();
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
    IEnumerator SpawnEnemy(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemy = GetEnemy();
            Transform childTransform;
            string[] names = new string[2];

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            Node spawnPointNode = null;
            int dungeonNumber = 0;

            switch (nowDungeonName)
            {
                case "Wood":
                    spawnPointNode = astar.GetNodeByPosition(woodEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
                    names = GameData.Instance.woodDungeonEnemy[Random.Range(0, GameData.Instance.woodDungeonEnemy.Count - 1)].Split('_');
                    dungeonNumber = (int)Dungeons.Wood;
                    break;
                case "Abyss":
                    spawnPointNode = astar.GetNodeByPosition(abyssEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
                    names = GameData.Instance.abyssDungeonEnemy[Random.Range(0, GameData.Instance.abyssDungeonEnemy.Count - 1)].Split('_');
                    dungeonNumber = (int)Dungeons.Abyss;
                    break;
                case "Cellar":
                    spawnPointNode = astar.GetNodeByPosition(cellarEnemySpawnPoint.position + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
                    names = GameData.Instance.cellarDungeonEnemy[Random.Range(0, GameData.Instance.cellarDungeonEnemy.Count - 1)].Split('_');
                    dungeonNumber = (int)Dungeons.Cellar;
                    break;
            }

            int enemyNumber = int.Parse(names[1]);

            childTransform = enemy.GetChild(enemyNumber);
            
            childTransform.gameObject.SetActive(true);

            enemy.position = spawnPointNode.nodePosition + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

            enemyController.isSpawned = true;
            enemyController.dropMoney = GameData.Instance.enemyHealthList[enemyNumber];
            enemyController.health = GameData.Instance.enemyHealthList[enemyNumber];
            enemyController.parentXPos = spawnPointNode.xPosition;
            enemyController.parentYPos = spawnPointNode.yPosition;
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

    private IEnumerator NpcPooling()
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

            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);

            if (npcController.setTargetQueueMethod == null)
            {
                npcController.setTargetQueueMethod += SetTargetQueueMethod;
                npcController.getNodeByPosition += astar.GetNodeByPosition;
            }
        }

        StartCoroutine(SetTarget());
        StartCoroutine(NpcGoToTarget());

        yield return null;
    }

    private void GetTarget(NpcController npcController)
    {
        astar.GetRandomNodeByLayer(npcController, (int)GameLayer.Building, BuildingType.Shop.ToString());
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

                    yield return new WaitForFixedUpdate();
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

                    yield return new WaitForFixedUpdate();
                }
            }

            Transform npcTransform = goTargetQueue.Dequeue();
            NpcController npcController = npcTransform.GetComponent<NpcController>();

            StartCoroutine(Go(npcTransform, astar.GetNodeByPosition(npcController.target).nodeTransform, npcController));
            
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Go(Transform npcTransform, Transform targetTransform, NpcController npcController)
    {
        while (true)
        {
            if (npcController.endOfSetTarget)
            {
                npcController.endOfSetTarget = false;

                break;
            }

            yield return new WaitForFixedUpdate();
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
                        while (true)
                        {
                            if (!pause)
                            {
                                break;
                            }
                        }

                        yield return new WaitForFixedUpdate();
                    }

                    npcTransform.Translate(Vector3.forward * 0.05f * GameData.Instance.gameSpeed);

                    if (!targetTransform.gameObject.activeSelf)
                    {
                        targetIsActive = false;
                        break;
                    }

                    yield return new WaitForFixedUpdate();
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

        setTargetQueue.Enqueue(npcTransform);
    }
    //NPC
    //Dungeon 던전 클리어때 이름 0~2아무거나 지정해줌

    IEnumerator DungeonActiveWhenRandomTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);

            foreach (var dungeon in dungeonTransforms)
            {
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

                    break;
                }
            }
        }
    }

    private string nowDungeonName;

    private void AddToDungeonQueue(Transform dungeon)
    {
        setDungeonWhenEnqueueDungeomFromCameraController.Enqueue(dungeon);
    }

    private void CalculateEnemyCountInEachDungeon(int dungeonParentNumber, int value)
    {
        dungeonDictionary.TryGetValue(dungeonParentNumber, out DungeonController dungeonController);
        dungeonController.activeTrueEnemyCount += value;
    }
    IEnumerator ActiveFalseDungeonSettingAfterDungeonActivetrue()
    {
        while (true)
        {
            while (true)
            {
                if (setDungeonWhenEnqueueDungeomFromCameraController.Count != 0)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            Transform dungeon = setDungeonWhenEnqueueDungeomFromCameraController.Dequeue();
            Transform dungeonParent = dungeon.parent;

            string[] names = dungeonParent.name.Split('_');

            dungeonDictionary.TryGetValue(int.Parse(names[0]), out DungeonController dungeonController);

            switch (int.Parse(names[1]))
            {
                case 0:
                    nowDungeonName = Dungeons.Wood.ToString();
                    dungeonController.dungeonName = Dungeons.Wood.ToString();
                    break;
                case 1:
                    nowDungeonName = Dungeons.Abyss.ToString();
                    dungeonController.dungeonName = Dungeons.Abyss.ToString();
                    break;
                case 2:
                    nowDungeonName = Dungeons.Cellar.ToString();
                    dungeonController.dungeonName = Dungeons.Cellar.ToString();
                    break;
            }

            dungeonTapAlphaImage.gameObject.SetActive(false);

            StartCoroutine(SpawnEnemy(Random.Range(3, 7)));
        }
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

    //Dungeon
}
