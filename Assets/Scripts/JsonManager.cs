using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonManager : MonoBehaviour
{
    [SerializeField] private Astar astar;
    [SerializeField] private string streamingAssetsPath;
    [SerializeField] private string persistentPath;

    public JsonData jsonData;

    private void Awake()
    {
        jsonData = new JsonData();
        astar = GetComponent<Astar>();
        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "//GameData.json");
        persistentPath = Path.Combine(Application.persistentDataPath + "//GameData.json");

        StartCoroutine("LoadJson");
    }

    private void Start()
    {
        LoadNode();
    }

    IEnumerator LoadJson()
    {
        string path = string.Empty;

        if (File.Exists(persistentPath))
        {
            path = persistentPath;
        }
        else
        {
            path = streamingAssetsPath;
        }

        string jsonString = File.ReadAllText(path);
        jsonData = JsonUtility.FromJson<JsonData>(jsonString);

        foreach (var npc in jsonData.npcData)
        {
            GameData.Instance.npcNameList.Add(npc.name);
        }

        int count = 0;

        foreach (var enemy in jsonData.enemyData)
        {
            switch (enemy.dungeonName)
            {
                case "Wood":
                    GameData.Instance.woodDungeonEnemy.Add(enemy.name);
                    break;
                case "Abyss":
                    GameData.Instance.abyssDungeonEnemy.Add(enemy.name);
                    break;
                case "Cellar":
                    GameData.Instance.cellarDungeonEnemy.Add(enemy.name);
                    break;
            }

            GameData.Instance.enemyHealthList.Add(enemy.health);
            GameData.Instance.enemyDropMoneyList.Add(enemy.dropMoney);
            GameData.Instance.enemyNameList.Add(enemy.name);

            count++;
        }

        GameData.Instance.money = jsonData.gameInfo.gameInfoMoney;
        GameData.Instance.gameSpeed = jsonData.gameInfo.gameInfoGameSpeed;

        yield return null;
    }

    private void LoadNode()
    {
        while (!astar.endOfSetNode)
        {
            if (astar.endOfSetNode)
            {
                break;
            }
        }
    
        foreach (var node in astar.GetNode())
        {
            if (node.layerNumber == (int)GameLayer.Building)
            {
                string nodePosToString = $"{node.xPosition}_{node.yPosition}";
    
                if (!GameData.Instance.buildingDictionary.ContainsKey(nodePosToString))
                {
                    GameData.Instance.buildingDictionary.Add(nodePosToString, node.buildingName);
                }
            }
        }
    }

    public void SaveJson()
    {
        string jsonString = JsonUtility.ToJson(jsonData);
        File.WriteAllText(persistentPath, jsonString);
    }
}
