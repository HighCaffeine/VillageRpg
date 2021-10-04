using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//npc에대한 정보, npc가 자신이 가야하는 건물 또는 던전 내에서 enemy가 꺼져있는지 확인하는 delegate가 있습니다.
//npc가 던전에서 죽었을 경우 manager에게 자신이 죽었고 죽었을때 가는 타겟을 정해달라고 delegate로 요청합니다.

public class NpcController : MonoBehaviour
{
    public Transform npcTransform;

    public delegate Node GetNodeByPosition(Vector3 position, bool isDungeon, string dungeonName);
    public GetNodeByPosition getNodeByPosition;

    public Animator npcAnimator;
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

    public bool arrivedDungeon;

    public int maxHealth;
    public int health;
    public int beforeHealHealth;

    public bool npcIsDead;

    public int damage;
    public int armor;

    public int money;

    public Transform targetInDungeon;

    //     0          1     2      3     4    5
    //  brokenSword sword hammer katana axe spear
    public Transform weaponParent;
    public int weaponNumber = 0;
    //      0       2         3             4
    //  Unarmed shieldhalf shieldKnight shieldViking
    public Transform shieldParent;
    public int shieldNumber;
    //     1
    // shamanMask
    public Transform maskParent;

    public Transform weaponTransformForBuyAnimation;
    public Transform armorTransformForBuyAnimation;
    public Transform healthTransformForBuyAnimation;
    public bool playAnimation = false;
    public string itemBuyType;

    private IEnumerator checkTargetIsActiveTrueCoroutine;
    private IEnumerator DidntFoundNodeCalculateCoroutine;

    private void Awake()
    {
        checkTargetIsActiveTrueCoroutine = CheckTargetIsActiveTrue();
        DidntFoundNodeCalculateCoroutine = DidntFoundNodeCalculate();
    }

    private void OnEnable()
    {
        if (!npcGoToDungeon)
        {
            StartCoroutine(checkTargetIsActiveTrueCoroutine);
        }
    }

    private void OnDisable()
    {
        if (!npcGoToDungeon)
        {
            StopCoroutine(checkTargetIsActiveTrueCoroutine);
        }
    }

    public void StartDidntFoundNodeCalculateCoroutine()
    { 
        StartCoroutine(DidntFoundNodeCalculateCoroutine);
    }

    public void StopDidntFoundNodeCalculateCoroutine()
    {
        StopCoroutine(DidntFoundNodeCalculateCoroutine);
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
            if (getNodeByPosition(target, false, null).nodeTransform == null)
            {
                setTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon(transform);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void SetDungeonValueTurnOff()
    {
        npcGoToDungeon = false;
        arrivedDungeon = false;
    }

    public void Die()
    {
        targetTransform = null;
        npcIsDead = true;
        targetInDungeon = null;
        npcAnimator.SetBool("NpcIsDead", true);

        npcAnimator.ResetTrigger("Attack");

        npcTransform.gameObject.SetActive(false);
        weaponParent.gameObject.SetActive(false);

        StartCoroutine(NpcGoToDungeonEntranceWhenNpcIsDead());
        setTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon(transform);

    }

    IEnumerator NpcGoToDungeonEntranceWhenNpcIsDead()
    {
        yield return null;
        StopAllCoroutines();

        npcMoveToDungeonEntrance(transform);

    }

    public void CallIfNpcIsDeadAndNpcGoToNpcStartPositionThenNpcWaitForASeconds(float waitTime)
    {
        StartCoroutine(IfNpcIsDeadAndNpcGoToNpcStartPositionThenNpcWaitForASeconds(waitTime));
    }

    private IEnumerator IfNpcIsDeadAndNpcGoToNpcStartPositionThenNpcWaitForASeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        setTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon(transform);
    }

    public delegate void NpcMoveToDungeonEntrance(Transform npc);
    public NpcMoveToDungeonEntrance npcMoveToDungeonEntrance;

    public delegate void SetTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon(Transform npcTransform);
    public SetTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon setTargetAtTargetBuildingActiveSelfFalseOrDieAtDungeon;
}
