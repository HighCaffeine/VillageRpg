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

    //ȸ���� Ȯ��
    [SerializeField]private Vector3 rotate;

    private void Awake()
    {
        astar = GetComponent<Astar>();
    }

    //input���� ��ư �����°͵� touch�� �ż� build�� true��
    //                                        �갡                �긦 ���������� �˾ƾߵ�
    //touch(started) -> Move(performed) -> touch(canceled) -> CallSetBuilding(button)
    //                                       ���⼭ ��ư�϶� ����ó���� ������ϴµ� ���������� �ȵ�
    public void CallSetBuilding(int buildingNumber)
    {
        build = false;

        //�����ϴ°ɷ� �ٲ�ߵ� pooling���ٰ�
        buildingTransform = buildingPrefabParent.GetChild(buildingNumber);
        buildingTransform.gameObject.SetActive(true);


        StartCoroutine("SetBuilding", buildingTransform);
    }

    //�ǹ� ����°�
    IEnumerator SetBuilding(Transform buildingTransform)
    {
        Vector3 position;
        Node buildingNode;

        //������ �Ͻ������� �ƴҶ����� ��ٸ���
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
