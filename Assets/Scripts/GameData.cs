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

[SerializeField]
public enum BuildingName
{
    ArmorShop,
    WeaponsShop,
    FoodShop,
    Platform,
    Tree,
    Fence,
    FenceCurve
}

public class GameData : GenericSingleton<GameData>
{
    public Dictionary<string, string> buildingDictionary; // X_Y로 이름 받아옴

    public Dictionary<string, Transform> npcTransformDictionary;
    public List<string> npcNameList; // 이름으로 데이터 받아옴

    protected override void Awake()
    {
        base.Awake();

        npcTransformDictionary = new Dictionary<string, Transform>();
        buildingDictionary = new Dictionary<string, string>();
        npcNameList = new List<string>();
    }
}
