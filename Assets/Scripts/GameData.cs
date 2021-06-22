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

public class GameData : GenericSingleton<GameData>
{
    public Dictionary<string, string> buildingDictionary; // X_Y�� �̸� �޾ƿ�
    public Dictionary<string, NpcController> npcDictionary; // �̸����� ������ �޾ƿ�

    protected override void Awake()
    {
        base.Awake();

        buildingDictionary = new Dictionary<string, string>();
        npcDictionary = new Dictionary<string, NpcController>();
    }
}
