using System;
using System.Collections.Generic;

[Serializable]
public class JsonData
{
    public List<NpcData> npcData;
    public List<BuildingData> buildingData;
    public WeaponData[] weaponData;
    public EnemyData[] enemyData;
    public ItemData[] itemData;
    public GameInfo[] gameInfo;
    public ArmorData[] armorData;
}

[Serializable]
public class NpcData
{
    public string name;
    public int weaponNumber;
    public int maxHealth;
    public int damage;
    public int armor;
    public int fatigue;
    public int money;
}

[Serializable]
public class EnemyData
{
    public string name;
    public int dropMoney;
    public int health;
    public int damage;
    public string dungeonName;
    public int number;
}

[Serializable]
public class BuildingData
{
    public int x;
    public int y;
    public string buildingName;
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
public class WeaponData
{
    public int weaponNumber;
    public string weaponName;
    public float attackSpeed;
    public int damage;
}

[Serializable]
public class ArmorData
{
    public int armorNumber;
    public string armorName;
    public int armorValue;
}

[Serializable]
public class GameInfo
{
    public int gameInfoMoney;
    public int gameInfoGameSpeed;
    public int gameInfoFrameRate;
    public float gameInfoTime;
}