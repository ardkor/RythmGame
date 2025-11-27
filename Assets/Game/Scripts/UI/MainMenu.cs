using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    /*[SerializeField] private GameManager _gameManager;
    [SerializeField] private CoinCounter _coinCounter;*/
    [SerializeField] private RecordTable _scoreTable;

    [SerializeField] private Button _song1;
    [SerializeField] private Button _song2;
    [SerializeField] private Button _song3;

    [SerializeField] private Button _easy;
    [SerializeField] private Button _medium;
    [SerializeField] private Button _hard;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _recordsTableButton;
    [SerializeField] private Button _recordsTableCloseButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        UnityAction onClickQuit = () =>
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        };
        _song1.onClick.AddListener(() =>
        {
            G.audio.PlaySongInMenu(0, 5);
            G.gameManager.SetupSong(0);
        });
        _song2.onClick.AddListener(() =>
        {
            G.audio.PlaySongInMenu(1, 5);
            G.gameManager.SetupSong(1);
        });
        _song3.onClick.AddListener(() =>
        {
            G.audio.PlaySongInMenu(2, 5);
            G.gameManager.SetupSong(2);
        });
        _easy.onClick.AddListener(() => G.gameManager.SetupDifficulty(0));
        _medium.onClick.AddListener(() => G.gameManager.SetupDifficulty(1));
        _hard.onClick.AddListener(() => G.gameManager.SetupDifficulty(2));

        _startButton.onClick.AddListener(G.gameManager.StartGame);
        _startButton.onClick.AddListener(Close);
        _exitButton.onClick.AddListener(onClickQuit.Invoke);
        _recordsTableButton.onClick.AddListener(_scoreTable.Open);
        _recordsTableCloseButton.onClick.AddListener(_scoreTable.Close);
    }

    /*private void OnDisable()
    {
        _startButton.onClick.RemoveListener(_coinCounter.Open);
        _startButton.onClick.RemoveListener(_gameManager.StartGame);
        _startButton.onClick.RemoveListener(Close);
        _exitButton.onClick.RemoveListener(_gameManager.Exit);
        _recordsTableButton.onClick.RemoveListener(_scoreTable.Open);
        _recordsTableCloseButton.onClick.RemoveListener(_scoreTable.Close);
    }
    */

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}