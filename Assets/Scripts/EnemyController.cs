using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public delegate Node GetNodeByPosition(Vector3 pos);
    public GetNodeByPosition getNodeByPosition;

    public delegate void CalculateEnemyCountInDungeon(int dungeonNumber, int value);
    public CalculateEnemyCountInDungeon calculateEnemyCountInDungeon;

    public Animator enemyAnimator;

    //가까운 npc, 가까운 npc가 죽으면 다른걸로(이거 뭘로 할지 결정)
    public Transform targetNpcTransform;
    [SerializeField] private float waitTime = 10f;

    public int health;
    public int dropMoney;

    public bool isSpawned;

    public int nowDungeonParentNumber;

    public Transform targetInDungeon;

    public int damage;

    private void OnEnable()
    {
        if (isSpawned)
        {
            calculateEnemyCountInDungeon(nowDungeonParentNumber, 1);

            StartCoroutine(CheckDead());
        }
    }

    private void OnDisable()
    {
        if (isSpawned)
        {
            setNewTargetInDungeonRequestFromActiveFalseNpc(targetInDungeon, true);
            isSpawned = false;

            calculateEnemyCountInDungeon(nowDungeonParentNumber, -1);

            StopCoroutine(CheckDead());
        }
    }

    IEnumerator CheckDead()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        targetNpcTransform = null;
        transform.gameObject.SetActive(false);

        yield return null;
    }

    //공격만들거

    public void Die()
    {
        transform.gameObject.SetActive(false);
    }

    public delegate void AttackEveryDelay(Transform mySelf, Transform target, bool isNpc);
    public AttackEveryDelay attackEveryDelay;

    public delegate void RemoveEnemyFromDungeonEnemyList(Transform enemy);
    public RemoveEnemyFromDungeonEnemyList removeEnemyFromDungeonEnemyList;

    public delegate void SetNewTargetInDungeonRequestFromActiveFalseNpc(Transform target, bool targetIsNpc);
    public SetNewTargetInDungeonRequestFromActiveFalseNpc setNewTargetInDungeonRequestFromActiveFalseNpc;
}
