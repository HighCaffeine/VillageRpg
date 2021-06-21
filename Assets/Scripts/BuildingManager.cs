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
        astar = GetComponent<Astar>();
    }

    //input���� ��ư �����°͵� touch�� �ż� build�� true��
    //                                        �갡                �긦 ���������� �˾ƾߵ�
    //touch(started) -> Move(performed) -> touch(canceled) -> CallSetBuilding(button)
    //                                       ���⼭ ��ư�϶� ����ó���� ������ϴµ� ���������� �ȵ�
    public void CallSetBuilding(int buildingNumber)
    {
        if (!cameraController.cameraMove)
        {
            //�����ϴ°ɷ� �ٲ�ߵ� pooling���ٰ�
            buildingTransform = buildingPrefabParent.GetChild(buildingNumber);
            buildingTransform.gameObject.SetActive(true);

            StartCoroutine("SetBuilding", buildingTransform);
        }
    }

    [SerializeField] private float buildingDelay = 0.5f;

    //�ǹ� ����°�
    IEnumerator SetBuilding(Transform buildingTransform)
    {
        Vector3 position;
        Node buildingNode;

        build = false;

        //������ �Ͻ������� �ƴҶ����� ��ٸ���
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
}
