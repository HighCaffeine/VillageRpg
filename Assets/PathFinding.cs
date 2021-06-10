using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Astar astar;

    private void Awake()
    {
        astar = GetComponent<Astar>();
    }

    private void PathFind(Vector3 npcPosition, Vector3 targetPosition)
    {
        Node npcNode = astar.GetNodeByPosition(npcPosition);
        Node targetNode = astar.GetNodeByPosition(targetPosition);

        List<Node> openNode = new List<Node>();
        HashSet<Node> closeNode = new HashSet<Node>();

        openNode.Add(npcNode);


    }

    private int GetHCost(Node firstNode, Node secondNode)
    {
        int xDistance = Mathf.Abs(firstNode.xPosition - secondNode.xPosition);
        int yDistance = Mathf.Abs(firstNode.yPosition - secondNode.yPosition);

        if (xDistance < yDistance)
        {
            return xDistance * 14 + (yDistance - xDistance) * 10;
        }
        else
        {
            return yDistance * 14 + (xDistance - yDistance) * 10;
        }
    }
}
