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

    //회전값 확인
    [SerializeField]private Vector3 rotate;

    private void Awake()
    {
        astar = GetComponent<Astar>();
    }

    //input에서 버튼 누르는것도 touch가 돼서 build가 true임
    //                                        얘가                얘를 누르는지를 알아야됨
    //touch(started) -> Move(performed) -> touch(canceled) -> CallSetBuilding(button)
    //                                       여기서 버튼일때 예외처리를 해줘야하는데 순서때문에 안됨
    public void CallSetBuilding(int buildingNumber)
    {
        build = false;

        //생성하는걸로 바꿔야됨 pooling해줄거
        buildingTransform = buildingPrefabParent.GetChild(buildingNumber);
        buildingTransform.gameObject.SetActive(true);


        StartCoroutine("SetBuilding", buildingTransform);
    }

    //건물 만드는거
    IEnumerator SetBuilding(Transform buildingTransform)
    {
        Vector3 position;
        Node buildingNode;

        //게임이 일시정지가 아닐때까지 기다리기
        do
        {
            position = cameraController.cameraParent.position;

            buildingNode = astar.GetNodeByPosition(position);
            buildingTransform.position = buildingNode.nodePosition;

            yield return new WaitForFixedUpdate();
        }
        while (!build);

        build = false;

        string[] buildingNames = buildingTransform.name.Split('_');

        buildingNode.buildingType = buildingNames[0];
        buildingNode.buildingName = buildingNames[1];

        yield return null;
    }

    private int rotateCount = 0;

    public void ChangeRotationValue()
    {
        if (rotateCount == 0)
        {
            buildingTransform.Rotate(new Vector3(0f, -90f, 0f));
            rotateCount++;
        }
        else
        {
            buildingTransform.Rotate(Vector3.zero);
            rotateCount = 0;
        }
    }
}
