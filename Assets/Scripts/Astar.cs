using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    public float nodeDiameter;

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private float nodeRadius;

    [SerializeField] private Vector2 worldSize;
    public int worldXSize;
    public int worldYSize;

    private Node[,] worldNode;
    public bool endOfSetNode = false;

    [SerializeField] private LayerMask layerMask;

    public Vector3 bottomLeftPos;
    public Vector3 upperRightPos;

    //Dungeon
    [SerializeField] private Vector2 dungeonSize;
    [SerializeField] private LayerMask dungeonLayerMask;
    private int dungeonXSize;
    private int dungeonYSize;

    [SerializeField] private Transform woodDungeonMainNodeTransform;
    [SerializeField] private Transform abyssDungeonMainNodeTransform;
    [SerializeField] private Transform cellarDungeonMainNodeTransform;
    public Node[,] woodDungeonNode;
    public Node[,] abyssDungeonNode;
    public Node[,] cellarDungeonNode;
    //Dungeon

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        SetNodeToWorld();
    }

    private void Start()
    {

        bottomLeftPos = worldNode[0, 0].nodePosition;
        upperRightPos = worldNode[worldXSize - 1, worldYSize - 1].nodePosition;
    }

    //나중에 맵 변동 생길때 쓸거
    //public void ReSettingWorldSize(int x, int y)
    //{
    //    worldSize.x = x;
    //    worldSize.y = y;
    //
    //    SetNodeToWorld();
    //}

    private void SetNodeToWorld()
    {
        nodeDiameter = nodeRadius * 2f;

        worldXSize = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        worldYSize = Mathf.RoundToInt(worldSize.y / nodeDiameter);

        dungeonXSize = Mathf.RoundToInt(dungeonSize.x / nodeDiameter);
        dungeonYSize = Mathf.RoundToInt(dungeonSize.y / nodeDiameter);

        CreateWorldNode();
        CreateDungeonNode();
    }

    Collider[] buildingColliders;

    List<Node> testNodeList;

    private void CreateWorldNode()
    {
        testNodeList = new List<Node>();

        worldNode = new Node[worldXSize, worldYSize];
        Vector3 leftPosition = transform.position - new Vector3(worldSize.x * 0.5f, 0f, 0f);

        for (int x = 0; x < worldXSize; x++)
        {
            for (int y = 0; y < worldYSize; y++)
            {
                Vector3 nodePosition = leftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0.1f, nodeDiameter * y + nodeRadius);
                bool isWalkable = false;
                buildingColliders = Physics.OverlapSphere(nodePosition, nodeRadius * 0.5f, layerMask);


                worldNode[x, y] = new Node(x, y, nodePosition, isWalkable);

                worldNode[x, y].layerNumber = 0;

                //if ((worldNode[x, y].nodePosition.x == npcStartPosTransformForReturnRandomNode.position.x)
                //    && (worldNode[x, y].nodePosition.z) == npcStartPosTransformForReturnRandomNode.position.z)
                //{
                //    worldNode[x, y].isWalkable = true;
                //    worldNode[x, y].layerNumber = (int)GameLayer.Road;
                //}

                testNodeList.Add(worldNode[x, y]);

                if (buildingColliders.Length != 0)
                {
                    string[] names = buildingColliders[0].name.Split('_');
                    
                    if (names[0] == BuildingType.Shop.ToString() || names[1] == BuildingName.Platform.ToString())
                    {
                        buildingManager.buildingCount++;
                        isWalkable = true;
                    }

                    if (names[0] == BuildingType.Environment.ToString()
                        || names[0] == BuildingType.Shop.ToString())
                    {
                        worldNode[x, y].buildingType = names[0];
                        worldNode[x, y].buildingName = names[1];
                        worldNode[x, y].layerNumber = buildingColliders[0].gameObject.layer;
                        worldNode[x, y].isWalkable = isWalkable;
                        worldNode[x, y].nodeTransform = buildingColliders[0].transform.parent;

                        string nodePosToString = $"{x}_{y}";

                        //GameData.Instance.buildingDictionary.Add(nodePosToString, int.Parse(worldNode[x, y].nodeTransform.parent.name));
                    }
                }
            }
        }

        endOfSetNode = true;
    }

    private void CreateDungeonNode()
    {
        woodDungeonNode = new Node[dungeonXSize, dungeonYSize];
        Vector3 woodLeftPosition = woodDungeonMainNodeTransform.position - new Vector3(dungeonSize.x * 0.5f, 0f, 0f);

        abyssDungeonNode = new Node[dungeonXSize, dungeonYSize];
        Vector3 abyssLeftPosition = abyssDungeonMainNodeTransform.position - new Vector3(dungeonSize.x * 0.5f, 0f, 0f);

        cellarDungeonNode = new Node[dungeonXSize, dungeonYSize];
        Vector3 cellarLeftPosition = cellarDungeonMainNodeTransform.position - new Vector3(dungeonSize.x * 0.5f, 0f, 0f);

        for (int x = 0; x < dungeonXSize; x++)
        {
            for (int y = 0; y < dungeonYSize; y++)
            {
                Vector3 woodDungeonNodePosition = woodLeftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0.1f, nodeDiameter * y + nodeRadius);
                Vector3 abyssDungeonNodePosition = abyssLeftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0.1f, nodeDiameter * y + nodeRadius);
                Vector3 cellarDungeonNodePosition = cellarLeftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0.1f, nodeDiameter * y + nodeRadius);
                
                bool isWalkable = true;
                Collider[] woodColliders = Physics.OverlapSphere(woodDungeonNodePosition, nodeRadius * 0.5f, dungeonLayerMask);
                Collider[] abyssColliders = Physics.OverlapSphere(abyssDungeonNodePosition, nodeRadius * 0.5f, dungeonLayerMask);
                Collider[] cellarColliders = Physics.OverlapSphere(cellarDungeonNodePosition, nodeRadius * 0.5f, dungeonLayerMask);

                woodDungeonNode[x, y] = new Node(x, y, woodDungeonNodePosition, isWalkable);
                abyssDungeonNode[x, y] = new Node(x, y, abyssDungeonNodePosition, isWalkable);
                cellarDungeonNode[x, y] = new Node(x, y, cellarDungeonNodePosition, isWalkable);

                woodDungeonNode[x, y].layerNumber = (int)GameLayer.Road;
                abyssDungeonNode[x, y].layerNumber = (int)GameLayer.Road;
                cellarDungeonNode[x, y].layerNumber = (int)GameLayer.Road;

                testNodeList.Add(woodDungeonNode[x, y]);
                testNodeList.Add(abyssDungeonNode[x, y]);
                testNodeList.Add(cellarDungeonNode[x, y]);

                if (woodColliders.Length > 0)
                {
                    woodDungeonNode[x, y].isWalkable = false;
                }

                if (abyssColliders.Length > 0)
                {
                    abyssDungeonNode[x, y].isWalkable = false;
                }

                if (cellarColliders.Length > 0)
                {
                    cellarDungeonNode[x, y].isWalkable = false;
                }
            }
        }
    }

    public Node GetNodeByPosition(Vector3 nodePosition)
    {
        float xPercent = (nodePosition.x + worldSize.x * 0.5f) / worldSize.x;
        float yPercent = nodePosition.z / worldSize.y;

        xPercent = Mathf.Clamp01(xPercent);
        yPercent = Mathf.Clamp01(yPercent);

        int nodeXPos = Mathf.RoundToInt((worldXSize - 1) * xPercent);
        int nodeYPos = Mathf.RoundToInt((worldYSize - 1) * yPercent);

        return worldNode[nodeXPos, nodeYPos];
    }

    public Node GetNodeByPosition(Vector3 nodePosition, string dungeonName)
    {
        Debug.Log(nodePosition);

        float xPercent;
        float yPercent;

        int nodeXPos;
        int nodeYPos;

        float xValue;
        float yValue;

        switch (dungeonName)
        {
            case "wood":
                xValue = nodePosition.x - woodDungeonMainNodeTransform.position.x;
                yValue = nodePosition.z - woodDungeonMainNodeTransform.position.z;

                xPercent = (xValue + dungeonSize.x * 0.5f) / dungeonSize.x;
                yPercent = yValue / dungeonSize.y;

                xPercent = Mathf.Clamp01(xPercent);
                yPercent = Mathf.Clamp01(yPercent);

                nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                return woodDungeonNode[nodeXPos, nodeYPos];
            case "abyss":
                xValue = nodePosition.x - abyssDungeonMainNodeTransform.position.x;
                yValue = nodePosition.z - abyssDungeonMainNodeTransform.position.z;

                xPercent = (xValue + dungeonSize.x * 0.5f) / dungeonSize.x;
                yPercent = yValue / dungeonSize.y;

                xPercent = Mathf.Clamp01(xPercent);
                yPercent = Mathf.Clamp01(yPercent);

                nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                return abyssDungeonNode[nodeXPos, nodeYPos];
            case "cellar":
                xValue = nodePosition.x - cellarDungeonMainNodeTransform.position.x;
                yValue = nodePosition.z - cellarDungeonMainNodeTransform.position.z;

                xPercent = (xValue + dungeonSize.x * 0.5f) / dungeonSize.x;
                yPercent = yValue / dungeonSize.y;

                xPercent = Mathf.Clamp01(xPercent);
                yPercent = Mathf.Clamp01(yPercent);

                nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                return cellarDungeonNode[nodeXPos, nodeYPos];
        }

        return null;
    }

    public List<Node> GetAroundNode(Node middleNode)
    {
        List<Node> aroundNodeList = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0)
                    || (x == -1 && y == -1)
                    || (x == -1 && y == 1)
                    || (x == 1 && y == -1)
                    || (x == 1 && y == 1))
                {
                    continue;
                }

                int aroundNodeX = middleNode.xPosition + x;
                int aroundNodeY = middleNode.yPosition + y;

                if (aroundNodeX >= 0 && aroundNodeX < worldXSize && aroundNodeY >= 0 && aroundNodeY < worldYSize)
                {
                    if (worldNode[aroundNodeX, aroundNodeY].layerNumber == (int)GameLayer.Road)
                    {
                        aroundNodeList.Add(worldNode[aroundNodeX, aroundNodeY]);
                    }
                    else if (worldNode[aroundNodeX, aroundNodeY].buildingType == BuildingType.Shop.ToString())
                    {
                        aroundNodeList.Add(worldNode[aroundNodeX, aroundNodeY]);
                    }
                }
            }
        }

        return aroundNodeList;
    }

    public Transform npcStartPosTransformForReturnRandomNode;

    public void GetRandomNodeByLayer(NpcController npcController, int layerNumber, string buildingType)
    {
        StartCoroutine(GetRandomNodeByLayerCoroutine(npcController, layerNumber, buildingType));
    }

    IEnumerator GetRandomNodeByLayerCoroutine(NpcController npcController, int layerNumber, string buildingType)
    {
        Node node;

        if (npcController.target != npcStartPosTransformForReturnRandomNode.position
            && !npcController.firstEntrance)
        {

            npcController.StartDidntFoundNodeCalculateCoroutine();
        }
        else
        {
            npcController.firstEntrance = false;
            //Debug.Log(npcController.npcTransform.parent.name);
            //Debug.Log(npcController.didntFoundNode);
        }

        //일정시간 못 찾으면 나가는걸로(처음 스폰지점을 타겟으로 해줌)
        while (true)
        {
            //Debug.Log(npcController.npcTransform.name);

            int xNode = Random.Range(0, worldXSize - 1);
            int yNode = Random.Range(0, worldYSize - 1);
            node = worldNode[xNode, yNode];

            if (npcController.didntFoundNode)
            {
                Debug.Log(npcController.npcTransform.parent.name + npcController.target);

                npcController.didntFoundNode = false;
                //Debug.Log(npcController.didntFoundNode);
                npcController.target = npcStartPosTransformForReturnRandomNode.position;

                break;
            }

            //if (xNode != npcController.targetXPos && yNode != npcController.targetYPos)
            //{
                if (node.layerNumber == layerNumber || node.buildingType == buildingType)
                {
                    npcController.StopDidntFoundNodeCalculateCoroutine();
                    
                    npcController.target = node.nodePosition;
                    //npcController.targetXPos = xNode;
                    //npcController.targetYPos = yNode;

                    break;
                }
            //}

            yield return new WaitForFixedUpdate();
        }

        npcController.endOfSetTarget = true;
    }

    public Node[,] GetNode()
    {
        return worldNode;
    }
}
