using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public delegate Node GetNodeByPosition(Vector3 pos);
    public GetNodeByPosition getNodeByPosition;

    public delegate void CalculateEnemyCountInDungeon(int dungeonNumber, int value);
    public CalculateEnemyCountInDungeon calculateEnemyCountInDungeon;

    //����� npc, ����� npc�� ������ �ٸ��ɷ�(�̰� ���� ���� ����)
    public Transform targetNpcTransform;
    [SerializeField] private float waitTime = 10f;

    public int health;
    public int dropMoney;

    public bool isSpawned;

    public int nowDungeonParentNumber;

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

    //���ݸ����
}
