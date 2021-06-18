using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform npcTransform, targetTransform;
    private PathFinding pathFinding;
    private Astar astar;

    [SerializeField] private Transform npcStartPosition;

    private int gameSpeed = 5;

    private void Awake()
    {
        astar = GetComponent<Astar>();
        pathFinding = GetComponent<PathFinding>();
    }

    private void Start()
    {
        npcTransform.position = npcStartPosition.position;

        StartCoroutine(NpcGoToTarget(pathFinding.pathFindDelegate(npcTransform.position, targetTransform.position), npcTransform));
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
