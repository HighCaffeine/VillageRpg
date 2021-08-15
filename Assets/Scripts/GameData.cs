using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] 
public enum Dungeons
{
    Wood,
    Abyss,
    Cellar
}

[SerializeField]
public enum BuildingAllow
{
    BuildingAllowded,
    BuildingNotAllowded
}

[SerializeField]
public enum GameLayer
{
    Ground = 6,//땅
    Building = 7,//건물
    Road = 8,//길
    Dungeon = 9//던전
}

[SerializeField]
public enum BuildingType
{
    Shop,
    Environment,
    Dungeon
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
    //public Dictionary<string, int> buildingDictionary; // X_Y로 몇번째 켜져있는지 받아옴

    public List<string> woodDungeonEnemy;
    public List<string> abyssDungeonEnemy;
    public List<string> cellarDungeonEnemy;

    public Dictionary<string, EnemyData> enemyDataDictionray;

    public Dictionary<string, Transform> npcTransformDictionary;
    public List<string> npcNameList; // 이름으로 데이터 받아옴
    public Dictionary<string, NpcData> npcDataDictionary;

    public Dictionary<int, WeaponData> weaponDataDictionary;

    public int gameSpeed;
    public int money;
    public int frameRate;

    protected override void Awake()
    {
        base.Awake();

        woodDungeonEnemy = new List<string>();
        abyssDungeonEnemy = new List<string>();
        cellarDungeonEnemy = new List<string>();

        enemyDataDictionray = new Dictionary<string, EnemyData>();
        weaponDataDictionary = new Dictionary<int, WeaponData>();

        npcTransformDictionary = new Dictionary<string, Transform>();
        npcNameList = new List<string>();
        npcDataDictionary = new Dictionary<string, NpcData>();

    }
}
