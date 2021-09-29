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

    //Dungeon
    [SerializeField] private Vector2 dungeonSize;
    [SerializeField] private LayerMask dungeonLayerMask;
    public int dungeonXSize;
    public int dungeonYSize;

    public Transform woodDungeonMainNodeTransform;
    public Transform abyssDungeonMainNodeTransform;
    public Transform cellarDungeonMainNodeTransform;
    public Node[,] woodDungeonNode;
    public Node[,] abyssDungeonNode;
    public Node[,] cellarDungeonNode;

    public Vector3 woodEnemySpawnLeftBottomPoint;
    public Vector3 abyssEnemySpawnLeftBottomPoint;
    public Vector3 cellarEnemySpawnLeftBottomPoint;
    //Dungeon

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        SetNodeToWorld();
    }

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

    public Vector3 leftPosition;

    private void CreateWorldNode()
    {
        worldNode = new Node[worldXSize, worldYSize];
        leftPosition = transform.position - new Vector3(worldSize.x * 0.5f, 0f, 0f);

        for (int x = 0; x < worldXSize; x++)
        {
            for (int y = 0; y < worldYSize; y++)
            {
                Vector3 nodePosition = leftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0.1f, nodeDiameter * y + nodeRadius);
                bool isWalkable = false;
                buildingColliders = Physics.OverlapSphere(nodePosition, nodeRadius * 0.5f, layerMask);


                worldNode[x, y] = new Node(x, y, nodePosition, isWalkable);

                worldNode[x, y].layerNumber = 0;

                if (buildingColliders.Length != 0)
                {
                    string[] names = buildingColliders[0].name.Split('_');
                    
                    if (names[1] == BuildingName.Platform.ToString())
                    {
                        buildingManager.buildingCount++;
                        isWalkable = true;
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
            }
        }
    }

    [SerializeField] private Transform mainFieldMiddle;
    [SerializeField] private Transform woodFieldMiddle;
    [SerializeField] private Transform abyssFieldMiddle;
    [SerializeField] private Transform cellarFieldMiddle;

    public float GetCameraLimitValueEachVertexInMain(string vertexName)
    {
        float returnValue = 0f;

        switch (vertexName)
        {
            case "bottom":
                returnValue = mainFieldMiddle.position.z - (worldYSize - 1) * nodeDiameter;

                break;
            case "upper":
                returnValue = mainFieldMiddle.position.z + (worldYSize - 1) * nodeDiameter;

                break;
            case "left":
                returnValue = mainFieldMiddle.position.x - (worldXSize - 1) * nodeDiameter;

                break;
            case "right":
                returnValue = mainFieldMiddle.position.x + (worldXSize - 1) * nodeDiameter;

                break;
        }

        return returnValue;
    }

    public float GetCameraLimitValueEachVertexInDungeon(string vertexName, string dungeonName)
    {
        float returnValue = 0f;

        switch (vertexName)
        {
            case "bottom":
                switch (dungeonName)
                {
                    case "Wood":
                        return woodFieldMiddle.position.z - (dungeonYSize - 1) * nodeDiameter;
                    case "Abyss":
                        return abyssFieldMiddle.position.z - (dungeonYSize - 1) * nodeDiameter;
                    case "Cellar":
                        return cellarFieldMiddle.position.z - (dungeonYSize - 1) * nodeDiameter;
                }

                break;
            case "upper":
                switch (dungeonName)
                {
                    case "Wood":
                        return woodFieldMiddle.position.z + (dungeonYSize - 1) * nodeDiameter;
                    case "Abyss":
                        return woodFieldMiddle.position.z + (dungeonYSize - 1) * nodeDiameter;
                    case "Cellar":
                        return woodFieldMiddle.position.z + (dungeonYSize - 1) * nodeDiameter;
                }

                break;
            case "left":
                switch (dungeonName)
                {
                    case "Wood":
                        return woodFieldMiddle.position.x - (dungeonXSize - 1) * nodeDiameter;
                    case "Abyss":
                        return woodFieldMiddle.position.x - (dungeonXSize - 1) * nodeDiameter;
                    case "Cellar":
                        return woodFieldMiddle.position.x - (dungeonXSize - 1) * nodeDiameter;
                }

                break;
            case "right":
                switch (dungeonName)
                {
                    case "Wood":
                        return woodFieldMiddle.position.x + (dungeonXSize - 1) * nodeDiameter;
                    case "Abyss":
                        return woodFieldMiddle.position.x + (dungeonXSize - 1) * nodeDiameter;
                    case "Cellar":
                        return woodFieldMiddle.position.x + (dungeonXSize - 1) * nodeDiameter;
                }

                break;
        }

        return returnValue;
    }

    public Node GetNodeByPosition(Vector3 nodePosition, bool isDungeon, string dungeonName)
    {
        float xPercent;
        float yPercent;

        int nodeXPos = 0;
        int nodeYPos = 0;

        if (isDungeon)
        {
            switch (dungeonName)
            {
                case "Wood":
                    xPercent = ((nodePosition.x - woodDungeonMainNodeTransform.position.x) + dungeonSize.x * 0.5f) / dungeonSize.x;
                    yPercent = (nodePosition.z - woodDungeonMainNodeTransform.position.z) / dungeonSize.y;

                    xPercent = Mathf.Clamp01(xPercent);
                    yPercent = Mathf.Clamp01(yPercent);

                    nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                    nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                    return woodDungeonNode[nodeXPos, nodeYPos];
                case "Abyss":
                    xPercent = ((nodePosition.x - abyssDungeonMainNodeTransform.position.x) + dungeonSize.x * 0.5f) / dungeonSize.x;
                    yPercent = (nodePosition.z - abyssDungeonMainNodeTransform.position.z) / dungeonSize.y;

                    xPercent = Mathf.Clamp01(xPercent);
                    yPercent = Mathf.Clamp01(yPercent);

                    nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                    nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                    return abyssDungeonNode[nodeXPos, nodeYPos];
                case "Cellar":
                    xPercent = ((nodePosition.x - cellarDungeonMainNodeTransform.position.x) + dungeonSize.x * 0.5f) / dungeonSize.x;
                    yPercent = (nodePosition.z - cellarDungeonMainNodeTransform.position.z) / dungeonSize.y;

                    xPercent = Mathf.Clamp01(xPercent);
                    yPercent = Mathf.Clamp01(yPercent);

                    nodeXPos = Mathf.RoundToInt((dungeonXSize - 1) * xPercent);
                    nodeYPos = Mathf.RoundToInt((dungeonYSize - 1) * yPercent);

                    return cellarDungeonNode[nodeXPos, nodeYPos];
            }
        }
        else
        {
            xPercent = (nodePosition.x + worldSize.x * 0.5f) / worldSize.x;
            yPercent = nodePosition.z / worldSize.y;

            xPercent = Mathf.Clamp01(xPercent);
            yPercent = Mathf.Clamp01(yPercent);

            nodeXPos = Mathf.RoundToInt((worldXSize - 1) * xPercent);
            nodeYPos = Mathf.RoundToInt((worldYSize - 1) * yPercent);

            //Debug.Log($"{nodePosition}, {nodeXPos}, {nodeYPos}");

            return worldNode[nodeXPos, nodeYPos];
        }

        return null;
    }

    public List<Node> GetAroundNode(Node middleNode, bool isDungeon, string dungeonName)
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

                if (isDungeon)
                {
                    if (aroundNodeX >= 0 && aroundNodeX < dungeonXSize && aroundNodeY >= 0 && aroundNodeY < dungeonYSize)
                    {
                        switch (dungeonName)
                        {
                            case "Wood":
                                if (woodDungeonNode[aroundNodeX, aroundNodeY].isWalkable)
                                {
                                    aroundNodeList.Add(woodDungeonNode[aroundNodeX, aroundNodeY]);
                                }

                                break;
                            case "Abyss":
                                if (abyssDungeonNode[aroundNodeX, aroundNodeY].isWalkable)
                                {
                                    aroundNodeList.Add(abyssDungeonNode[aroundNodeX, aroundNodeY]);
                                }

                                break;
                            case "Cellar":
                                if (cellarDungeonNode[aroundNodeX, aroundNodeY].isWalkable)
                                {
                                    aroundNodeList.Add(cellarDungeonNode[aroundNodeX, aroundNodeY]);
                                }

                                break;
                        }
                    }
                }
                else
                {
                    if (aroundNodeX >= 0 && aroundNodeX < worldXSize && aroundNodeY >= 0 && aroundNodeY < worldYSize)
                    {
                        if (worldNode[aroundNodeX, aroundNodeY].layerNumber == (int)GameLayer.Road)
                        {
                            aroundNodeList.Add(worldNode[aroundNodeX, aroundNodeY]);
                        }
                        else if (worldNode[aroundNodeX, aroundNodeY].buildingType == BuildingType.Shop.ToString()
                            || worldNode[aroundNodeX, aroundNodeY].buildingType == BuildingType.Dungeon.ToString())
                        {
                            aroundNodeList.Add(worldNode[aroundNodeX, aroundNodeY]);
                        }
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

            npcController.didntFoundNode = false;
        }

        //일정시간 못 찾으면 나가는걸로(처음 스폰지점을 타겟으로 해줌)
        while (true)
        {
            int xNode = Random.Range(0, worldXSize - 1);
            int yNode = Random.Range(0, worldYSize - 1);
            node = worldNode[xNode, yNode];

            if (npcController.npcGoToDungeon)
            {
                npcController.StopDidntFoundNodeCalculateCoroutine();
                break;
            }

            if (npcController.didntFoundNode)
            {
                npcController.StopDidntFoundNodeCalculateCoroutine();

                npcController.didntFoundNode = false;
                npcController.target = npcStartPosTransformForReturnRandomNode.position;

                break;
            }

            if (node.layerNumber == layerNumber || node.buildingType == buildingType)
            {
                //if (node.buildingName == "Hotel" && npcController.health != npcController.maxHealth)
                //{
                //    npcController.StopDidntFoundNodeCalculateCoroutine();
                //    npcController.didntFoundNode = false;

                //    npcController.target = node.nodePosition;
                //}
                //else
                //{
                    npcController.StopDidntFoundNodeCalculateCoroutine();
                    npcController.didntFoundNode = false;

                    npcController.target = node.nodePosition;
                //}

                break;
            }

            //if (node.layerNumber == layerNumber )

            yield return new WaitForFixedUpdate();
        }

        npcController.endOfSetTarget = true;
    }

    public Node[,] GetNode()
    {
        return worldNode;
    }
}
