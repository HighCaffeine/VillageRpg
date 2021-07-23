using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public delegate void SetTargetQueueMethod(Transform npc);
    public SetTargetQueueMethod setTargetQueueMethod;

    public delegate Node GetNodeByPosition(Vector3 position);
    public GetNodeByPosition getNodeByPosition;

    [SerializeField] private Animator npcAnimator;
    public Vector3 target; // gameManager에서 정해줌
    public int targetXPos = -1;
    public int targetYPos = -1;

    public float coroutineCheckTime;

    //스탯 만들어 줄거임

    private void OnEnable()
    {
        StartCoroutine(CheckTargetIsActiveTrue());
    }

    private void OnDisable()
    {
        StopCoroutine(CheckTargetIsActiveTrue());
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
