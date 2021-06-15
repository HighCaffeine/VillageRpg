using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Astar astar;

    [SerializeField] private Transform buildingPrefabParent;
    [SerializeField] private Transform buildingBlockPrefabParent;

    private Queue<Transform> buildingQueue;

    private void Awake()
    {
        buildingQueue = new Queue<Transform>();
    }

    IEnumerator SetBuilding(Transform buildingTransform, Vector3 position, Vector3 rotation)
    {
        //게임이 일시정지가 아닐때까지 기다리기

        Node buildingNode = astar.GetNodeByPosition(position);
        string[] buildingNames = buildingTransform.name.Split('_');

        buildingNode.buildingType = buildingNames[0];
        buildingNode.buildingName = buildingNames[1];

        buildingTransform.position = position;
        buildingTransform.rotation = Quaternion.Euler(rotation);

        yield return null;
    }

}
