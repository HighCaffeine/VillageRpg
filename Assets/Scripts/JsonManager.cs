using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Google Sheets로 만든 Json파일을 읽고 저장하는 곳입니다.
//읽은 파일을 싱글톤인 GameData에 저장해줍니다.

public class JsonManager : MonoBehaviour
{
    //[SerializeField] private Astar astar;
    [SerializeField] private string streamingAssetsPath;
    [SerializeField] private string persistentPath;

    public JsonData jsonData;

    private void Start()
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

        jsonData.buildingData = new List<BuildingData>();
        jsonData.npcData = new List<NpcData>();
        
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

        foreach (var npcData in jsonData.npcData)
        {
            GameData.Instance.npcNameList.Add(npcData.name);
            GameData.Instance.npcDataDictionary.Add(npcData.name, npcData);
        }

        foreach (var weaponData in jsonData.weaponData)
        {
            GameData.Instance.weaponDataDictionary.Add(weaponData.weaponNumber, weaponData);
        }

        foreach (var armorData in jsonData.armorData)
        {
            GameData.Instance.armorDataDictionary.Add(armorData.armorNumber, armorData);
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

            string[] names = enemyData.name.Split('_');

            GameData.Instance.enemyDictionary.Add(names[0], enemyData);
        }

        foreach (var buildingData in jsonData.buildingData)
        {
            string key = buildingData.x + "_" + buildingData.y;

            GameData.Instance.buildingDataList.Add(key, buildingData.buildingName);
        }

        GameData.Instance.gameSpeed = jsonData.gameInfo[0].gameInfoGameSpeed;
        GameData.Instance.frameRate = jsonData.gameInfo[0].gameInfoFrameRate;
        GameData.Instance.money = jsonData.gameInfo[0].gameInfoMoney;

        yield return null;
    }

    private bool isNotDone = true;

    public bool SaveIsDone()
    {
        return isNotDone;
    }

    public void IsNotDoneFalseIntoTrue()
    {
        isNotDone = true;
    }

    public void SaveToJson()
    {
        jsonData.buildingData.Clear();
        jsonData.npcData.Clear();

        foreach (var data in GameData.Instance.buildingDataList)
        {
            BuildingData buildingData = new BuildingData();

            string[] key = data.Key.Split('_');

            buildingData.x = int.Parse(key[0]);
            buildingData.y = int.Parse(key[1]);
            buildingData.buildingName = data.Value;

            jsonData.buildingData.Add(buildingData);
        }

        foreach (var npcData in GameData.Instance.npcDataDictionary)
        {
            jsonData.npcData.Add(GameData.Instance.npcDataDictionary[npcData.Key]);
        }

        jsonData.gameInfo[0].gameInfoGameSpeed = GameData.Instance.gameSpeed;
        jsonData.gameInfo[0].gameInfoMoney = GameData.Instance.money;
        jsonData.gameInfo[0].gameInfoFrameRate = GameData.Instance.gameSpeed;
        jsonData.gameInfo[0].gameInfoTime = GameData.Instance.time;

        string jsonString = JsonUtility.ToJson(jsonData);
        File.WriteAllText(persistentPath, jsonString);

        isNotDone = false;
    }
}
