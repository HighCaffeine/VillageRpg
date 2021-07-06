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
    public Vector3 target; // gameManager���� ������

    public float coroutineCheckTime;

    //���� ����� �ٰ���

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
