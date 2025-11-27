using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class EndGameMenu : MonoBehaviour
{
    [SerializeField] private MainMenu _mainMenu;

    /*[SerializeField] private Player _player;
    [SerializeField] private CoinCounter _coinCounter;*/

    [SerializeField] private Button _saveScoreButton;
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private TMP_Text _accuracy;
    [SerializeField] private TMP_Text _combos;
    [SerializeField] private TMP_Text _score;
    private int _scoreValue;
    private int _accuracyValue;
    private int _combosValue;
    private void OnEnable()
    {
        _saveScoreButton.onClick.AddListener(SaveScore);
        _saveScoreButton.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        _saveScoreButton.onClick.RemoveListener(SaveScore);
        _saveScoreButton.onClick.RemoveListener(Close);
    }

    public void Activate(int score, int accuracy, int combos)
    {
        gameObject.SetActive(true);
        SetValues(score, accuracy, combos);
    }

    private void SetValues(int score, int accuracy, int combos)
    {
        _scoreValue = score;
        _accuracyValue = accuracy;
        _combosValue = combos;
        _score.text = score.ToString();
        _accuracy.text = accuracy.ToString();
        _combos.text = combos.ToString();
    }
    private void SaveScore()
    {
        JsonSerializer jsonSerializer = new JsonSerializer();
        List<Record> records = jsonSerializer.LoadJson();
        records.Add(new Record(_input.text, _scoreValue, _accuracyValue, _combosValue));
        RecordsData recordsData = new RecordsData();
        recordsData.records = records.ToArray();
        jsonSerializer.SaveJson(recordsData);
    }

    private void Close()
    {
        _mainMenu.Open();
        gameObject.SetActive(false);
    }
}