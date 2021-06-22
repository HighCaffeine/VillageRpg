using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Astar astar;

    [SerializeField] private Transform buildingPrefabParent;
    public Transform buildingTransform;
    public bool build = false;

    private void Awake()
    {
        buildingTransform = null;
        astar = GetComponent<Astar>();
    }

    //input에서 버튼 누르는것도 touch가 돼서 build가 true임
    //                                        얘가                얘를 누르는지를 알아야됨
    //touch(started) -> Move(performed) -> touch(canceled) -> CallSetBuilding(button)
    //                                       여기서 버튼일때 예외처리를 해줘야하는데 순서때문에 안됨
    public void CallSetBuilding()
    {
        if (!cameraController.cameraMove)
        {
            //생성하는걸로 바꿔야됨 pooling해줄거
            //buildingTransform = buildingPrefabParent.GetChild(buildingNumber);

            StartCoroutine("SetBuilding");
        }
    }

    [SerializeField] private float buildingDelay = 0.5f;

    //건물 만드는거
    IEnumerator SetBuilding()
    {
        Vector3 position;
        Node buildingNode;

        build = false;

        yield return buildingTransform != null;
        Debug.Log("setBuildSuccess");

        buildingTransform.gameObject.SetActive(true);
        //게임이 일시정지가 아닐때까지 기다리기
        do
        {
            position = cameraController.cameraParent.position;

            buildingNode = astar.GetNodeByPosition(position);
            buildingTransform.position = buildingNode.nodePosition;

            yield return new WaitForSeconds(buildingDelay);
        }
        while (!build);

        build = false;

        string[] buildingNames = buildingTransform.name.Split('_');

        buildingNode.buildingType = buildingNames[0];
        buildingNode.buildingName = buildingNames[1];
        buildingNode.layerNumber = buildingTransform.gameObject.layer;

        this.buildingTransform = null;

        yield return null;
    }

    private int rotateCount = 0;

    public void ChangeRotationValue()
    {
        if (buildingTransform != null)
        {
            if (rotateCount == 0)
            {
                buildingTransform.Rotate(new Vector3(0f, -90f, 0f));
                rotateCount++;
            }
            else
            {
                buildingTransform.Rotate(new Vector3(0f, 90f, 0f));
                rotateCount = 0;
            }
        }
    }

    // UI에서 건물 고르는거 띄우고 고른 건물을 buildingTransform에 넣어줌
    public void ChooseBuildingToBuild(int value)
    {
        buildingTransform = buildingPrefabParent.GetChild(value);
    }
}
