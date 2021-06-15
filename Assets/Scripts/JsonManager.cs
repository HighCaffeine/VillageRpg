using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonManager : MonoBehaviour
{
    [SerializeField] private string streamingAssetsPath;
    [SerializeField] private string persistentPath;
    private JsonData jsonData;

    private void Awake()
    {
        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "/GameData.json");
        persistentPath = Path.Combine(Application.persistentDataPath + "/GameData.json");

        StartCoroutine("LoadJson");
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

        yield return null;
    }

    public void Save()
    {
        StartCoroutine("SaveJson");
    }

    IEnumerator SaveJson()
    {
        string jsonString = JsonUtility.ToJson(jsonData);
        File.WriteAllText(persistentPath, jsonString);

        yield return null;
    }
}
