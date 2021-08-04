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
                buildingWindow.gameObject.SetActive(true);
            }

            setBuilding = StartCoroutine(SetBuilding());
        }
    }

    [SerializeField] private float buildingDelay = 0.5f;
    private bool nowBuilding = false;

    public delegate void PauseGame(bool pauseOrNot);
    public PauseGame pauseGame;

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

    //건물 만드는거
    IEnumerator SetBuilding()
    {
        pauseGame(true);

        nowBuilding = true;
        Vector3 position;
        Node buildingNode;

        while (buildingTransform == null)
        {
            if (buildingTransform != null)
            {
                break;
            }

            yield return new WaitForSeconds(buildingDelay);
        }

        build = false;

        while (true)
        {
            
            position = cameraController.cameraParent.position;

            buildingNode = astar.GetNodeByPosition(position);

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

        this.buildingTransform = null;
        build = false;
        nowBuilding = false;

        buildingCount++;

        pauseGame(false);

        yield return null;
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

            buildingNode = astar.GetNodeByPosition(cameraController.cameraParent.position);

            if (buildingNode.layerNumber != 0)
            {
                buildingNode.nodeTransform.GetChild(int.Parse(buildingNode.nodeTransform.name)).gameObject.SetActive(false);
                buildingNode.nodeTransform.gameObject.SetActive(false);

                buildingNode.buildingType = null;
                buildingNode.buildingName = null;
                buildingNode.layerNumber = 0;
                buildingNode.nodeTransform = null;

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
}
