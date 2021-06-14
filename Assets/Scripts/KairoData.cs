using UnityEngine;

[SerializeField]
public class KairoData
{
    public NpcData[] npcData;
    public EnemyData[] enemyData;
    public BuildingData[] buildingData;
    public ItemData[] itemData;
    public EventData[] eventData;
}

[SerializeField]
public class NpcData
{
    public string name;
    public string job;
    public string weapon;
}

[SerializeField]
public class EnemyData
{
    public string name;
    public string type;
}

[SerializeField]
public class BuildingData
{
    public string name;
    public string saleType;
    //노드기준
    public int x;
    public int y;
}

[SerializeField]
public class ItemData
{
    public string name;
    public string type;
}

[SerializeField]
public class EventData
{

}
