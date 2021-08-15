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

    public Animator npcAnimator;
    public Vector3 target; // gameManager���� ������
    public int targetXPos = -1;
    public int targetYPos = -1;

    public float coroutineCheckTime;

    public bool endOfSetTarget = false;

    public bool didntFoundNode;
    public bool firstEntrance;

    public Transform targetTransform;

    public bool npcGoToDungeon = false;
    public bool endToDo;

    public bool arrivedDungeon;

    public int health;
    public int damage;
    public int armor;

    //     0      1     2      3     4    5
    //  unarmed sword hammer katana axe spear
    public MeshFilter weaponMeshFilter;
    public int weaponNumber = 0;

    public Transform targetInDungeon;

    private void OnEnable()
    {
        StartCoroutine(CheckTargetIsActiveTrue());
    }

    private void OnDisable()
    {
        StopCoroutine(CheckTargetIsActiveTrue());

        setNewTargetInDungeonRequestFromActiveFalseEnemy(targetInDungeon, false);
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

    public delegate void AttackEveryDelay(Transform mySelf, Transform target, bool isNpc);
    public AttackEveryDelay attackEveryDelay;

    //ó�� ���� Ÿ�� �����ָ� enemy�� npc �� �� target�� �������µ�
    //enemy�� activeFalse�ɶ� �Ű������� npcTarget�Ѱ��༭ ���ο� enemyTarget�� �����ְ� ����
    public delegate void SetNewTargetInDungeonRequestFromActiveFalseEnemy(Transform target, bool targetIsNpc);
    public SetNewTargetInDungeonRequestFromActiveFalseEnemy setNewTargetInDungeonRequestFromActiveFalseEnemy;

}
