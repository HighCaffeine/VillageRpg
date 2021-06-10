using System;
using UnityEngine;

[Serializable]
public class Node
{
    public int xPosition;
    public int yPosition;

    public Vector3 nodePosition;

    public int gCost;
    public int hCost;
    public int FCost { get => gCost + hCost; }

    public Node(int _xPosition, int _yPosition, Vector3 _nodePosition)
    {
        xPosition = _xPosition;
        yPosition = _yPosition;
        nodePosition = _nodePosition;
    }
}
