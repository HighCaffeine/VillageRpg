using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Manager : MonoBehaviour
{
    private void GetTarget(NpcController npcController)
    {
        astar.GetRandomNodeByLayer(npcController, (int)GameLayer.Building, BuildingType.Shop.ToString());
    }

    private IEnumerator SetTarget()
    {
        while (true)
        {
            //타겟을 정해줄 npc가 들어올때까지 기다림
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

            GetTarget(npcController);



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
            NpcController npcController = npcTransform.GetComponent<NpcController>();

            StartCoroutine(Go(npcTransform, astar.GetNodeByPosition(npcController.target).nodeTransform, npcController));

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Go(Transform npcTransform, Transform targetTransform, NpcController npcController)
    {
        while (true)
        {
            if (npcController.endOfSetTarget)
            {
                npcController.endOfSetTarget = false;

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        Stack<Vector3> path = pathFinding.pathFindDelegate(npcTransform.position, npcController.target);
        Animator npcAnimator = npcTransform.GetComponent<Animator>();

        bool targetIsActive = true;

        if (path != null)
        {
            int count = path.Count;

            npcController.npcTransform.gameObject.SetActive(true);

            npcAnimator.SetFloat("Speed", 1f);

            for (int i = 0; i < count; i++)
            {
                Vector3 nextPos = path.Pop();

                npcTransform.LookAt(nextPos);

                while (Vector3.Distance(nextPos, npcTransform.position) >= 0.1f)
                {
                    if (pause)
                    {
                        while (true)
                        {
                            if (!pause)
                            {
                                break;
                            }
                        }

                        yield return new WaitForFixedUpdate();
                    }

                    npcTransform.Translate(Vector3.forward * 0.05f * GameData.Instance.gameSpeed);

                    if (!targetTransform.gameObject.activeSelf)
                    {
                        targetIsActive = false;
                        break;
                    }

                    yield return new WaitForFixedUpdate();
                }

                yield return null;
            }

            if (targetIsActive)
            {
                npcController.npcTransform.gameObject.SetActive(false);

                if (npcController.target == astar.npcStartPosTransformForReturnRandomNode.position)
                {
                    npcTransform.position = astar.npcStartPosTransformForReturnRandomNode.position;
                }
            }

            npcAnimator.SetFloat("Speed", 0f);
        }

        setTargetQueue.Enqueue(npcTransform);
    }
}
