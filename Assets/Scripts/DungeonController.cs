using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    //�̰ɷ� ���� Ŭ���� ���� �����ٰ���

    public int reward;

    public int nowActiveTrueEnemyCount = 0;
    public int nowActiveDungeonNumber; //���� ������ ���� ����

    private void OnDisable()
    {
        nowActiveDungeonNumber = Random.Range(0, 3);
    }
}