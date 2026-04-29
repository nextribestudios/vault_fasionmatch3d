using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class LevelGeneratorController : MonoBehaviour
{
    public LevelInfo levelInfo;

    public void SerializeToJson()
    {
        Debug.Log(Application.dataPath);
        string jsonData = JsonConvert.SerializeObject(levelInfo);
        string jsonPath = Application.dataPath + "/Resources/Json/" + levelInfo.LevelName + ".json";
        File.WriteAllText(jsonPath, jsonData);
        Debug.Log("level json created!!!");
    }
}

[System.Serializable]
public class LevelInfo
{
    public string LevelName;
    public int LevelNo;
    public float timer;
    public int totalItemCount;
    public bool isHardLevel;
    public int[] LevelItems;
    public int[] LevelItemsCount;
    public int totalCounterCount;
    public int[] CounterItem;
    public int[] CounterItemCount;
}
