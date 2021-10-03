using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enemyï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ö½ï¿½ï¿½Ï´ï¿½.
//ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½×¾ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ managerï¿½ï¿½ï¿½ï¿½ ï¿½Ú½ï¿½ï¿½ï¿½ ï¿½×¾ï¿½ï¿½Ù°ï¿½ ï¿½Ë¸ï¿½ï¿½ï¿½ delegateï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Õ´Ï´ï¿½.
public class EnemyController : MonoBehaviour
{
    public delegate Node GetNodeByPosition(Vector3 pos, bool isDungeon, string dungeonName);
    public GetNodeByPosition getNodeByPosition;

    public delegate void CalculateEnemyCountInDungeon(int dungeonNumber, int value);
    public CalculateEnemyCountInDungeon calculateEnemyCountInDungeon;

    //°¡±î¿î npc, °¡±î¿î npc°¡ Á×À¸¸é ´Ù¸¥°É·Î(ÀÌ°Å ¹»·Î ÇÒÁö °áÁ¤)
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
