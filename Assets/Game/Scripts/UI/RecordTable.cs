using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordTable : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _recordPrefab;

    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;

    public void Open()
    {
        LoadRecords();
        UpdateHeight();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        ClearTable();
        gameObject.SetActive(false);
    }

    private void ClearTable()
    {
        int records = _content.transform.childCount;
        for (int i = 0; i < records; i++)
        {
            Transform record = _content.transform.GetChild(i);
            Destroy(record.gameObject);
        }
    }

    private void LoadRecords()
    {
        JsonSerializer jsonSerializer = new JsonSerializer();
        List<Record> records = jsonSerializer.LoadJson();
        records.Sort((r1, r2) => r2.score.CompareTo(r1.score));
        GameObject nameText = Instantiate(_recordPrefab, _content.transform);
        nameText.GetComponent<TMP_Text>().text = "Nick";
        GameObject scoreText = Instantiate(_recordPrefab, _content.transform);
        scoreText.GetComponent<TMP_Text>().text = "Score";
        GameObject accuracyText = Instantiate(_recordPrefab, _content.transform);
        accuracyText.GetComponent<TMP_Text>().text = "Accuracy";
        GameObject maxCombosText = Instantiate(_recordPrefab, _content.transform);
        maxCombosText.GetComponent<TMP_Text>().text = "Max Combos";
        foreach (var record in records)
        {
            GameObject nameRecord = Instantiate(_recordPrefab, _content.transform);
            nameRecord.GetComponent<TMP_Text>().text = record.name;
            GameObject scoreRecord = Instantiate(_recordPrefab, _content.transform);
            scoreRecord.GetComponent<TMP_Text>().text = record.score.ToString();
            GameObject accuracyRecord = Instantiate(_recordPrefab, _content.transform);
            accuracyRecord.GetComponent<TMP_Text>().text = record.accuracy.ToString();
            GameObject maxCombosRecord = Instantiate(_recordPrefab, _content.transform);
            maxCombosRecord.GetComponent<TMP_Text>().text = record.combos.ToString();
        }
    }

    private void UpdateHeight()
    {
        _rectTransform = _content.GetComponent<RectTransform>();
        _gridLayoutGroup = _content.GetComponent<GridLayoutGroup>();
        int records = _content.transform.childCount;
        float linesCount = records % 2 == 0 ? records / 2f : records / 2f + 1;
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, linesCount * _gridLayoutGroup.cellSize.y);
    }
}