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

    private Queue<Transform> setTargetQueue;
    private Queue<Transform> goTargetQueue;

    private void Awake()
    {
        setTargetQueue = new Queue<Transform>();
        goTargetQueue = new Queue<Transform>();

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

            npc.GetChild(Random.Range(0, npc.childCount - 1)).gameObject.SetActive(true);

            npc.gameObject.SetActive(false);

            setTargetQueue.Enqueue(npc);

            npcPool.Add(npc);
            GameData.Instance.npcTransformDictionary.Add(GameData.Instance.npcNameList[i], npc);
        }

        StartCoroutine(SetTarget());
        StartCoroutine(NpcGoToTarget());
    }

    private Vector3 GetTarget(Vector3 target)
    {
        Node node = null;

        while (true)
        {
            node = astar.GetRandomNodeByLayer((int)GameLayer.building, BuildingType.Shop.ToString());
                
            //갈거 없으면 위로 가서 몹 잡는걸로
            if (target != node.nodePosition)
            {
                break;
            }
            else
            {
                node = astar.GetRandomNodeByLayer((int)GameLayer.building, BuildingType.Shop.ToString());

                if (target != node.nodePosition)
                {
                    break;
                }
            }
        }

        return node.nodePosition;
    }

    private IEnumerator SetTarget()
    {
        while (true)
        {
            if (setTargetQueue.Count == 0)
            {
                while (true)
                {
                    if (setTargetQueue.Count != 0)
                    {
                        break;
                    }

                    yield return new WaitForFixedUpdate();
                }
            }


            Transform npcTransform = setTargetQueue.Dequeue();
            NpcController npcController = npcTransform.GetComponent<NpcController>();

            npcController.target = GetTarget(npcController.target);

            goTargetQueue.Enqueue(npcTransform);
        }
    }

    IEnumerator NpcGoToTarget()
    {
        while (true)
        {
            if (goTargetQueue.Count == 0)
            {
                while (true)
                {
                    if (goTargetQueue.Count != 0)
                    {
                        break;
                    }

                    yield return new WaitForFixedUpdate();
                }
            }

            Transform npcTransform = goTargetQueue.Dequeue();

            StartCoroutine(Go(npcTransform));
            
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Go(Transform npcTransform)
    {
        NpcController npcController = npcTransform.GetComponent<NpcController>();

        //while (true)
        //{
        //    if (npcController.target)
        //}
        
        Stack<Vector3> path = pathFinding.pathFindDelegate(npcTransform.position, npcController.target);
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        int count = path.Count;

        npcTransform.gameObject.SetActive(true);

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

        setTargetQueue.Enqueue(npcTransform);
        npcTransform.gameObject.SetActive(false);
        npcAnimator.SetFloat("Speed", 0f);
    }
}
