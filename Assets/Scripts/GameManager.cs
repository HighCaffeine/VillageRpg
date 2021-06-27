using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform npcPrefab;
    private List<Transform> npcPool;

    [SerializeField] private Transform buildingPrefab;

    private Astar astar;
    private PathFinding pathFinding;

    [SerializeField] private Transform npcStartTransform;

    private int gameSpeed = 5;

    [SerializeField] private int frameRate = 60;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>(); 
        astar = GetComponent<Astar>();
        npcPool = new List<Transform>();
    }

    private void Start()
    {
        Application.targetFrameRate = frameRate;
        NpcPooling();
    }

    private void NpcPooling()
    {
        for (int i = 0; i < GameData.Instance.npcNameList.Count; i++)
        {
            Transform npc = Instantiate(npcPrefab, npcParent);
            npc.position = npcStartTransform.position;
            npc.name = GameData.Instance.npcNameList[i];
            SetTarget(npc.gameObject);

            npc.gameObject.SetActive(false);

            npcPool.Add(npc);
            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);
        }
    }

    private void SetTarget(GameObject npc)
    {
        NpcController npcController = npc.GetComponent<NpcController>();

        npcController.target = GetTarget();

        npc.SetActive(false);
        Transform npcTransform = npc.transform.GetChild(0);
        npcTransform.gameObject.SetActive(true);
        npc.SetActive(true);

        //StartCoroutine(NpcGoToTarget(pathFinding.pathFindDelegate(npc.transform.position, npcController.target), npc.transform));
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
