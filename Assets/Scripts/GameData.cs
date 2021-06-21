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
    public Dictionary<Node, string> buildingDictionary; // Node�� �̸� �޾ƿ�
    public Dictionary<string, NpcController> npcDictionary; // �̸����� ������ �޾ƿ�

    protected override void Awake()
    {
        base.Awake();

        buildingDictionary = new Dictionary<Node, string>();
        npcDictionary = new Dictionary<string, NpcController>();
    }
}
