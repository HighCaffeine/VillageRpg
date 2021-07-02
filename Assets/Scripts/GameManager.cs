using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PathFinding pathFinding;
    private Astar astar;

    //npc
    [SerializeField] private Transform npcStartTransform;
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform npcPrefab;
    private List<Transform> npcPool;

    private Queue<Transform> setTargetQueue;
    private Queue<Transform> goTargetQueue;
    //npc
    //enemy
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform enemyPrefab;
    private List<Transform> enemyPool;
    //enemy

    [SerializeField] private Transform buildingPrefab;

    //gameInfo
    [SerializeField] private bool pause = false;
    [SerializeField] private int frameRate = 60;
    [SerializeField] private float gameSpeed = 1;

    [SerializeField] private Text timeText;

    private float second = 0f;
    private int week = 0;
    private int month = 0;
    private int year = 0;
    //gameInfo

    private void Awake()
    {
        setTargetQueue = new Queue<Transform>();
        goTargetQueue = new Queue<Transform>();

        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();

        npcPool = new List<Transform>();
        enemyPool = new List<Transform>();
    }

    private void Start()
    {
        Application.targetFrameRate = frameRate;
        NpcPooling();

        EnemyPooling();

        StartCoroutine(CalculateTime());
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

            yield return new WaitForSeconds(1f / gameSpeed);
        }
    }
    //gameInfo

    //enemy

    [SerializeField] private int activeTrueEnemyCount = 0;

    private void EnemyCountCalculate(int value)
    {
        activeTrueEnemyCount += value;
    }

    IEnumerator EnemyActiveTrueWhenAllActiveFalseInEnemyPool()
    {
        while (true)
        {
            if (activeTrueEnemyCount == 0)
            {
                StartCoroutine(SpawnEnemy(Random.Range(0, GameData.Instance.enemyNameList.Count -1), 
                                            Random.Range(0, GameData.Instance.enemyNameList.Count - 1))); 
                                            // 시간따라 소환갯수 늘리는식으로 바꿀듯 
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void EnemyPooling()
    {
        for (int i = 0; i < GameData.Instance.enemyNameList.Count; i++)
        {
            Transform enemy = Instantiate(enemyPrefab, enemyParent);

            enemy.gameObject.SetActive(false);
            enemyPool.Add(enemy);
        }

        StartCoroutine(EnemyActiveTrueWhenAllActiveFalseInEnemyPool());
    }

    //number = 번호
    //count = 갯수
    IEnumerator SpawnEnemy(int enemyNumber, int enemyCount)
    {
        Transform spawnPoint = enemySpawnPoint.GetChild(Random.Range(0, enemySpawnPoint.childCount - 1));
        
        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemy = GetEnemy();
            Transform childTransform = enemy.GetChild(enemyNumber);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            Node spawnPointNode = astar.GetNodeByPosition(spawnPoint.position);
            
            childTransform.gameObject.SetActive(true);

            enemy.name = GameData.Instance.enemyNameList[enemyNumber];
            enemy.position = spawnPointNode.nodePosition + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

            enemyController.isSpawned = true;
            enemyController.dropMoney = GameData.Instance.enemyHealthList[enemyNumber];
            enemyController.health = GameData.Instance.enemyHealthList[enemyNumber];

            if (enemyController.enemyCountPulsOrMinus == null)
            {
                enemyController.enemyCountPulsOrMinus += EnemyCountCalculate;
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

    //private Transform GetTarget()
    //{

    //}

    //enemy

    //NPC
    private void NpcPooling()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            Transform npc = Instantiate(npcPrefab, npcParent);
            npc.position = npcStartTransform.position;
            npc.name = GameData.Instance.npcNameList[i];

            npc.GetChild(Random.Range(0, npc.childCount - 1)).gameObject.SetActive(true);

            npc.gameObject.SetActive(false);

            setTargetQueue.Enqueue(npc);

            npcPool.Add(npc);
            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);
        }

        StartCoroutine(SetTarget());
        StartCoroutine(NpcGoToTarget());
    }

    private Vector3 GetTarget(Vector3 target)
    {
        Node node = null;

        //building, enemy 둘중에 하나 해야됨
        while (true)
        {
            node = astar.GetRandomNodeByLayer((int)GameLayer.building, BuildingType.Shop.ToString());
                
            //갈거 없으면 위로 가서 몹 잡는걸로
            if (target != node.nodePosition)
            {
                break;
            }
            else
            {
                node = astar.GetRandomNodeByLayer((int)GameLayer.building, BuildingType.Shop.ToString());

                if (target != node.nodePosition)
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

        //while (true)
        //{
        //    if (npcController.target)
        //}
        
        Stack<Vector3> path = pathFinding.pathFindDelegate(npcTransform.position, npcController.target);
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        if (path != null)
        {
            //Debug.Log($"{npcTransform.name}, pathCount : {path.Count}");

            int count = path.Count;

            npcTransform.gameObject.SetActive(true);

            npcAnimator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                Vector3 nextPos = path.Pop();

                npcTransform.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
                {
                    npcTransform.Translate(Vector3.forward * 0.05f * gameSpeed);

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
}
