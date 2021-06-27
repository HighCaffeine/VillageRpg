using System;

[Serializable]
public class JsonData
{
    public NpcData[] npcData;
    public EnemyData[] enemyData;
    public BuildingData[] buildingData;
    public ItemData[] itemData;
    public EventData[] eventData;
}

[Serializable]
public class NpcData
{
    public string name;
    public string job;
    public string weapon;
}

[Serializable]
public class EnemyData
{
    public string name;
    public string type;
}

[Serializable]
public class BuildingData
{
    public string name;
    public string saleType;
    public int x;
    public int y;
}

[Serializable]
public class ItemData
{
    public string name;
    public string type;
    public int price;
    public int damage;
}

[Serializable]
public class EventData
{

}
