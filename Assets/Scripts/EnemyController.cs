using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public delegate void EnemyCountPlusOrMinus(int value);
    public EnemyCountPlusOrMinus enemyCountPulsOrMinus;

    //����� npc, ����� npc�� ������ �ٸ��ɷ�(�̰� ���� ���� ����)
    public Transform targetNpcTransform;
    private float waitTime = 10f;
    private Vector3 movePosition;

    public int health;
    public int dropMoney;

    public bool isSpawned;

    private void OnEnable()
    {
        if (isSpawned)
        {
            enemyCountPulsOrMinus(1);

            StartCoroutine(CheckDead());
            StartCoroutine(RandomMove()); 
        }
    }

    private void OnDisable()
    {
        if (isSpawned)
        {
            isSpawned = false;

            enemyCountPulsOrMinus(-1);

            StopCoroutine(CheckDead());
            StopCoroutine(RandomMove());
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

    IEnumerator RandomMove()
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            //enemy waitTime���� ���ݾ� ��ó���� �����̰� �� 
            //������ x-1f~1f z-1f~1f

            yield return new WaitForSeconds(waitTime);
        }
    }
}
