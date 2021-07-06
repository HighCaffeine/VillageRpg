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

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
    }

    private void Start()
    {
        SetNodeToWorld();

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

        CreateNode();
    }

    Collider[] buildingColliders;

    public delegate void AddToDungeonList(Transform dungeon);
    public AddToDungeonList addToDungeonList;

    private void CreateNode()
    {
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

                if (buildingColliders.Length != 0)
                {
                    string[] names = buildingColliders[0].name.Split('_');
                    
                    if (names[0] == BuildingType.Shop.ToString() || names[1] == BuildingName.Platform.ToString())
                    {
                        buildingManager.buildingCount++;
                        isWalkable = true;
                    }

                    if (worldNode[x, y].buildingType == BuildingType.Dungeon.ToString())
                    {
                        worldNode[x, y].buildingType = names[0];
                        worldNode[x, y].buildingName = names[0];
                        worldNode[x, y].layerNumber = buildingColliders[0].gameObject.layer;
                        worldNode[x, y].isWalkable = true;
                        worldNode[x, y].nodeTransform = buildingColliders[0].transform;

                        addToDungeonList(buildingColliders[0].transform);
                    }
                    else
                    {
                        worldNode[x, y].buildingType = names[0];
                        worldNode[x, y].buildingName = names[1];
                        worldNode[x, y].layerNumber = buildingColliders[0].gameObject.layer;
                        worldNode[x, y].isWalkable = isWalkable;
                        worldNode[x, y].nodeTransform = buildingColliders[0].transform.parent;
                    }

                    string nodePosToString = $"{x}_{y}";

                    if (!GameData.Instance.buildingDictionary.ContainsKey(nodePosToString))
                    {
                        GameData.Instance.buildingDictionary.Add(nodePosToString, buildingColliders[0].name);
                    }
                }
            }
        }

        endOfSetNode = true;
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

    public Node GetRandomNodeByLayer(int layerNumber, string buildingType)
    {
        Node node = null;

        //여기서 몬스터노드/빌딩노드 찾아야됨
        do
        {
            int xNode = Random.Range(0, worldXSize - 1);
            int yNode = Random.Range(0, worldYSize - 1);
            node = worldNode[xNode, yNode];
        }
        while (node.layerNumber != layerNumber || node.buildingType != buildingType
                || (node.layerNumber == (int)GameLayer.Dungeon && node.nodeTransform.gameObject.activeSelf));

        return node;
    }

    public Node[,] GetNode()
    {
        return worldNode;
    }
}
