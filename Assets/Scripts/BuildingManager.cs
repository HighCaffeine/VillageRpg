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
    public bool nowBuilding = false;

    //�ǹ� ����°�
    IEnumerator SetBuilding()
    {
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

        //������ �Ͻ������� �ƴҶ����� ��ٸ���
        while (true)
        {
            
            position = cameraController.cameraParent.position;

            buildingNode = astar.GetNodeByPosition(position);

            buildingTransform.position = buildingNode.nodePosition;

            if (buildingNode.layerNumber == 0)
            {
                Debug.Log("buildingWhile");

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

        if (buildingNode.layerNumber == (int)GameLayer.road)
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

    // UI���� �ǹ� ���°� ���� �� �ǹ��� buildingTransform�� �־���
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

    //�ǹ� �μ���
    public void CallDemolition()
    {
        buildingWindow.gameObject.SetActive(false);

        StopCoroutine(setBuilding);
        StartCoroutine(BuildingDemolition());
    }

    private IEnumerator BuildingDemolition()
    {
        Debug.Log("startDemolition");

        if (buildingCount != 0)
        {
            Debug.Log("In if ");

            demolition = false;
            Node buildingNode;
        
            while (true)
            {
                if (this.demolition)
                {
                    Debug.Log("while Break");
                    break;
                }

                yield return new WaitForSeconds(buildingDelay);
            }

            Debug.Log("setNode");
            buildingNode = astar.GetNodeByPosition(cameraController.cameraParent.position);

            Debug.Log($"buildingNodeXPos : {buildingNode.xPosition}, buildingNodeYPos : {buildingNode.yPosition}");

            if (buildingNode.layerNumber != 0)
            {
                Debug.Log("layer is not 0");

                Debug.Log("demolition");
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

        Debug.Log("demolition end");

        yield return null;
    }

    public Transform buildingWindow;
    [SerializeField] private Transform buildingCountPrefab;
    private Transform buildingCountButtonParent; // ��� building �̸� �����ͼ� parent �޾ƿ��¿�

    //��ư parent
    [SerializeField] private Transform environmentCountButtonParent;
    [SerializeField] private Transform shopCountButtonParent;

    private int nextBuildingButtonCount = -1;


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
        //��ư �������     �̰͵� Ǯ�� ������ҵ�
        //���ο� �ǹ��� �ִٸ� �־���
        GameObject buttonObj = Instantiate(buildingCountPrefab).gameObject;
        Button button = buttonObj.GetComponent<Button>();
        Text childForSetText = buttonObj.transform.GetChild(0).GetComponent<Text>();

        //�����ʿ� �־��� ���ο� ��Ʈ�� �������
        int nextbuildingCountNewInt = new int();
        nextbuildingCountNewInt = nextBuildingButtonCount;

        if (nextbuildingCountNewInt >= 0)
        {
            //�ؽ�Ʈ ����
            string[] name = buildingPrefabParent.GetChild(nextbuildingCountNewInt).name.Split('_');
            childForSetText.text = name[1];

            //�θ� ����
            switch (name[0])
            {
                case "Environment":
                    buildingCountButtonParent = environmentCountButtonParent;
                    break;
                case "Shop":
                    buildingCountButtonParent = shopCountButtonParent;
                    break;
            }

            //������ �Ҵ�
            button.onClick.AddListener(delegate { ChooseBuildingToBuild(nextbuildingCountNewInt); });

            buttonObj.transform.SetParent(buildingCountButtonParent);
        }
        else
        {
            //�ı��� ��ư
            childForSetText.text = "Demolition";
            buildingCountButtonParent = environmentCountButtonParent;
            button.onClick.AddListener(delegate { CallDemolition(); });
            buttonObj.transform.SetParent(buildingCountButtonParent);
        }

        //������
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
