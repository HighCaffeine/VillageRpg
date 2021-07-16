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
    [SerializeField] private Camera dungeonCamera;
    [SerializeField] private Camera mainCamera;

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

        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();

        npcPool = new List<Transform>();
        enemyPool = new List<Transform>();

        cameraController.addToDungeonQueue += AddToDungeonQueue;

        for (int i = 0; i < dungeonTransforms.Length; i++)
        {
            DungeonController dungeonController = dungeonTransforms[i].GetComponent<DungeonController>();

            dungeonTransforms[i].name = GameData.Instance.dungeonActiveNumber[i].ToString();
            dungeonDictionary.Add(i, dungeonController);
        }
    }

    private void Start()
    {
        GameData.Instance.gameSpeed = gameSpeed;
        Application.targetFrameRate = frameRate;
        NpcPooling();
        EnemyPooling();

        StartCoroutine(CalculateTime());
        StartCoroutine(DungeonActiveWhenRandomTime());
        StartCoroutine(ActiveFalseDungeonSettingAfterDungeonActivetrue());
    }

    //gameInfo

    //시간 계산해줄거
    // 60second = 1week
    // 4week(240second) = 1month
    // 12month(2800second) = 1year
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

    IEnumerator EnemyActiveTrueWhenAllActiveFalseInEnemyPool()
    {
        StartCoroutine(SpawnEnemy(Random.Range(3, 7)));

        yield return null;
    }

    private void EnemyPooling()
    {
        for (int i = 0; i < maxEnemyPoolCount; i++)
        {
            Transform enemy = Instantiate(enemyPrefab, enemyParent);

            enemy.gameObject.SetActive(false);
            enemyPool.Add(enemy);
        }
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
            Node spawnPointNode = new Node(0, 0, Vector3.zero, false);
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
            enemyController.nowDungeonNumber = dungeonNumber;

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

    private void NpcPooling()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            Transform npc = Instantiate(npcPrefab, npcParent);
            NpcController npcController = npc.GetComponent<NpcController>();
            npc.position = npcStartTransform.position;
            npc.name = GameData.Instance.npcNameList[i];

            npc.GetChild(Random.Range(0, npc.childCount - 1)).gameObject.SetActive(true);

            npc.gameObject.SetActive(false);

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
    }

    private Vector3 GetTarget(Vector3 targetNodePosition)
    {
        Node node = null;

        //building, enemy 둘중에 하나 해야됨
        while (true)
        {
            node = astar.GetRandomNodeByLayer((int)GameLayer.Building, BuildingType.Shop.ToString());
            
            //갈거 없으면 위로 가서 몹 잡는걸로
            if (targetNodePosition != node.nodePosition)
            {
                break;
            }
            else
            {
                node = astar.GetRandomNodeByLayer((int)GameLayer.Building, BuildingType.Shop.ToString());

                if (targetNodePosition != node.nodePosition)
                {
                    break;
                }
            }
        }

        return node.nodePosition;
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

            npcController.target = GetTarget(npcController.target);

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

            StartCoroutine(Go(npcTransform));
            
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Go(Transform npcTransform)
    {
        NpcController npcController = npcTransform.GetComponent<NpcController>();

        Stack<Vector3> path = pathFinding.pathFindDelegate(npcTransform.position, npcController.target);
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        if (path != null)
        {
            int count = path.Count;

            npcTransform.gameObject.SetActive(true);

            npcAnimator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                Vector3 nextPos = path.Pop();

                npcTransform.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
                {
                    npcTransform.Translate(Vector3.forward * 0.05f * GameData.Instance.gameSpeed);

                    yield return new WaitForFixedUpdate();
                }

                yield return null;
            }

            npcTransform.gameObject.SetActive(false);
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
            yield return new WaitForSeconds(100f);

            foreach (var dungeon in dungeonTransforms)
            {
                if (!dungeonTransforms[int.Parse(dungeon.name)].gameObject.activeSelf)
                {
                    dungeonTransforms[int.Parse(dungeon.name)].gameObject.SetActive(true);
                }
            }
        }
    }

    private string nowDungeonName;
    
    private void AddToDungeonQueue(Transform dungeon)
    {
        setDungeonWhenEnqueueDungeomFromCameraController.Enqueue(dungeon);
    }

    private void CalculateEnemyCountInEachDungeon(int dungeonNumber, int value)
    {
        dungeonDictionary.TryGetValue(dungeonNumber, out DungeonController dungeonController);
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

            string[] names = dungeon.name.Split('_');

            nowDungeonName = names[1];
            dungeonDictionary.TryGetValue(int.Parse(dungeonParent.name), out DungeonController dungeonController);

            dungeonController.dungeonName = names[1];

            StartCoroutine(EnemyActiveTrueWhenAllActiveFalseInEnemyPool());
        }
    }

    public void SwitchMainCameraAndDungeonCamera(string thisCameraName)
    {
        switch (thisCameraName)
        {
            case "main":
                if (mainTapAlphaImage.gameObject.activeSelf)
                {
                    dungeonTapAlphaImage.gameObject.SetActive(false);
                    mainTapAlphaImage.gameObject.SetActive(true);

                    cameraController.isMainCamera = true;
                }

                break;
            case "dungeon":
                if (dungeonTapAlphaImage.gameObject.activeSelf)
                {
                    mainTapAlphaImage.gameObject.SetActive(false);
                    dungeonTapAlphaImage.gameObject.SetActive(true);

                    cameraController.isMainCamera = false;
                }

                break;
        }
    }

    //Dungeon
}
