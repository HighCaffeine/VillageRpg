using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Astar astar;

    [SerializeField] private Transform buildingPrefabParent;
    public Transform buildingTransform;
    public bool build = false;

    private List<Transform> buildingTransformList;

    public int buildingCount = 0;

    public int poolCount = 10;

    public int weaponBuildingCount;
    public int armorBuildingCount;
    public int hotelBuildingCount;

    private void Awake()
    {
        buildingTransformList = new List<Transform>();
        buildingTransform = null;
        astar = GetComponent<Astar>();

        //for (int i = 0; i < poolCount; i++)
        //{
        //    CreateBuilding();
        //}

        StartCoroutine("BuildingCountTransformPool", buildingPrefabParent.childCount);
    }

    private void OnEnable()
    {
        SettingBuilding();
    }

    private Coroutine setBuilding;

    public bool endOfSetBuilding = false;

    public int GetBuildingCountEachName(string buildingName)
    {
        int returnValue = 0;

        switch (buildingName)
        {
            case "WeaponShop":
                returnValue = weaponBuildingCount;
                break;
            case "ArmorShop":
                returnValue = armorBuildingCount;
                break;
            case "Hotel":
                returnValue = hotelBuildingCount;
                break;
        }

        return returnValue;
    }

    public void SettingBuilding()
    {
        foreach (var buildingData in GameData.Instance.buildingDataList)
        {
            int buildingNumber = 0;
            bool isDungeon = false;
            int layerNumber = 0;
            bool isWalkable = false;

            switch (buildingData.Value)
            {
                case "ArmorShop":
                    buildingNumber = (int)BuildingName.ArmorShop;
                    layerNumber = (int)GameLayer.Building;
                    isWalkable = false;
                    break;
                case "WeaponShop":
                    buildingNumber = (int)BuildingName.WeaponShop;
                    layerNumber = (int)GameLayer.Building;
                    isWalkable = false;
                    break;
                case "Hotel":
                    buildingNumber = (int)BuildingName.Hotel;
                    layerNumber = (int)GameLayer.Building;
                    isWalkable = false;
                    break;
                case "Platform":
                    buildingNumber = (int)BuildingName.Platform;
                    layerNumber = (int)GameLayer.Road;
                    isWalkable = true;
                    break;
                case "Tree":
                    buildingNumber = (int)BuildingName.Tree;
                    //layerNumber = (int)GameLayer.Building;
                    isWalkable = false;
                    break;
                case "Wood":
                    buildingNumber = (int)Dungeons.Wood;
                    isDungeon = true;
                    break;
                case "Abyss":
                    buildingNumber = (int)Dungeons.Abyss;
                    isDungeon = true;
                    break;
                case "Cellar":
                    buildingNumber = (int)Dungeons.Cellar;
                    isDungeon = true;
                    break;
                default:
                    break;
            }

            string[] pos = buildingData.Key.Split('_');

            Vector3 position = astar.leftPosition + new Vector3(int.Parse(pos[0]) * 2f, 0f, int.Parse(pos[1]) * 2f);

            Node buildingNode = astar.GetNodeByPosition(position, false, null);

            if (isDungeon)
            {
                buildingNode.nodeTransform.GetChild(buildingNumber).gameObject.SetActive(true);
            }
            else
            {
                Transform buildingTransform = Instantiate(buildingPrefabParent, buildingPoolTransform);
                buildingTransform.name = buildingNumber.ToString();

                buildingTransform.position = buildingNode.nodePosition;

                Transform buildingChildTransform = buildingTransform.GetChild(buildingNumber);
                string[] names = buildingChildTransform.name.Split('_');

                buildingChildTransform.gameObject.SetActive(true);

                buildingNode.buildingType = names[0];
                buildingNode.buildingName = names[1];
                buildingNode.isWalkable = isWalkable;
                buildingNode.layerNumber = layerNumber;

                buildingNode.nodeTransform = buildingTransform;

                switch (names[1])
                {
                    case "WeaponShop":
                        weaponBuildingCount++;
                        break;
                    case "ArmorShop":
                        armorBuildingCount++;
                        break;
                    case "Hotel":
                        hotelBuildingCount++;
                        break;
                }

                buildingCount++;
            }
        }

        endOfSetBuilding = true;
    }

    public void CallSetBuilding()
    {
        if (!cameraController.cameraMove && !nowBuilding)
        {
            if (!buildingWindow.gameObject.activeSelf)
            {
                buildCancelButton.gameObject.SetActive(true);
                buildingWindow.gameObject.SetActive(true);
            }

            setBuilding = StartCoroutine(SetBuilding());
        }
    }

    [SerializeField] private float buildingDelay = 0.5f;
    private bool nowBuilding = false;
    public bool nowSelectingBuilding = false;

    public delegate void PauseGame(bool pauseOrNot);
    public PauseGame pauseGame;

    [SerializeField] private Transform buildCancelButton;

    [SerializeField] private Transform buildButton;

    public Transform GetBuildButtonTransform()
    {
        return buildButton;
    }

    public bool GetBuildingWindowActiveSelf()
    {
        return buildingWindow.gameObject.activeSelf;
    }

    public void SetBuildingValue(bool value)
    {
        build = value;
        demolition = value;
    }

    public bool GetNowBuilding()
    {
        return nowBuilding;
    }

    public bool GetNowSelectingBuilding()
    {
        return nowSelectingBuilding;
    }

    //건물 만드는거
    IEnumerator SetBuilding()
    {
        pauseGame(true);
        saveButtonSetActive(false);

        nowBuilding = true;

        nowSelectingBuilding = true;

        Vector3 position;
        Node buildingNode;

        while (true)
        {
            if (buildingTransform != null)
            {
                nowSelectingBuilding = false;
                break;
            }

            yield return new WaitForSeconds(buildingDelay);
        }

        build = false;

        while (true)
        {
            position = cameraController.cameraParent.position;

            buildingNode = astar.GetNodeByPosition(position, false, null);

            buildingTransform.position = buildingNode.nodePosition;

            if (buildingNode.layerNumber == 0)
            {
                if (build)
                {
                    break;
                }
            }
            else
            {
                build = false;
            }

            yield return new WaitForSeconds(buildingDelay);
        }

        Transform childTransforn = buildingTransform.GetChild(int.Parse(buildingTransform.name));
        string[] buildingNames = childTransforn.name.Split('_');

        buildingNode.buildingType = buildingNames[0];
        buildingNode.buildingName = buildingNames[1];
        buildingNode.layerNumber = childTransforn.gameObject.layer;
        buildingNode.nodeTransform = buildingTransform;

        if (buildingNode.layerNumber == (int)GameLayer.Road)
        {
            buildingNode.isWalkable = true;
        }
        else
        {
            buildingNode.isWalkable = false;
        }

        switch (buildingNode.buildingName)
        {
            case "WeaponShop":
                weaponBuildingCount++;
                break;
            case "ArmorShop":
                armorBuildingCount++;
                break;
            case "Hotel":
                hotelBuildingCount++;
                break;
        }

        this.buildingTransform = null;
        build = false;
        nowBuilding = false;

        buildingCount++;

        pauseGame(false);
        saveButtonSetActive(true);

        buildCancelButton.gameObject.SetActive(false);

        string key = buildingNode.xPosition + "_" + buildingNode.yPosition;

        GameData.Instance.buildingDataList.Add(key, buildingNames[1]);
        addMoney(-int.Parse(buildingNames[buildingNames.Length - 1]));

        yield return null;
    }

    public void CancelBuilding()
    {
        StopCoroutine(setBuilding);

        saveButtonSetActive(true);
        pauseGame(false);
        nowBuilding = false;
        build = false;
        nowSelectingBuilding = false;

        buildingWindow.gameObject.SetActive(false);
        buildCancelButton.gameObject.SetActive(false);


        if (buildingTransform.gameObject.activeSelf)
        {
            buildingTransform.GetChild(int.Parse(buildingTransform.name)).gameObject.SetActive(false);
            buildingTransform.gameObject.SetActive(false);

            buildingTransform = null;
        }
    }

    private int rotateCount = 0;

    public void ChangeRotationValue()
    {
        if (buildingTransform != null)
        {
            if (rotateCount == 0)
            {
                buildingTransform.Rotate(new Vector3(0f, -90f, 0f));
                rotateCount++;
            }
            else
            {
                buildingTransform.Rotate(new Vector3(0f, 90f, 0f));
                rotateCount = 0;
            }
        }
    }

    // UI에서 건물 고르는거 띄우고 고른 건물을 buildingTransform에 넣어줌
    public void ChooseBuildingToBuild(int value)
    {
        buildingTransform = GetBuildingTransform();

        Transform childTransform = buildingTransform.GetChild(value);
        string[] names = childTransform.name.Split('_');

        if (int.Parse(names[names.Length - 1]) <= getNowMoney())
        {
            childTransform.gameObject.SetActive(true);

            buildingTransform.name = value.ToString();
            buildingTransform.gameObject.SetActive(true);

            buildingWindow.gameObject.SetActive(false);
        }
    }
    private Transform GetBuildingTransform()
    {
        foreach (var buildingTransform in buildingTransformList)
        {
            if (buildingTransform.gameObject.activeSelf == false)
            {
                return buildingTransform;
            }
        }

        Transform newBuildingTransform = CreateBuilding();

        return newBuildingTransform;
    }

    public bool demolition = false;

    //건물 부수기
    public void CallDemolition()
    {
        buildingWindow.gameObject.SetActive(false);

        StopCoroutine(setBuilding);
        StartCoroutine(BuildingDemolition());
    }

    private IEnumerator BuildingDemolition()
    {
        if (buildingCount != 0)
        {
            nowSelectingBuilding = false;
            Node buildingNode;
        
            while (true)
            {
                if (this.demolition)
                {
                    break;
                }

                yield return new WaitForSeconds(buildingDelay);
            }

            buildingNode = astar.GetNodeByPosition(cameraController.cameraParent.position, false, null);

            if (buildingNode.layerNumber != 0)
            {
                buildingNode.nodeTransform.GetChild(int.Parse(buildingNode.nodeTransform.name)).gameObject.SetActive(false);
                buildingNode.nodeTransform.gameObject.SetActive(false);

                buildingNode.buildingType = null;
                buildingNode.buildingName = null;
                buildingNode.layerNumber = 0;
                buildingNode.nodeTransform = null;

                switch (buildingNode.buildingName)
                {
                    case "WeaponShop":
                        weaponBuildingCount--;
                        break;
                    case "ArmorShop":
                        armorBuildingCount--;
                        break;
                    case "Hotel":
                        hotelBuildingCount--;
                        break;
                }

                demolition = false;
                build = false;
                nowBuilding = false;
            
                buildingCount--;
            }

            string key = buildingNode.xPosition + "_" + buildingNode.yPosition;
            GameData.Instance.buildingDataList.Remove(key);

            saveButtonSetActive(true);
            pauseGame(false);
            addMoney(-50);
        }


        yield return null;
    }

    [SerializeField] private Transform buildingWindow;
    [SerializeField] private Transform buildingCountPrefab;
    private Transform buildingCountButtonParent; // 얘는 building 이름 가져와서 parent 받아오는용

    //버튼 parent
    [SerializeField] private Transform environmentCountButtonParent;
    [SerializeField] private Transform shopCountButtonParent;

    [SerializeField] private int nextBuildingButtonCount = -1;


    [SerializeField] private Transform buildingPoolTransform;


    private Transform CreateBuilding()
    {
        Transform buildingTransform = Instantiate(buildingPrefabParent, buildingPoolTransform);
        buildingTransformList.Add(buildingTransform);
        buildingTransform.gameObject.SetActive(false);

        return buildingTransform;
    }
    IEnumerator BuildingCountTransformPool(int poolCount)
    {
        for (int i = 0; i < poolCount; i++)
        {
            BuildingCountObjectMake();
        }

        buildingWindow.gameObject.SetActive(false);
        shopCountButtonParent.gameObject.SetActive(false);

        yield return null;
    }

    public void BuildingCountObjectMake()
    {
        //버튼 만들어줌     이것도 풀링 해줘야할듯
        //새로운 건물이 있다면 넣어줌
        if (nextBuildingButtonCount >= 0)
        {
            string[] name = buildingPrefabParent.GetChild(nextBuildingButtonCount).name.Split('_');

            if (name[2] == BuildingAllow.BuildingAllowded.ToString())
            {
                Transform buttonTransform = Instantiate(buildingCountPrefab);
                Button button = buttonTransform.GetComponent<Button>();
                Text childForSetText = buttonTransform.GetChild(0).GetComponent<Text>();

                //리스너에 넣어줄 새로운 인트값 만들어줌
                int nextbuildingCountNewInt = new int();
                nextbuildingCountNewInt = nextBuildingButtonCount;

                //텍스트 설정
                childForSetText.text = name[1] + ", " + name[name.Length - 1];

                //부모 설정
                switch (name[0])
                {
                    case "Environment":
                        buildingCountButtonParent = environmentCountButtonParent;
                        break;
                    case "Shop":
                        buildingCountButtonParent = shopCountButtonParent;
                        break;
                }

                //리스너 할당
                button.onClick.AddListener(delegate { ChooseBuildingToBuild(nextbuildingCountNewInt); });

                buttonTransform.SetParent(buildingCountButtonParent);
            }
        }
        else if (nextBuildingButtonCount == -1) //파괴용 버튼
        {
            Transform buttonTransform = Instantiate(buildingCountPrefab);
            Button button = buttonTransform.GetComponent<Button>();
            Text childForSetText = buttonTransform.GetChild(0).GetComponent<Text>();

            childForSetText.text = "Demolition, 50";
            buildingCountButtonParent = environmentCountButtonParent;
            button.onClick.AddListener(delegate { CallDemolition(); });
            buttonTransform.SetParent(buildingCountButtonParent);
        }

        //다음거
        nextBuildingButtonCount++;
    }

    [SerializeField] private Transform environmentAlphaImage;
    [SerializeField] private Transform shopAlphaImage;

    public void SwapBuildingType(string type)
    {
        switch (type)
        {
            case "Environment":
                if (!environmentCountButtonParent.gameObject.activeSelf)
                {
                    shopCountButtonParent.gameObject.SetActive(false);
                    environmentCountButtonParent.gameObject.SetActive(true);
                }

                if (!environmentAlphaImage.gameObject.activeSelf)
                {
                    shopAlphaImage.gameObject.SetActive(false);
                    environmentAlphaImage.gameObject.SetActive(true);
                }

                break;
            case "Shop":
                if (!shopCountButtonParent.gameObject.activeSelf)
                {
                    environmentCountButtonParent.gameObject.SetActive(false);
                    shopCountButtonParent.gameObject.SetActive(true);
                }

                if (!shopAlphaImage.gameObject.activeSelf)
                {
                    environmentAlphaImage.gameObject.SetActive(false);
                    shopAlphaImage.gameObject.SetActive(true);
                }

                break;
        }
    }

    //range = 0~10000   if range == 100 -> 1.00%
    private bool GetPercent(int range)
    {
        int percentValue = Random.Range(0, 10001);

        if (percentValue <= range)
        {
            return true;
        }

        return false;
    }

    //itemType : weapon, armor, health
    private void BuyItem(NpcController npcController, string itemType, bool sameItem)
    {
        int price = 0;

        switch (itemType)
        {
            case "weapon": // 1번 10
                if (!sameItem)
                {
                    npcController.weaponParent.GetChild(npcController.weaponNumber).gameObject.SetActive(false);

                    npcController.weaponNumber++;
                    npcController.weaponParent.GetChild(npcController.weaponNumber).gameObject.SetActive(true);

                    npcController.npcAnimator.SetFloat("WeaponNumber", npcController.weaponNumber);
                    GameData.Instance.npcDataDictionary[npcController.npcTransform.parent.name].weaponNumber = npcController.weaponNumber;
                }

                price = npcController.weaponNumber * 5 + 3;

                break;
            case "armor": // 1번 9
                if (!sameItem)
                {
                    if (npcController.shieldNumber != 0)
                    {
                        npcController.shieldParent.GetChild(npcController.shieldNumber).gameObject.SetActive(false);
                    
                    }
                    else
                    {
                        npcController.maskParent.GetChild(0).gameObject.SetActive(false);
                    }

                    npcController.shieldNumber++;
                    npcController.shieldParent.GetChild(npcController.shieldNumber).gameObject.SetActive(true);

                    GameData.Instance.npcDataDictionary[npcController.npcTransform.parent.name].armor = npcController.shieldNumber;
                }

                price = npcController.shieldNumber * 6 + 4;

                break;
            case "health": // 체력당 2원
                npcController.beforeHealHealth = npcController.health;

                if (npcController.health < npcController.maxHealth)
                {
                    int healthPoint = npcController.maxHealth - npcController.health;

                    npcController.health = npcController.maxHealth;
                    npcController.npcAnimator.SetInteger("Health", npcController.health);
                    price = healthPoint * 2 + 2;
                }

                if (npcController.npcIsDead)
                {
                    setTargetDelegate(npcController.npcTransform.parent);
                }

                break;
        }

        addMoney(price);
    }

    public void CallBuyAnimation(NpcController npcController)
    {
        StartCoroutine(BuyAnimation(npcController, npcController.itemBuyType));
    }

    //itemType : weapon, armor
    IEnumerator BuyAnimation(NpcController npcController, string itemType)
    {
        Transform transformForItemBuyAni = null;
        int childCount = 0;

        switch (itemType)
        {
            case "weapon":
                transformForItemBuyAni = npcController.weaponTransformForBuyAnimation;
                childCount = npcController.weaponNumber;
                break;
            case "armor":
                transformForItemBuyAni = npcController.armorTransformForBuyAnimation;
                childCount = npcController.shieldNumber;
                break;
            case "health":
                transformForItemBuyAni = npcController.healthTransformForBuyAnimation;
                break;
        }

        if (itemType != "health")
        {
            float yPos = transformForItemBuyAni.position.y;
            float targetYPos = yPos + 3f;

            Transform itemTransform = transformForItemBuyAni.GetChild(childCount);

            itemTransform.gameObject.SetActive(true);

            while (true)
            {
                transformForItemBuyAni.position = Vector3.Lerp(transformForItemBuyAni.position, new Vector3(transformForItemBuyAni.position.x, targetYPos, transformForItemBuyAni.position.z), Time.deltaTime);

                if (Mathf.Abs(transformForItemBuyAni.position.y - targetYPos) <= 0.5f)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            itemTransform.gameObject.SetActive(false);

            transformForItemBuyAni.position = new Vector3(transformForItemBuyAni.position.x, yPos, transformForItemBuyAni.position.z);
            npcController.playAnimation = false;
        }
        else
        {
            float firstValue = npcController.beforeHealHealth / npcController.maxHealth;

            transformForItemBuyAni.localScale = new Vector3(firstValue, transformForItemBuyAni.localScale.y, transformForItemBuyAni.localScale.z);
            transformForItemBuyAni.gameObject.SetActive(true);

            while (transformForItemBuyAni.localScale.x <= 1f)
            {
                transformForItemBuyAni.localScale = new Vector3(transformForItemBuyAni.localScale.x + 0.15f, transformForItemBuyAni.localScale.y, transformForItemBuyAni.localScale.z);

                yield return new WaitForSeconds(0.2f);
            }

            transformForItemBuyAni.localScale = new Vector3(1f, transformForItemBuyAni.localScale.y, transformForItemBuyAni.localScale.z);
            npcController.playAnimation = false;

            transformForItemBuyAni.gameObject.SetActive(false);
        }
    }

    //buildingType : armor, weapon, hotel
    public void BuildingInteraction(NpcController npcController, string buildingType)
    {
        switch (buildingType)
        {
            case "weapon":
                npcController.itemBuyType = "weapon";


                if (GetPercent(100))
                {
                    if (npcController.weaponNumber != GameData.Instance.weaponDataDictionary.Count - 1)
                    {
                        BuyItem(npcController, "weapon", false);
                    }
                    else
                    {
                        BuyItem(npcController, "weapon", true);
                    }
                }
                else
                {
                    BuyItem(npcController, "weapon", true);
                }
                break;
            case "armor":
                npcController.itemBuyType = "armor";

                if (GetPercent(100))
                {
                    if (npcController.shieldNumber != GameData.Instance.armorDataDictionary.Count - 1)
                    {
                        BuyItem(npcController, "armor", false);
                    }
                    else
                    {
                        BuyItem(npcController, "armor", true);
                    }
                }
                else
                {
                    BuyItem(npcController, "armor", true);
                }
                break;
            case "health":
                npcController.itemBuyType = "health";

                if (npcController.npcIsDead)
                {
                    npcController.npcIsDead = false;
                }

                BuyItem(npcController, "health", false);
                break;
        }

        npcController.playAnimation = true;
    }

    public delegate int GetNowMoney();
    public GetNowMoney getNowMoney;

    public delegate void SetTargetDelegate(Transform npcTransform);
    public SetTargetDelegate setTargetDelegate;

    public delegate void AddMoney(int value);
    public AddMoney addMoney;

    public delegate void SaveButtonSetActive(bool value);
    public SaveButtonSetActive saveButtonSetActive;
}
