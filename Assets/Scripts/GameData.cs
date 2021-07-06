using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] 
public enum Dungeons
{
    Abyss,
    Wood,
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
    public Dictionary<string, string> buildingDictionary; // X_Y�� �̸� �޾ƿ�

    public List<int> enemyHealthList;
    public List<int> enemyDropMoneyList;
    public List<string> enemyNameList;

    public Dictionary<string, Transform> npcTransformDictionary;
    public List<string> npcNameList; // �̸����� ������ �޾ƿ�

    public int gameSpeed;

    protected override void Awake()
    {
        base.Awake();

        enemyHealthList = new List<int>();
        enemyDropMoneyList = new List<int>();
        enemyNameList = new List<string>();

        npcTransformDictionary = new Dictionary<string, Transform>();
        npcNameList = new List<string>();

        buildingDictionary = new Dictionary<string, string>();
    }
}
