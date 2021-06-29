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
        LoadData();
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

        yield return null;
    }

    private void LoadData()
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
            if (node.layerNumber == (int)GameLayer.building)
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
