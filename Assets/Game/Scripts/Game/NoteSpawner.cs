using UnityEngine;
using UnityEngine.UI;

public class NoteSpawner : MonoBehaviour
{
    //public RhythmTrackAnalyzer analyzer;
    [SerializeField] private OfflineAudioAnalyzer _offlineAudioAnalyzer;

    public Transform[] lanes = new Transform[4]; // точки спавна по дорожкам
    public Sprite[] lanesSprites = new Sprite[4];
    public GameObject tapNotePrefab;
    public GameObject holdNotePrefab;

    private int difficulty;
    private int clipIndex;

    float lastSpawnTime;
    float audioTime;

    float baseIntervalEasy = 1.2f;
    float baseIntervalNormal = 0.9f;
    float baseIntervalHard = 0.7f;

    float holdChanceEasy = 0.05f;
    float holdChanceNormal = 0.08f;
    float holdChanceHard = 0.1f;
    private bool active = false;

    public void Init(int difficulty, int clipIndex)
    {
        this.difficulty = difficulty;
        this.clipIndex = clipIndex;
        audioTime = 0;
        active = true;
    }

    public void Disable()
    {
        active = false;
    }
    void Update()
    {
        if (!active)
            return;
        audioTime += Time.deltaTime;
        if (!_offlineAudioAnalyzer.IsTimeToSpawnNoteFromOfflineData(clipIndex, audioTime))
            return;

        float difficultyInterval = GetSpawnInterval();
        float difficultyHoldChance = GetHoldChance();

        if (Time.time - lastSpawnTime < difficultyInterval)
            return;

        lastSpawnTime = Time.time;

        // Выбор дорожки
        int lane = GetLaneByDifficulty();

        // Выбор типа ноты
        NoteType type = Random.value < difficultyHoldChance
            ? NoteType.Hold
            : NoteType.Tap;

        SpawnNote(type, lane);
    }

    float GetSpawnInterval()
    {
        return difficulty switch
        {
            0 => baseIntervalEasy,
            1 => baseIntervalNormal,
            2 => baseIntervalHard,
            _ => baseIntervalNormal
        };
    }

    float GetHoldChance()
    {
        return difficulty switch
        {
            0 => holdChanceEasy,
            1 => holdChanceNormal,
            2 => holdChanceHard,
            _ => 0.1f
        };
    }

    int GetLaneByDifficulty()
    {
        return difficulty switch
        {
            0 => Random.Range(0, 4),
            1 => Random.Range(0, 4),
            2 => Random.Range(0, 4),
            _ => Random.Range(0, 4)
        };
    }

    void SpawnNote(NoteType type, int lane)
    {
        GameObject prefab = type == NoteType.Tap ? tapNotePrefab : holdNotePrefab;

        GameObject block = Instantiate(prefab, lanes[lane]);
        block.GetComponent<Image>().sprite = lanesSprites[lane];
    }
}