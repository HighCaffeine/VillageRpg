using System;

[Serializable]
public class JsonData
{
    public NpcData[] npcData;
    public EnemyData[] enemyData;
    public BuildingData[] buildingData;
    public ItemData[] itemData;
    public EventData[] eventData;
    public GameInfo[] gameInfo;
}

[Serializable]
public class NpcData
{
    public string name;
    public string weapon;
    public int maxHealth;
    public int damage;
    public int armor;
    public int fatigue;
}

[Serializable]
public class EnemyData
{
    public string name;
    public int dropMoney;
    public int health;
    public string dungeonName;
    public int number;
}

[Serializable]
public class BuildingData
{
    public int x;
    public int y;
    public int activeNumber;
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

[Serializable]
public class GameInfo
{
    public int gameInfoMoney;
    public int gameInfoGameSpeed;
    public int gameInfoFrameRate;
}
