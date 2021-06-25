using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform npcPrefab;
    private List<GameObject> npcPool;

    [SerializeField] private Transform buildingPrefab;

    private Astar astar;
    private PathFinding pathFinding;

    [SerializeField] private Transform npcStartTransform;

    private int gameSpeed = 5;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();
        npcPool = new List<GameObject>();
    }

    private void Start()
    {
        //그냥 생성해서 테스트
        GameObject obj = Instantiate(npcPrefab, npcParent).gameObject;
        obj.transform.position = npcStartTransform.position;

        NpcController npcController = obj.GetComponent<NpcController>();

        npcController.target = GetTarget();

        obj.SetActive(false);
        Transform npcTransform = obj.transform.GetChild(0);
        npcTransform.gameObject.SetActive(true);
        obj.SetActive(true);

        npcPool.Add(obj);

        StartCoroutine(NpcGoToTarget(pathFinding.pathFindDelegate(obj.transform.position, npcController.target), obj.transform));
    }

    //이거 노드로 받아야됨
    private Vector3 GetTarget()
    {
        Node node = astar.GetRandomNodeByLayer((int)GameLayer.building, BuildingType.Shop.ToString());

        return node.nodePosition;
    }

    IEnumerator NpcGoToTarget(Stack<Vector3> path, Transform npcTransform)
    {
        int count = path.Count;
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        npcAnimator.SetFloat("Speed", 1f);

        for (int i = 0; i < count; i++)
        {
            Vector3 nextPos = path.Pop();

            npcTransform.LookAt(nextPos);

            while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
            {
                npcTransform.Translate(Vector3.forward * 0.01f * gameSpeed);

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        npcTransform.gameObject.SetActive(false);
        npcAnimator.SetFloat("Speed", 0f);

        yield return null;
    }
}
