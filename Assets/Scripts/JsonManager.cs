using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonManager : MonoBehaviour
{
    //[SerializeField] private Astar astar;
    [SerializeField] private string streamingAssetsPath;
    [SerializeField] private string persistentPath;

    public JsonData jsonData;

    private void Awake()
    {
        jsonData = new JsonData();
        //astar = GetComponent<Astar>();
        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "//GameData.json");
        persistentPath = Path.Combine(Application.persistentDataPath + "//GameData.json");

        StartCoroutine(LoadJson());
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

        Debug.Log(jsonString);

        foreach (var npcData in jsonData.npcData)
        {
            GameData.Instance.npcNameList.Add(npcData.name);
            GameData.Instance.npcDataDictionary.Add(npcData.name, npcData);
        }

        foreach (var weaponData in jsonData.weaponData)
        {
            GameData.Instance.weaponDataDictionary.Add(weaponData.weaponNumber, weaponData);
        }

        foreach (var enemyData in jsonData.enemyData)
        {
            switch (enemyData.dungeonName)
            {
                case "Wood":
                    GameData.Instance.woodDungeonEnemy.Add(enemyData.name);
                    break;
                case "Abyss":
                    GameData.Instance.abyssDungeonEnemy.Add(enemyData.name);
                    break;
                case "Cellar":
                    GameData.Instance.cellarDungeonEnemy.Add(enemyData.name);
                    break;
            }

            GameData.Instance.enemyDataDictionray.Add(enemyData.name, enemyData);
        }

        GameData.Instance.gameSpeed = jsonData.gameInfo[0].gameInfoGameSpeed;
        GameData.Instance.frameRate = jsonData.gameInfo[0].gameInfoFrameRate;
        GameData.Instance.money = jsonData.gameInfo[0].gameInfoMoney;

        yield return null;
    }

    public void SaveJson()
    {
        string jsonString = JsonUtility.ToJson(jsonData);
        File.WriteAllText(persistentPath, jsonString);
    }
}
