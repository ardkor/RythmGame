using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text phraseText;
    [SerializeField] private TMP_Text comboText;

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdatePhrase(string phrase)
    {
        phraseText.text = phrase;
    }
    public void UpdateCombo(string combo)
    {
        comboText.text = combo;
    }
}