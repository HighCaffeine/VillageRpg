using System;
using UnityEngine;

[Serializable]
public class Node
{
    public string buildingName;
    public string buildingType;

    public Node parentNode;

    public int xPosition;
    public int yPosition;

    public Vector3 nodePosition;

    public int gCost;
    public int hCost;
    public int FCost { get => gCost + hCost; }

    public bool isWalkable;

    public Node(int _xPosition, int _yPosition, Vector3 _nodePosition, bool _isWalkable)
    {
        xPosition = _xPosition;
        yPosition = _yPosition;
        nodePosition = _nodePosition;
        isWalkable = _isWalkable;
    }
}
