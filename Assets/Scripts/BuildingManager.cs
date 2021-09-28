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

        for (int i = 0; i < poolCount; i++)
        {
            CreateBuilding();
        }

        StartCoroutine("BuildingCountTransformPool", buildingPrefabParent.childCount);
    }

    private Coroutine setBuilding;

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
    private bool nowSelectingBuilding = false;

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

        testNode = buildingNode;

        buildCancelButton.gameObject.SetActive(false);

        yield return null;
    }

    public Node testNode;

    public void CancelBuilding()
    {
        pauseGame(false);
        nowBuilding = false;
        build = false;

        buildingWindow.gameObject.SetActive(false);
        buildCancelButton.gameObject.SetActive(false);

        StopCoroutine(SetBuilding());
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

        Debug.Log(value);

        Transform childTransform = buildingTransform.GetChild(value);

        childTransform.gameObject.SetActive(true);

        buildingTransform.name = value.ToString();
        buildingTransform.gameObject.SetActive(true);

        buildingWindow.gameObject.SetActive(false);
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

            demolition = false;
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
                childForSetText.text = name[1];

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

            childForSetText.text = "Demolition";
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
    private void BuyItem(NpcController npcController, string itemType, int itemNumber, bool sameItem)
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
                }

                price = npcController.weaponNumber * 10;

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
                }

                price = npcController.shieldNumber * 9;

                break;
            case "health": // 체력당 2원
                if (npcController.health < npcController.maxHealth)
                {
                    int healthPoint = npcController.maxHealth - npcController.health;

                    npcController.health = npcController.maxHealth;
                    price = healthPoint * 2;
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
        Transform weaponOrArmorTransform = null;
        int childCount = 0;

        switch (itemType)
        {
            case "weapon":
                weaponOrArmorTransform = npcController.weaponTransformForBuyAnimation;
                childCount = npcController.weaponNumber;

                break;
            case "armor":
                weaponOrArmorTransform = npcController.armorTransformForBuyAnimation;
                childCount = npcController.shieldNumber;

                break;
        }

        float yPos = weaponOrArmorTransform.position.y;
        float targetYPos = yPos + 3f;


        Debug.Log($"{npcController.npcTransform.parent.name}, {itemType}, {childCount}");
        Transform itemTransform = weaponOrArmorTransform.GetChild(childCount);

        itemTransform.gameObject.SetActive(true);

        while (true)
        {
            weaponOrArmorTransform.position = Vector3.Lerp(weaponOrArmorTransform.position, new Vector3(weaponOrArmorTransform.position.x, targetYPos, weaponOrArmorTransform.position.z), Time.deltaTime);

            if (Mathf.Abs(weaponOrArmorTransform.position.y - targetYPos) <= 0.5f)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        itemTransform.gameObject.SetActive(false);

        weaponOrArmorTransform.position = new Vector3(weaponOrArmorTransform.position.x, yPos, weaponOrArmorTransform.position.z);
        npcController.playAnimation = false;
    }

    //buildingType : armor, weapon, hotel 12    q
    public void BuildingInteraction(NpcController npcController, string buildingType)
    {
        switch (buildingType)
        {
            case "weapon":
                npcController.itemBuyType = "weapon";


                if (GetPercent(10000))
                {
                    if (npcController.weaponNumber != GameData.Instance.weaponDataDictionary.Count - 1)
                    {
                        BuyItem(npcController, "weapon", npcController.weaponNumber, false);
                    }
                    else
                    {
                        BuyItem(npcController, "weapon", npcController.weaponNumber, true);
                    }
                }
                else
                {
                    BuyItem(npcController, "weapon", npcController.weaponNumber, true);
                }
                break;
            case "armor":
                npcController.itemBuyType = "armor";

                if (GetPercent(10000))
                {
                    Debug.Log(GameData.Instance.armorDataDictionary.Count);

                    if (npcController.shieldNumber != GameData.Instance.armorDataDictionary.Count - 1)
                    {
                        BuyItem(npcController, "armor", npcController.shieldNumber, false);
                    }
                    else
                    {
                        BuyItem(npcController, "armor", npcController.shieldNumber, true);
                    }
                }
                else
                {
                    BuyItem(npcController, "armor", npcController.shieldNumber, true);
                }
                break;
            case "hotel":
                BuyItem(npcController, "health", 0, false);
                break;
        }

        npcController.playAnimation = true;
    }

    public delegate void AddMoney(int value);
    public AddMoney addMoney;
}
