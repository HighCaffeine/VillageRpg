using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 던전 안에있는 enemy의 수와 현재 몇번째 dungeon이 켜져있는지와 dungeon클리어시 보상을 저장해둡니다.

public class DungeonController : MonoBehaviour
{
    public int reward;

    public int nowActiveTrueEnemyCount = 0;
    public int nowActiveDungeonNumber; //던전 끝날때 랜덤 설정
}