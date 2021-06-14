using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonData : MonoBehaviour
{
    [SerializeField] private string streamingAssetsPath;
    [SerializeField] private string persistentPath;
    private KairoData kairoData;

    private void Awake()
    {
        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "KairoGameData.json");
        persistentPath = Path.Combine(Application.persistentDataPath + "KairoGameData.json");

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
        kairoData = JsonUtility.FromJson<KairoData>(jsonString);

        Debug.Log(kairoData);
        
        yield return null;
    }

    public void Save()
    {
        StartCoroutine("SaveJson");
    }

    IEnumerator SaveJson()
    {
        string jsonString = JsonUtility.ToJson(kairoData);
        File.WriteAllText(persistentPath, jsonString);

        yield return null;
    }
}
