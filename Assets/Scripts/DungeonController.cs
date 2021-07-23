using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public int beforeChildCount;

    //이걸로 던전 클리어 판정 시켜줄거임
    public int activeTrueEnemyCount;
    public string dungeonName;

    //얘를 activeTrueEnemyCount가 0일때 끄고 parent의 이름을 0~2중 아무거나 정해줌

    public void StartDungeonActiveselfWhenEnemyCountZero()
    {

    }

    //IEnumerator CheckEnemyActiveSelf()
    //{

    //}
}
