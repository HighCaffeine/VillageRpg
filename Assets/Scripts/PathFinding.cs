using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Astar astar;
    public delegate Stack<Vector3> PathFindDelegate(Vector3 npcPosition, Vector3 targetPosition);
    public PathFindDelegate pathFindDelegate;

    private void Awake()
    {
        astar = GetComponent<Astar>();
        pathFindDelegate += PathFind;
    }

    private Stack<Vector3> PathFind(Vector3 npcPosition, Vector3 targetPosition)
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
                return GetPath(npcNode, targetNode);
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

        return null;
    }

    private Stack<Vector3> GetPath(Node startNode, Node targetNode)
    {
        Stack<Vector3> returnNodePositionList = new Stack<Vector3>();

        returnNodePositionList.Push(targetNode.nodePosition); // 도착지점
        
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            if (currentNode.xPosition == startNode.xPosition || currentNode.yPosition == startNode.yPosition)
            {
                break; // start랑 같은 줄에 있음
            }

            if (currentNode.xPosition != currentNode.parentNode.parentNode.xPosition 
                && currentNode.yPosition != currentNode.parentNode.parentNode.yPosition)
            {
                returnNodePositionList.Push(currentNode.parentNode.nodePosition); // 수직부분
            }

            currentNode = currentNode.parentNode;
        }

        return returnNodePositionList;
    }
    
    //대각선 이동 안함
    private int GetDistance(Node firstNode, Node secondNode)
    {
        int xDistance = Mathf.Abs(firstNode.xPosition - secondNode.xPosition);
        int yDistance = Mathf.Abs(firstNode.yPosition - secondNode.yPosition);

        //if (xDistance < yDistance)
        //{
        //    return xDistance * 14 + (yDistance - xDistance) * 10;
        //}
        //else
        //{
        //    return yDistance * 14 + (xDistance - yDistance) * 10;
        //}

        return xDistance * 10 + yDistance * 10;
    }
}
