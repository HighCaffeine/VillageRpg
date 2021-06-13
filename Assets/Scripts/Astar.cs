using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    private float nodeDiameter;
    [SerializeField] private float nodeRadius;

    [SerializeField] private Vector2 worldSize;
    public int worldXSize;
    public int worldYSize;

    private Node[,] worldNode;

    [SerializeField] private LayerMask layerMask;

    //test
    public PathFinding pathFinding;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        SetNodeToWorld();
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

    private void CreateNode()
    {
        worldNode = new Node[worldXSize, worldYSize];
        Vector3 bottomLeftPosition = transform.position - new Vector3(worldSize.x * 0.5f, 0f, worldSize.y * 0.5f);

        for (int x = 0; x < worldXSize; x++)
        {
            for (int y = 0; y < worldYSize; y++)
            {
                Vector3 nodePosition = bottomLeftPosition + new Vector3(nodeDiameter * x + nodeRadius, 1f, nodeDiameter * y + nodeRadius);
                bool isWalkable = !Physics.CheckSphere(nodePosition, nodeRadius, layerMask);

                worldNode[x, y] = new Node(x, y, nodePosition, isWalkable);
            }
        }
    }

    public Node GetNodeByPosition(Vector3 nodePosition)
    {
        float xPercent = (nodePosition.x + worldSize.x * 0.5f) / worldSize.x;
        float yPercent = (nodePosition.z + worldSize.y * 0.5f) / worldSize.y;

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
                    aroundNodeList.Add(worldNode[aroundNodeX, aroundNodeY]);
                }
            }
        }

        return aroundNodeList;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < worldXSize; x++)
        {
            for (int y = 0; y < worldYSize; y++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(worldNode[x, y].nodePosition, Vector3.one * 2f);
            }
        }
    }
}
