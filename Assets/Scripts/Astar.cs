using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    private float nodeDiameter;
    [SerializeField] private float nodeRadius;

    [SerializeField] private Vector2 worldSize;
    private int worldXSize;
    private int worldYSize;

    private Node[,] worldNode;

    private void Awake()
    {
        SetNodeToWorld();
    }

    //���߿� �� ���� ���涧 ����
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
                Vector3 nodePosition = bottomLeftPosition + new Vector3(nodeDiameter * x + nodeRadius, 0f, nodeDiameter * y + nodeRadius);

                worldNode[x, y] = new Node(x, y, nodePosition);
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
}
