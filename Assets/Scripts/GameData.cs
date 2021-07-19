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
    BuildingAllowed,
    BuildingNotAllowed
}

[SerializeField]
public enum GameLayer
{
    Ground = 6,//��
    Building = 7,//�ǹ�
    Road = 8,//��
    Dungeon = 9//����
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
    public Dictionary<string, int> buildingDictionary; // X_Y�� ���° �����ִ��� �޾ƿ�

    public List<string> woodDungeonEnemy;
    public List<string> abyssDungeonEnemy;
    public List<string> cellarDungeonEnemy;

    public List<int> enemyHealthList;
    public List<int> enemyDropMoneyList;
    public List<string> enemyNameList;

    public Dictionary<string, Transform> npcTransformDictionary;
    public List<string> npcNameList; // �̸����� ������ �޾ƿ�

    public int gameSpeed;
    public int money;
    public int frameRate;

    public List<int> dungeonActiveNumber;

    protected override void Awake()
    {
        base.Awake();

        woodDungeonEnemy = new List<string>();
        abyssDungeonEnemy = new List<string>();
        cellarDungeonEnemy = new List<string>();

        enemyHealthList = new List<int>();
        enemyDropMoneyList = new List<int>();
        enemyNameList = new List<string>();

        npcTransformDictionary = new Dictionary<string, Transform>();
        npcNameList = new List<string>();

        buildingDictionary = new Dictionary<string, int>();

        dungeonActiveNumber = new List<int>();
    }
}
