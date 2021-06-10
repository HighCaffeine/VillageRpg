using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Astar astar;
    public delegate void PathFindDelegate(Vector3 npcPosition, Vector3 targetPosition);
    public PathFindDelegate pathFindDelegate;

    private List<Node> path;

    private void Awake()
    {
        astar = GetComponent<Astar>();
        pathFindDelegate += PathFind;
    }

    private void PathFind(Vector3 npcPosition, Vector3 targetPosition)
    {
        Node npcNode = astar.GetNodeByPosition(npcPosition);
        Node targetNode = astar.GetNodeByPosition(targetPosition);

        List<Node> openNode = new List<Node>();
        HashSet<Node> closeNode = new HashSet<Node>();

        openNode.Add(npcNode);

        while (openNode.Count != 0)
        {
            Node currentNode = openNode[0];

            for (int i = 1; i < openNode.Count; i++)
            {
                if (currentNode.FCost > openNode[i].FCost || (currentNode.FCost == openNode[i].FCost && currentNode.hCost > openNode[i].hCost))
                {
                    currentNode = openNode[i];
                }
            }

            openNode.Remove(currentNode);
            closeNode.Add(currentNode);

            if (currentNode == targetNode)
            {
                GetPath(npcNode, targetNode);
                break;
            }

            foreach (Node aroundNode in astar.GetAroundNode(currentNode))
            {
                if (closeNode.Contains(aroundNode) || !aroundNode.isWalkable)
                {
                    continue;
                }

                int newGCost = currentNode.gCost + GetDistance(currentNode, aroundNode);

                if (openNode.Contains(aroundNode))
                {
                    if (aroundNode.gCost > newGCost)
                    {
                        aroundNode.gCost = newGCost;

                        aroundNode.parentNode = currentNode;
                    }
                }
                else
                {
                    aroundNode.gCost = newGCost;
                    aroundNode.hCost = GetDistance(aroundNode, targetNode);
                    aroundNode.parentNode = currentNode;

                    openNode.Add(aroundNode);
                }
            }
        }
    }

    private void GetPath(Node startNode, Node targetNode)
    {
        path = new List<Node>();

        Node currentNode = targetNode;

        path.Add(currentNode);

        while (currentNode != startNode)
        {
            currentNode = currentNode.parentNode;

            path.Add(currentNode);
        }
    }

    private int GetDistance(Node firstNode, Node secondNode)
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

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            foreach (var node in path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(node.nodePosition, Vector3.one * 10f);
            }
        }
    }
}
