using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public delegate void EnemyCountPlusOrMinus(int value);
    public EnemyCountPlusOrMinus enemyCountPulsOrMinus;

    public delegate Node GetNodeByPosition(Vector3 pos);
    public GetNodeByPosition getNodeByPosition;

    public Vector3 parentNodePos;

    //����� npc, ����� npc�� ������ �ٸ��ɷ�(�̰� ���� ���� ����)
    public Transform targetNpcTransform;
    [SerializeField] private float waitTime = 10f;
    private Vector3 movePosition;

    public int health;
    public int dropMoney;

    public bool isSpawned;

    public int parentXPos;
    public int parentYPos;

    private void OnEnable()
    {
        if (isSpawned)
        {
            enemyCountPulsOrMinus(1);

            StartCoroutine(CheckDead());
            StartCoroutine(MoveToRandomPos()); 
        }
    }

    private void OnDisable()
    {
        if (isSpawned)
        {
            isSpawned = false;

            enemyCountPulsOrMinus(-1);

            StopCoroutine(CheckDead());
            StopCoroutine(MoveToRandomPos());
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

    IEnumerator MoveToRandomPos()
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            //enemy waitTime���� ���ݾ� ��ó���� �����̰� �� 
            //������ x-1f~1f z-1f~1f
            //��尡 �ִٸ� �� ��������� ��ó�� �ϴ°ɷ�

            Node node;
            Vector3 addPos;
            Vector3 newPos;

            do
            {
                addPos = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                node = getNodeByPosition(parentNodePos + addPos);
            } 
            while (node == null);

            newPos = addPos + node.nodePosition;

            transform.LookAt(newPos);

            while (Vector3.Distance(newPos, transform.position) > 0.1f)
            {
                transform.Translate(Vector3.forward * 0.05f * GameData.Instance.gameSpeed);

                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(waitTime);
        }
    }
}
