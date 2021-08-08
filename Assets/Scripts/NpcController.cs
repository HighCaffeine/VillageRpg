using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public Transform npcTransform;

    public delegate void SetTargetQueueMethod(Transform npc);
    public SetTargetQueueMethod setTargetQueueMethod;

    public delegate Node GetNodeByPosition(Vector3 position);
    public GetNodeByPosition getNodeByPosition;

    [SerializeField] private Animator npcAnimator;
    public Vector3 target; // gameManager에서 정해줌
    public int targetXPos = -1;
    public int targetYPos = -1;

    public float coroutineCheckTime;

    public bool endOfSetTarget = false;

    public bool didntFoundNode;
    public bool firstEntrance;

    public Transform targetTransform;

    public bool npcGoToDungeon = false;
    public bool endToDo;

    //스탯 만들어 줄거임

    private void OnEnable()
    {
        StartCoroutine(CheckTargetIsActiveTrue());
    }

    private void OnDisable()
    {
        StopCoroutine(CheckTargetIsActiveTrue());
    }

    public void StartDidntFoundNodeCalculateCoroutine()
    {
        StartCoroutine(DidntFoundNodeCalculate());
    }

    public void StopDidntFoundNodeCalculateCoroutine()
    {
        StopCoroutine(DidntFoundNodeCalculate());
    }

    IEnumerator DidntFoundNodeCalculate()
    {
        yield return new WaitForSeconds(Random.Range(5f, 15f) / GameData.Instance.gameSpeed);

        didntFoundNode = true;
    }

    IEnumerator CheckTargetIsActiveTrue()
    {
        if (target == Vector3.zero)
        {
            while (true)
            {
                if (target != Vector3.zero)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        while (true)
        {
            if (getNodeByPosition(target).nodeTransform == null)
            {
                setTargetQueueMethod(transform);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
