using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public Transform npcTransform;

    public delegate Node GetNodeByPosition(Vector3 position, bool isDungeon, string dungeonName);
    public GetNodeByPosition getNodeByPosition;

    public Animator npcAnimator;
    public Vector3 target; // gameManagerø°º≠ ¡§«ÿ¡‹
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
    public bool playAnimation = false;
    public string itemBuyType;


    private void OnEnable()
    {
        if (!npcGoToDungeon)
        {
            StartCoroutine(CheckTargetIsActiveTrue());
        }
    }

    private void OnDisable()
    {
        if (!npcGoToDungeon)
        {
            StopCoroutine(CheckTargetIsActiveTrue());
        }
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
            if (getNodeByPosition(target, false, null).nodeTransform == null)
            {
                setTargetAtTargetBuildingActiveSelfFalse(transform);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void SetDungeonValueTurnOff()
    {
        npcGoToDungeon = false;
        arrivedDungeon = false;
    }

    public delegate void SetTargetAtTargetBuildingActiveSelfFalse(Transform npcTransform);
    public SetTargetAtTargetBuildingActiveSelfFalse setTargetAtTargetBuildingActiveSelfFalse;


    public delegate void GoToTargetInDungeon(Transform npc, Transform target);

    public delegate void AttackEveryDelay(Transform mySelf, Transform target, bool isNpc);
    public AttackEveryDelay attackEveryDelay;
}
