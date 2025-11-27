using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.IO;

public class JsonSerializer
{
    private const string fileName = "records.json";

    public void SaveJson(RecordsData records)
    {
        string json = JsonUtility.ToJson(records);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filePath, json);
    }

    public List<Record> LoadJson()
    {
        List<Record> records = new List<Record>();
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            RecordsData recordsData = JsonUtility.FromJson<RecordsData>(json);
            records.AddRange(recordsData.records);
        }
        else
        {
            Debug.LogWarning("Файл JSON не найден!");
        }

        return records;
    }
}