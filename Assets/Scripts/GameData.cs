using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum GameLayer
{
    ground = 6,//��
    building = 7,//�ǹ�
    road = 8//��
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
    public Dictionary<string, string> buildingDictionary; // X_Y�� �̸� �޾ƿ�

    public Dictionary<string, Transform> npcTransformDictionary;
    public List<string> npcNameList; // �̸����� ������ �޾ƿ�

    protected override void Awake()
    {
        base.Awake();

        npcTransformDictionary = new Dictionary<string, Transform>();
        buildingDictionary = new Dictionary<string, string>();
        npcNameList = new List<string>();
    }
}
