using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    //이걸로 던전 클리어 판정 시켜줄거임

    public int reward;

    public int nowActiveTrueEnemyCount = 0;
    public int nowActiveDungeonNumber; //던전 끝날때 랜덤 설정

    private void OnDisable()
    {
        nowActiveDungeonNumber = Random.Range(0, 3);
    }
}