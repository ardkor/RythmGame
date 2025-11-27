using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecordsData
{
    public Record[] records;
}
[System.Serializable]
public class Record
{
    public string name;
    public int score;
    public int accuracy;
    public int combos;

    public Record(string name, int score, int accuracy, int combos)
    {
        this.name = name;
        this.score = score;
        this.accuracy = accuracy;
        this.combos = combos;
    }
}
