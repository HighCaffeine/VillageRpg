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

    [SerializeField] private Transform weaponParentPrefab;

    //npc
    //enemy
    [SerializeField] private Transform woodEnemySpawnPoint;
    [SerializeField] private Transform abyssEnemySpawnPoint;
    [SerializeField] private Transform cellarEnemySpawnPoint;

    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform enemyPrefab;
    private List<Transform> enemyPool;
    [SerializeField] private int maxEnemyPoolCount = 6;

    private Dictionary<string, EnemyController> enemyControllerDictionary;
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

    public delegate void SaveToJson();
    public SaveToJson saveToJson;

    private JsonManager jsonManager;

    [SerializeField] private Image waitForCompeleteSaveCheckImage;

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

    [SerializeField] private Transform dungeonEntranceNpcButtonPrefab;

    private Dictionary<string, Transform> npcEntranceSelectedCheckImage;
    private List<Transform> dungeonEntranceNpcList;
    public List<Transform> dungeonEnemyTransformList;

    [SerializeField] private Transform npcEnterListTransform;

    [SerializeField] private RectTransform npcEnterListRectTransform;

    private bool enterTheDungeon = false;
    private int currentEntranceNpcCount = 0;

    [SerializeField] private Transform dungeonEnterButtonAlphaImageTransform;

    [SerializeField] private Transform dungeonEnterCancelButton;

    [SerializeField] private bool dungeonEnterCancel = false;

    private int dungeonBuildingNumber;
    //Dungeon

    private void Awake()
    {
        jsonManager = GameObject.FindGameObjectWithTag("JsonManager").GetComponent<JsonManager>();

        npcControllerDictionary = new Dictionary<string, NpcController>();
        npcEntranceSelectedCheckImage = new Dictionary<string, Transform>();

        buildingManager = GetComponent<BuildingManager>();

        enemyControllerDictionary = new Dictionary<string, EnemyController>();
        dungeonDictionary = new Dictionary<int, DungeonController>();

        pathFinding = GetComponent<PathFinding>();
        astar = GetComponent<Astar>();

        dungeonEntranceNpcList = new List<Transform>();
        npcPool = new List<Transform>();
        enemyPool = new List<Transform>();

        dungeonEnemyTransformList = new List<Transform>();

        dungeonCameraController.getCameraLimitValueEachVertexInDungeon += astar.GetCameraLimitValueEachVertexInDungeon;
        dungeonCameraController.getNowSelectingBuilding += buildingManager.GetNowSelectingBuilding;
        dungeonCameraController.getNpcListIsActive += GetNpcListIsActive;
        dungeonCameraController.getBuildingWindowActiveSelf += buildingManager.GetBuildingWindowActiveSelf;

        buildingManager.pauseGame += PauseGame;
        buildingManager.addMoney += AddMoney;
        buildingManager.setTargetDelegate += SetTargetDelegate;

        cameraController.getCameraLimitValueEachVertexInMain += astar.GetCameraLimitValueEachVertexInMain;
        cameraController.callACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine += CallACtiveFalseDungeonSettingAfterDungeonActivetrueCoroutine;
        cameraController.isDungeonEntrance += IsDungeonEntrance;
        cameraController.setDungeonBuildingNumber += SetDungeonBuildingNumber;

        cameraController.pauseGame += PauseGame;

        cameraController.getNowBuilding += buildingManager.GetNowBuilding;
        cameraController.setBuildingValue += buildingManager.SetBuildingValue;
        cameraController.getNowSelectingBuilding += buildingManager.GetNowSelectingBuilding;
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

    public void CallSaveCoroutine()
    {
        StartCoroutine(Save());
    }

    private IEnumerator Save()
    {
        PauseGame(true);
        waitForCompeleteSaveCheckImage.transform.gameObject.SetActive(true);

        jsonManager.SaveToJson();

        while (jsonManager.SaveIsDone())
        {
            yield return new WaitForSeconds(1f);
        }

        jsonManager.IsNotDoneFalseIntoTrue();

        yield return new WaitForSeconds(1f);

        waitForCompeleteSaveCheckImage.transform.gameObject.SetActive(false);
        PauseGame(false);
    }

    private void AddMoney(int value)
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

    IEnumerator EnemyPooling()
    {
        for (int i = 0; i < maxEnemyPoolCount; i++)
        {
            Transform enemy = Instantiate(enemyPrefab, enemyParent);

            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            for (int childEnemyCount = 0; childEnemyCount < enemy.childCount; childEnemyCount++)
            {
                Animator animator = enemy.GetChild(childEnemyCount).GetComponent<Animator>();

                enemyController.enemyAnimatorList.Add(animator);
            }

            enemyControllerDictionary.Add(i.ToString(), enemyController);

            enemy.gameObject.SetActive(false);
            enemyController.endOfPooling = true;
            enemyPool.Add(enemy);
        }

        yield return null;
    }

    private int[,] enemyNodeArray = {{ 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },};

    private void SetEnemyNodeArrayOneIntoZero(int xPos, int yPos)
    {
        enemyNodeArray[xPos, yPos] = 0;
    }

    //number = 번호
    //count = 갯수
    IEnumerator SpawnEnemy(int enemyCount, string dungeonName)
    {
        Vector3 dungeonEnemyLeftBottomPoint;
        DungeonController dungeonController = dungeonDictionary[dungeonBuildingNumber];

        switch (dungeonName)
        {
            case "Wood":
                dungeonEnemyLeftBottomPoint = astar.GetNodeByPosition(astar.woodDungeonMainNodeTransform.position - new Vector3(astar.dungeonXSize * 0.5f, 0f, 0f), true, "Wood").nodePosition;
                break;
            case "Abyss":
                dungeonEnemyLeftBottomPoint = astar.GetNodeByPosition(astar.abyssDungeonMainNodeTransform.position - new Vector3(astar.dungeonXSize * 0.5f, 0f, 0f), true, "Abyss").nodePosition;
                break;
            case "Cellar":
                dungeonEnemyLeftBottomPoint = astar.GetNodeByPosition(astar.cellarDungeonMainNodeTransform.position - new Vector3(astar.dungeonXSize * 0.5f, 0f, 0f), true, "Cellar").nodePosition;
                break;
            default:
                dungeonEnemyLeftBottomPoint = Vector3.zero;
                break;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemy = GetEnemy();
            enemy.name = i.ToString();

            int enemyNumber = 0;

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            Vector3 spawnPoint = Vector3.zero;
            int dungeonNumber = 0;

            enemyController.setEnemyNodeArrayOneIntoZero += SetEnemyNodeArrayOneIntoZero;

            int xPos;
            int yPos;

            do
            {
                xPos = Random.Range(0, 4);
                yPos = Random.Range(1, 5);

            } while (enemyNodeArray[xPos, yPos] == 1);

            enemyController.xPos = xPos;
            enemyController.yPos = yPos;

            enemyNodeArray[xPos, yPos] = 1;

            switch (dungeonName)
            {
                case "Wood":
                    spawnPoint = dungeonEnemyLeftBottomPoint + new Vector3(xPos * 2f, 0f, yPos * 2f);

                    enemyNumber = int.Parse(GameData.Instance.woodDungeonEnemy[Random.Range(0, GameData.Instance.woodDungeonEnemy.Count - 1)].Split('_')[1]);

                    dungeonNumber = (int)Dungeons.Wood;
                    break;
                case "Abyss":
                    spawnPoint = dungeonEnemyLeftBottomPoint + new Vector3(xPos * 2f, 0f, yPos * 2f);

                    enemyNumber = int.Parse(GameData.Instance.abyssDungeonEnemy[Random.Range(0, GameData.Instance.abyssDungeonEnemy.Count - 1)].Split('_')[1]);
                    dungeonNumber = (int)Dungeons.Abyss;
                    break;
                case "Cellar":
                    spawnPoint = dungeonEnemyLeftBottomPoint + new Vector3(xPos * 2f, 0f, yPos * 2f);

                    enemyNumber = int.Parse(GameData.Instance.cellarDungeonEnemy[Random.Range(0, GameData.Instance.cellarDungeonEnemy.Count - 1)].Split('_')[1]);
                    dungeonNumber = (int)Dungeons.Cellar;
                    break;
            }

            Transform childTransform = enemy.GetChild(enemyNumber);

            enemyController.myNumber = enemyNumber;

            Animator childAnimator = childTransform.GetComponent<Animator>();

            enemyController.dropMoney = GameData.Instance.enemyDictionary[childTransform.name].dropMoney;
            enemyController.health = GameData.Instance.enemyDictionary[childTransform.name].health;
            enemyController.damage = GameData.Instance.enemyDictionary[childTransform.name].damage;

            enemyController.nowDungeonParentNumber = dungeonNumber;

            if (enemyController.attackEveryDelay == null)
            {
                enemyController.setNewTargetInDungeonRequestToActiveNpc += CallSetNewTargetInDungeon;
                enemyController.calculateEnemyCountInDungeon += CalculateEnemyCountInEachDungeon;
                enemyController.getNodeByPosition += astar.GetNodeByPosition;

                enemyController.attackEveryDelay += CallAttackEveryDelay;
                enemyController.removeEnemyFromDungeonEnemyList += RemoveEnemyFromDungeonEnemyList;

                enemyController.addMoney += AddMoney;
            }

            enemy.gameObject.SetActive(true);
            childTransform.gameObject.SetActive(true);
            enemy.position = spawnPoint;

            dungeonController.reward += GameData.Instance.enemyDictionary[childTransform.name].dropMoney / 3;

            if (!enemyControllerDictionary.ContainsKey(i.ToString()))
            {
                enemyControllerDictionary.Add(i.ToString(), enemyController);
            }

            dungeonEnemyTransformList.Add(enemy);

            //Debug.Log(childAnimator.parameterCount);

            //while (childAnimator.parameterCount == 0)
            //{
            //    Debug.Log("wait for add parameter");

            //    if (childAnimator.parameterCount != 0)
            //    {

            //        break;
            //    }

            //    yield return new WaitForSeconds(1f);
            //}

            childAnimator.SetInteger("Health", GameData.Instance.enemyDictionary[childTransform.name].health);
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

    //enemy랑 npc 공용

    //target으로 가기

    private void GoToTargetInDungeon(Transform npc, Transform target)
    {
        Stack<Vector3> path = pathFinding.PathFind(npc.position, target.position, true, nowDungeonTransform.name.Split('_')[1]);

        StartCoroutine(GoToTargetInDungeonCoroutine(path, npc, target));
    }

    IEnumerator GoToTargetInDungeonCoroutine(Stack<Vector3> path, Transform npc, Transform target)
    {
        float beforeAnimationSpeed;
        Animator animator = npc.GetComponent<Animator>();
        beforeAnimationSpeed = animator.speed;

        if (!npcControllerDictionary[npc.name].npcTransform.gameObject.activeSelf)
        {
            npcControllerDictionary[npc.name].npcTransform.gameObject.SetActive(true);
        }

        if (path != null)
        {
            enemyControllerDictionary[target.name].targetInDungeon.Add(npc);

            int count = path.Count;

            animator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                Vector3 nextPos = path.Pop();

                npc.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npc.position) >= 0.1f)
                {
                    if (pause)
                    {
                        animator.speed = 0f;

                        while (true)
                        {
                            if (!pause)
                            {
                                animator.speed = beforeAnimationSpeed;

                                break;
                            }

                            yield return new WaitForSeconds(1f);
                        }
                    }

                    npc.Translate(Vector3.forward * GameData.Instance.gameSpeed * Time.deltaTime);

                    if (!target.gameObject.activeSelf)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(0.02f / GameData.Instance.gameSpeed);
                }
            }

            animator.SetFloat("Speed", 0f);

            if (npcControllerDictionary[npc.name].npcIsDead)
            {
                npcControllerDictionary[npc.name].npcAnimator.SetBool("NpcIsDead", false);
            }

            CallAttackEveryDelay(npc, npcControllerDictionary[npc.name].targetInDungeon, true);
            CallAttackEveryDelay(target, enemyControllerDictionary[target.name].targetInDungeon[0], false);
        }
    }

    public void CallAttackEveryDelay(Transform mySelf, Transform target, bool isNpc)
    {
        StartCoroutine(AttackEveryDelay(mySelf, target, isNpc));
    }

    IEnumerator AttackEveryDelay(Transform mySelf, Transform target, bool isNpc)
    {
        Animator animator = new Animator();

        NpcController npcController = null;
        EnemyController enemyController = null;

        if (isNpc)
        {
            npcController = npcControllerDictionary[mySelf.name];
            animator = mySelf.GetComponent<Animator>();
        }
        else
        {
            enemyController = enemyControllerDictionary[mySelf.name];

            animator = mySelf.GetChild(enemyController.myNumber).GetComponent<Animator>();
        }

        while (target.gameObject.activeSelf)
        {
            if (!isNpc)
            {
                yield return new WaitForSeconds(3f);

            }

            // 1.타겟으로 감(타겟이랑 거리 체크해서 가까우면 공격)
            // 2.타겟이 죽었는지 확인(target gameObject active false)
            // 3.타겟이 죽었다면 SetNewEnemyTarget

            animator.SetTrigger("Attack");

            if (isNpc)
            {
                if (npcController.npcIsDead)
                {
                    enemyControllerDictionary[target.name].removeEnemyFromDungeonEnemyList(npcController.npcTransform.parent);
                    break;
                }

                enemyControllerDictionary[target.name].health -= npcController.damage + GameData.Instance.weaponDataDictionary[npcController.weaponNumber].damage;

                if (enemyControllerDictionary[target.name].health <= 0)
                {
                    enemyControllerDictionary[target.name].Die();
                }
            }
            else
            {
                npcControllerDictionary[target.name].health -= enemyController.damage -
                    Mathf.CeilToInt(1 * Mathf.FloorToInt(0.5f * GameData.Instance.armorDataDictionary[npcControllerDictionary[target.name].shieldNumber].armorValue));

                if (npcControllerDictionary[target.name].health <= 0)
                {
                    npcControllerDictionary[target.name].Die();
                }
            }

            yield return new WaitForSeconds(10f);
        }
    }

    public void RemoveEnemyFromDungeonEnemyList(Transform enemy)
    {
        dungeonEnemyTransformList.Remove(enemy);
    }

    public void CallSetNewTargetInDungeon(Transform npc)
    {
        StartCoroutine(SetNewTargetInDungeon(npc));
    }

    //던전에 있는 enemy들이랑 npc들 가지고 있고 타겟이 죽으면 거리 계산해서 가장 가까운 npc/enemy를 타겟으로 정해줌
    IEnumerator SetNewTargetInDungeon(Transform npc)
    {
        string[] names = nowDungeonTransform.parent.name.Split('_');

        if (dungeonDictionary[int.Parse(names[0])].nowActiveTrueEnemyCount != 0)
        {
            Transform newTarget = null;
            float distance = 999f;

            foreach (var enemy in dungeonEnemyTransformList)
            {
                float newDistance = Vector3.Distance(enemy.position, npc.position);

                if (distance > newDistance)
                {
                    newTarget = enemy;
                    distance = newDistance;
                }
            }

            npcControllerDictionary[npc.name].targetInDungeon = newTarget;

            GoToTargetInDungeon(npc, newTarget);
        }

        yield return null;
    }

    //NPC

    IEnumerator NpcPooling()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            Transform npc = Instantiate(npcPrefab, npcParent);
            NpcController npcController = npc.GetComponent<NpcController>();

            Animator animator = npc.GetComponent<Animator>();

            npcController.npcAnimator = animator;

            npc.position = npcStartTransform.position;
            npc.name = GameData.Instance.npcNameList[i];

            npcController.npcAnimator.SetInteger("Health", GameData.Instance.npcDataDictionary[npc.name].maxHealth);

            npcController.firstEntrance = true;

            npcController.npcTransform = npc.GetChild(Random.Range(0, npc.childCount - 4));
            npcController.npcTransform.gameObject.SetActive(false);

            Transform arm = npc.GetChild(npc.childCount - 4).GetChild(0).GetChild(0).GetChild(0).GetChild(0);

            npcController.weaponTransformForBuyAnimation = npc.GetChild(npc.childCount - 3);
            npcController.armorTransformForBuyAnimation = npc.GetChild(npc.childCount - 2);
            npcController.healthTransformForBuyAnimation = npc.GetChild(npc.childCount - 1);

            npcController.weaponParent = arm.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1);
            npcController.shieldParent = arm.GetChild(0).GetChild(0).GetChild(0).GetChild(1);

            npcController.maskParent = arm.GetChild(2).GetChild(0).GetChild(0).GetChild(0);

            npcController.weaponParent.gameObject.SetActive(false);

            npcPool.Add(npc);

            npcControllerDictionary.Add(npc.name, npcController);
            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);

            npcController.setTargetAtTargetBuildingActiveSelfFalse += SetTargetDelegate;
            npcController.attackEveryDelay += CallAttackEveryDelay;
            npcController.getNodeByPosition += astar.GetNodeByPosition;
            npcController.npcMoveToDungeonEntrance += NpcMoveToDungeonEntrance;

            npcController.maxHealth = GameData.Instance.npcDataDictionary[npc.name].maxHealth;
            npcController.health = GameData.Instance.npcDataDictionary[npc.name].maxHealth;
            npcController.damage = GameData.Instance.npcDataDictionary[npc.name].damage;
            npcController.armor = GameData.Instance.npcDataDictionary[npc.name].armor;

            npcController.weaponNumber = GameData.Instance.npcDataDictionary[npc.name].weaponNumber;

            npcController.weaponParent.GetChild(npcController.weaponNumber).gameObject.SetActive(true);

            StartCoroutine(SetTarget(npc));
        }

        yield return null;
    }

    [SerializeField] private Transform nowDungeonTransform;

    private void GetTarget(NpcController npcController)
    {
        if (npcController.npcGoToDungeon)
        {
            npcController.targetTransform = nowDungeonTransform;
            npcController.target = nowDungeonTransform.position;
            npcController.endOfSetTarget = true;
        }
        else
        {
            astar.GetRandomNodeByLayer(npcController, (int)GameLayer.Building, BuildingType.Shop.ToString(), false);
        }
    }

    private void SetTargetDelegate(Transform npcTransform)
    {
        StartCoroutine(SetTarget(npcTransform));
    }

    private IEnumerator SetTarget(Transform npcTransform)
    {
        NpcController npcController = npcControllerDictionary[npcTransform.name];

        GetTarget(npcController);

        StartCoroutine(NpcGoToTarget(npcTransform));
        
        yield return null;
    }

    IEnumerator NpcGoToTarget(Transform npcTransform)
    {
        NpcController npcController = npcControllerDictionary[npcTransform.name];

        npcController.targetTransform = astar.GetNodeByPosition(npcController.target, false, null).nodeTransform;

        StartCoroutine(Go(npcTransform, npcController.targetTransform, npcController));
        
        yield return null;
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

        Stack<Vector3> path = pathFinding.PathFind(npcTransform.position, npcController.target, false, null);

        float beforeAnimationSpeed = npcController.npcAnimator.speed;

        bool targetIsActive = true;

        if (path != null)
        {
            int count = path.Count;

            npcController.weaponParent.gameObject.SetActive(true);
            npcController.npcTransform.gameObject.SetActive(true);

            npcController.npcAnimator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                if (i == 1 && npcController.playAnimation)
                {
                    buildingManager.CallBuyAnimation(npcController);

                    npcController.npcAnimator.SetFloat("Speed", 0f);

                    while (true)
                    {
                        if (!npcController.playAnimation)
                        {
                            break;
                        }

                        yield return new WaitForSeconds(1f);
                    }

                    npcController.npcAnimator.SetFloat("Speed", 1f);
                }

                Vector3 nextPos = path.Pop();
                npcTransform.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
                {
                    if (pause)
                    {
                            npcController.npcAnimator.speed = 0f;

                        while (true)
                        {
                            if (!pause)
                            {
                                npcController.npcAnimator.speed = beforeAnimationSpeed;
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

            if (npcController.npcGoToDungeon
                && Vector3.Distance(npcTransform.position, dungeonTransforms[int.Parse(nowDungeonTransform.parent.name.Split('_')[0])].position) <= 0.1f)
            {
                npcController.arrivedDungeon = true;
            }

            if (targetIsActive)
            {
                npcController.npcTransform.gameObject.SetActive(false);

                if (npcController.target == astar.npcStartPosTransformForReturnRandomNode.position)
                {
                    npcTransform.position = astar.npcStartPosTransformForReturnRandomNode.position;
                }
            }

            npcController.npcAnimator.SetFloat("Speed", 0f);

            if (npcController.arrivedDungeon)
            {
                npcController.arrivedDungeon = false;
                NpcMoveToNowActiveDungeon(npcTransform);
            }
            else
            {
                npcController.npcTransform.gameObject.SetActive(false);

                switch (astar.GetNodeByPosition(npcController.target, false, null).buildingName)
                {
                    case "WeaponShop":
                        buildingManager.BuildingInteraction(npcController, "weapon");
                        break;
                    case "ArmorShop":
                        buildingManager.BuildingInteraction(npcController, "armor");
                        break;
                    case "Hotel":
                        buildingManager.BuildingInteraction(npcController, "health");
                        break;
                }

                StartCoroutine(SetTarget(npcTransform));
            }
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

        StartCoroutine(SetNewTargetInDungeon(npcTransform));

        if (npcControllerDictionary[npcTransform.name].npcIsDead)
        {
            npcControllerDictionary[npcTransform.name].npcIsDead = false;
        }

        npcControllerDictionary[npcTransform.name].weaponParent.gameObject.SetActive(true);
        npcTransform.gameObject.SetActive(true);
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
                    Node node = astar.GetNodeByPosition(dungeon.position, false, null);

                    node.layerNumber = (int)GameLayer.Dungeon;
                    node.nodePosition = dungeon.position;
                    node.nodeTransform = dungeonChild;
                    node.isWalkable = true;
                    node.buildingType = BuildingType.Dungeon.ToString();

                    dungeonChild.gameObject.SetActive(true);

                    DungeonController dungeonController = dungeon.GetComponent<DungeonController>();

                    dungeonController.nowActiveDungeonNumber = int.Parse(names[1]);


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
        DungeonController dungeonController = dungeonDictionary[dungeonParentNumber];
        dungeonController.nowActiveTrueEnemyCount += value;

        if (dungeonController.nowActiveTrueEnemyCount == 0)
        {
            DungeonActiveFalse(dungeonController.nowActiveDungeonNumber);
        }
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
        
        switch (int.Parse(names[1]))
        {
            case 0:
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Wood.ToString());
                dungeonName = "Wood";

                dungeonCameraParent.position = woodEnemySpawnPoint.position;
                break;
            case 1:
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Abyss.ToString());
                dungeonName = "Abyss";

                dungeonCameraParent.position = abyssEnemySpawnPoint.position;
                break;
            case 2:
                dungeonCameraController.SetCameraLimitMethod(Dungeons.Cellar.ToString());
                dungeonName = "Cellar";

                dungeonCameraParent.position = cellarEnemySpawnPoint.position;
                break;
        }

        StartCoroutine(DungeonEnterNpcCheck(dungeonName));

        yield return null;
    }

    public void SwitchMainCameraAndDungeonCamera(string thisCameraName)
    {
        switch (thisCameraName)
        {
            case "main":
                if (!mainTapAlphaImage.gameObject.activeSelf)
                {
                    mainTapAlphaImage.gameObject.SetActive(true);
                    mainCamera.gameObject.SetActive(true);

                    dungeonCamera.gameObject.SetActive(false);

                    if (isDungeonEntrance)
                    {
                        dungeonTapAlphaImage.gameObject.SetActive(false);
                    }
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

    public void DungeonEnterCancel()
    {
        dungeonEnterCancel = true;
    }

    private bool GetNpcListIsActive()
    {
        return npcEnterListTransform.parent.parent.gameObject.activeSelf;
    }

    IEnumerator DungeonEnterNpcCheck(string dungeonName)
    {
        PauseGame(true);
        npcEnterListTransform.parent.parent.gameObject.SetActive(true);
        buildingManager.GetBuildButtonTransform().gameObject.SetActive(false);

        dungeonEnterCancelButton.gameObject.SetActive(true);

        while (true)
        {
            if (enterTheDungeon)
            {
                break;
            }

            if (dungeonEnterCancel)
            {
                PauseGame(false);

                foreach (var dungeonEnterMessageSelectedNpc in dungeonEntranceNpcList)
                {
                    npcEntranceSelectedCheckImage[dungeonEnterMessageSelectedNpc.name].gameObject.SetActive(false);
                }

                dungeonEntranceNpcList.Clear();
                currentEntranceNpcCount = 0;

                dungeonEnterCancel = false;
                isDungeonEntrance = false;

                npcEnterListTransform.parent.parent.gameObject.SetActive(false);

                dungeonEnterCancelButton.gameObject.SetActive(false);

                StopCoroutine(DungeonEnterNpcCheck(dungeonName));
            }

            yield return new WaitForSeconds(1f);
        }

        buildingManager.GetBuildButtonTransform().gameObject.SetActive(true);

        dungeonTapAlphaImage.gameObject.SetActive(false);

        PauseGame(false);

        dungeonEnterButtonAlphaImageTransform.gameObject.SetActive(true);
        npcEnterListTransform.parent.parent.gameObject.SetActive(false);

        //List에 있는거 던전으로 target정하고 던전 안에있는 enemy를 enemyTarget으로 정하고
        //던전 내부에 enemy는 처음 들어온애를 target으로함 

        foreach (var npcTransform in dungeonEntranceNpcList)
        {
            npcControllerDictionary[npcTransform.name].npcGoToDungeon = true;
        }

        StartCoroutine(SpawnEnemy(Random.Range(3, 7), dungeonName));
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
        maxHealthInEntranceButton.text = GameData.Instance.npcDataDictionary[npcName].maxHealth.ToString();
        damageInEntranceButton.text = GameData.Instance.npcDataDictionary[npcName].damage.ToString();
        armorInEntranceButton.text = GameData.Instance.npcDataDictionary[npcName].armor.ToString();
        fatigueInEntranceButton.text = GameData.Instance.npcDataDictionary[npcName].fatigue.ToString();

        button.onClick.AddListener(delegate { NpcAddToEntranceQueue(npcName); });
    }


    private void NpcAddToEntranceQueue(string npcName)
    {
        Transform npcTransform = null;

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

    private void DungeonActiveFalse(int dungeonNumberInDungeonEnterance)
    {
        foreach (var npc in dungeonEntranceNpcList)
        {
            if (!npcControllerDictionary[npc.name].npcIsDead)
            {
                NpcMoveToDungeonEntrance(npc);
            }

            npcControllerDictionary[npc.name].SetDungeonValueTurnOff();
            npcControllerDictionary[npc.name].npcTransform.gameObject.SetActive(false);
            npcControllerDictionary[npc.name].weaponParent.gameObject.SetActive(false);

            StartCoroutine(SetTarget(npc));
        }

        foreach (var dungeonEnteranceNpcButtonAlphaImage in npcEntranceSelectedCheckImage)
        {
            if (dungeonEnteranceNpcButtonAlphaImage.Value.gameObject.activeSelf)
            {
                dungeonEnteranceNpcButtonAlphaImage.Value.gameObject.SetActive(false);
            }
        }

        isDungeonEntrance = false;
        enterTheDungeon = false;
        currentEntranceNpcCount = 0;

        dungeonTapAlphaImage.gameObject.SetActive(true);

    }

    private void NpcMoveToDungeonEntrance(Transform npc)
    {
        npc.position = dungeonTransforms[dungeonBuildingNumber].position;

        astar.GetRandomNodeByLayer(npcControllerDictionary[npc.name], 0, null, true);
    }

    private void SetDungeonBuildingNumber(int value)
    {
        dungeonBuildingNumber = value;
    }

    //Dungeon
}
