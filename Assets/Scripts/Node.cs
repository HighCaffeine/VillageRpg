using System;
using UnityEngine;

//node들의 대한 정보가 담겨있습니다.

[Serializable]
public class Node
{
    public Transform nodeTransform;

    public string buildingName;
    public string buildingType;
    public int layerNumber;

    public Node parentNode;

    public int xPosition;
    public int yPosition;

    public Vector3 nodePosition;

    public int gCost;
    public int hCost;
    public int FCost { get => gCost + hCost; }

    public bool isWalkable;
    public bool isBreakable;

    public Node(int _xPosition, int _yPosition, Vector3 _nodePosition, bool _isWalkable)
    {
        xPosition = _xPosition;
        yPosition = _yPosition;
        nodePosition = _nodePosition;
        isWalkable = _isWalkable;
    }
}
