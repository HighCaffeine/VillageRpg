using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum GameLayer
{
    ground = 6,//땅
    building = 7,//건물
    road = 8//길
}

[SerializeField]
public enum BuildingType
{
    Shop,
    Environment
}

public class GameData : GenericSingleton<GameData>
{
    public Dictionary<string, string> buildingDictionary; // X_Y로 이름 받아옴
    public Dictionary<string, NpcController> npcDictionary; // 이름으로 데이터 받아옴

    protected override void Awake()
    {
        base.Awake();

        buildingDictionary = new Dictionary<string, string>();
        npcDictionary = new Dictionary<string, NpcController>();
    }
}
