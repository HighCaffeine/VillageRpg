using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enemy�� ���� ������ �ֽ��ϴ�.
//�������� �׾������ manager���� �ڽ��� �׾��ٰ� �˸��� delegate�� �����մϴ�.
public class EnemyController : MonoBehaviour
{
    public delegate Node GetNodeByPosition(Vector3 pos, bool isDungeon, string dungeonName);
    public GetNodeByPosition getNodeByPosition;

    public delegate void CalculateEnemyCountInDungeon(int dungeonNumber, int value);
    public CalculateEnemyCountInDungeon calculateEnemyCountInDungeon;

    //����� npc, ����� npc�� ������ �ٸ��ɷ�(�̰� ���� ���� ����)
    public bool endOfPooling;

    public int myNumber;

    public int health;
    public int dropMoney;
    public int damage;

    public int nowDungeonParentNumber;

    public List<Transform> targetInDungeon;

    public List<Animator> enemyAnimatorList;

    public int xPos;
    public int yPos;

    private void Awake()
    {
        targetInDungeon = new List<Transform>();
    }

    private void OnEnable()
    {
        if (endOfPooling)
        {
            calculateEnemyCountInDungeon(nowDungeonParentNumber, 1);
        }
    }

    public void Die()
    {
        transform.gameObject.SetActive(false);
        targetInDungeon.RemoveAt(0);

        addMoney(dropMoney);
        calculateEnemyCountInDungeon(nowDungeonParentNumber, -1);
        removeEnemyFromDungeonEnemyList(transform);

        transform.GetChild(myNumber).gameObject.SetActive(false);

        enemyAnimatorList[myNumber].ResetTrigger("Attack");

        foreach (var npc in targetInDungeon)
        {
            setNewTargetInDungeonRequestToActiveNpc(npc);
        }

        setEnemyNodeArrayOneIntoZero(xPos, yPos);
    }

    public void RemoveNpcFromTargetList(Transform npc)
    {
        targetInDungeon.Remove(npc);
    }

    public delegate void SetEnemyNodeArrayOneIntoZero(int xPos, int yPos);
    public SetEnemyNodeArrayOneIntoZero setEnemyNodeArrayOneIntoZero;

    public delegate void RemoveEnemyFromDungeonEnemyList(Transform enemy);
    public RemoveEnemyFromDungeonEnemyList removeEnemyFromDungeonEnemyList;
    public delegate void SetNewTargetInDungeonRequestToActiveNpc(Transform target);
    public SetNewTargetInDungeonRequestToActiveNpc setNewTargetInDungeonRequestToActiveNpc;

    public delegate void AddMoney(int value);
    public AddMoney addMoney;
}
