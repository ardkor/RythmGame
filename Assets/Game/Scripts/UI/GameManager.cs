using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private EndGameMenu _endGameMenu;
    [SerializeField] private RhythmTrackAnalyzer _rhythmTrackAnalyzer;
    [SerializeField] private OfflineAudioAnalyzer _offlineAudioAnalyzer;
    [SerializeField] private NoteSpawner _noteSpawner;
    [SerializeField] private GUIManager _guiManager;

    [SerializeField] private Button line1Button;
    [SerializeField] private Button line2Button;
    [SerializeField] private Button line3Button;
    [SerializeField] private Button line4Button;

    [SerializeField] private Transform spawner1;
    [SerializeField] private Transform spawner2;
    [SerializeField] private Transform spawner3;
    [SerializeField] private Transform spawner4;

    private int difficulty = 1;
    private int songIndex = 1;
    private bool gameActive = false;

    void Awake()
    {
        G.gameManager = this;
    }

    public void SetupSong(int songIndex)
    {
        this.songIndex = songIndex;
    }

    public void SetupDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }

    public void StartGame()
    {
        gameActive = true;
        G.audio.PlaySongInGame(songIndex, 6.3f);
        _offlineAudioAnalyzer.InitPeakIndices();
        //_rhythmTrackAnalyzer.Initialize(G.audio.CurrentMutedMusicSource);
        _noteSpawner.Init(difficulty, songIndex);
        StartCoroutine(EndSong(G.audio.GetSongDuration(songIndex) + 6.3f));
        _guiManager.UpdateScore(0);
        _guiManager.UpdatePhrase("");
        _guiManager.UpdateCombo("");
    }

    private IEnumerator EndSong(float delay)
    {
        yield return new WaitForSeconds(delay);
        _noteSpawner.Disable();
        _endGameMenu.Activate(_scoreValue, (int)(_accuracySuccess * 100f / _accuracyTriesCount), _combosValue);
        gameActive = false;
        _scoreValue = 0;
        _accuracySuccess = 0;
        _accuracyTriesCount = 0;
        _combosValue = 0;
    }

    private int _scoreValue = 0;
    private int _accuracySuccess = 0;
    private int _accuracyTriesCount = 0;
    private int _combosValue = 0;

    List<RectTransform> GetNonHoldBlocks(Transform spawner, bool hold = false)
    {
        List<RectTransform> blocks = new List<RectTransform>();
        foreach (RectTransform rt in spawner.GetComponentsInChildren<RectTransform>())
        {
            if (!hold && rt.GetComponent<IsHoldBlock>() == null
                      && rt.GetComponent<FallingBlock>() != null && !rt.GetComponent<FallingBlock>().IsUsed)
                blocks.Add(rt);
            if (hold && rt.GetComponent<IsHoldBlock>() != null && rt.GetComponent<FallingBlock>() != null &&
                !rt.GetComponent<FallingBlock>().IsUsed)
                blocks.Add(rt);
        }

        return blocks;
    }

    private RectTransform line1CurrentHold;
    private RectTransform line2CurrentHold;
    private RectTransform line3CurrentHold;
    private RectTransform line4CurrentHold;
    private float holdBlockCenterShift = 50f;

    private void Update()
    {
        if (!gameActive)
            return;
        var pointer = new PointerEventData(EventSystem.current);
        List<RectTransform> line1Blocks = GetNonHoldBlocks(spawner1);
        List<RectTransform> line2Blocks = GetNonHoldBlocks(spawner2);
        List<RectTransform> line3Blocks = GetNonHoldBlocks(spawner3);
        List<RectTransform> line4Blocks = GetNonHoldBlocks(spawner4);
        List<RectTransform> line1HoldBlocks = GetNonHoldBlocks(spawner1, true);
        List<RectTransform> line2HoldBlocks = GetNonHoldBlocks(spawner2, true);
        List<RectTransform> line3HoldBlocks = GetNonHoldBlocks(spawner3, true);
        List<RectTransform> line4HoldBlocks = GetNonHoldBlocks(spawner4, true);
        if (Input.GetKeyDown(KeyCode.D))
        {
            line1Button.OnPointerDown(pointer);
            OnKeyDown(ref line1CurrentHold, line1Button, line1Blocks, line1HoldBlocks);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            line1Button.OnPointerUp(pointer);
            OnKeyUp(ref line1CurrentHold, line1Button);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            line2Button.OnPointerDown(pointer);
            OnKeyDown(ref line2CurrentHold, line2Button, line2Blocks, line2HoldBlocks);
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            line2Button.OnPointerUp(pointer);
            OnKeyUp(ref line2CurrentHold, line2Button);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            line3Button.OnPointerDown(pointer);
            OnKeyDown(ref line3CurrentHold, line3Button, line3Blocks, line3HoldBlocks);
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            line3Button.OnPointerUp(pointer);
            OnKeyUp(ref line3CurrentHold, line3Button);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            line4Button.OnPointerDown(pointer);
            OnKeyDown(ref line4CurrentHold, line4Button, line4Blocks, line4HoldBlocks);
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            line4Button.OnPointerUp(pointer);
            OnKeyUp(ref line4CurrentHold, line4Button);
        }
    }

    private void OnKeyDown(ref RectTransform lineCurrentHold, Button lineButton, List<RectTransform> lineBlocks,
        List<RectTransform> lineHoldBlocks)
    {
        if (lineBlocks.Count != 0 && GetLineButtonDifference(lineButton, lineBlocks) < 30)
        {
            Perfect();
            FindClosestY(lineButton.GetComponent<RectTransform>(), lineBlocks).GetComponent<FallingBlock>()
                .IsUsed = true;
        }
        else if (lineHoldBlocks.Count != 0 &&
                 GetLineButtonHoldDifference(lineButton, lineHoldBlocks, -holdBlockCenterShift) < 30)
        {
            Perfect();
            lineCurrentHold = FindClosestYHold(lineButton.GetComponent<RectTransform>(), lineHoldBlocks,
                -holdBlockCenterShift);
            lineCurrentHold.GetComponent<FallingBlock>().IsUsed = true;
        }
        else if (lineBlocks.Count != 0 && GetLineButtonDifference(lineButton, lineBlocks) < 80)
        {
            Good();
            FindClosestY(lineButton.GetComponent<RectTransform>(), lineBlocks).GetComponent<FallingBlock>()
                .IsUsed = true;
        }
        else if (lineHoldBlocks.Count != 0 &&
                 GetLineButtonHoldDifference(lineButton, lineHoldBlocks, -holdBlockCenterShift) < 80)
        {
            Good();
            lineCurrentHold = FindClosestYHold(lineButton.GetComponent<RectTransform>(), lineHoldBlocks,
                -holdBlockCenterShift);
            lineCurrentHold.GetComponent<FallingBlock>().IsUsed = true;
        }
        else
        {
            Miss();
        }
    }

    private void OnKeyUp(ref RectTransform lineCurrentHold, Button lineButton)
    {
        if (lineCurrentHold == null)
        {
            return;
        }

        if (GetYDifferenceHold(lineButton.GetComponent<RectTransform>(), lineCurrentHold, holdBlockCenterShift) < 30)
        {
            Perfect();
            lineCurrentHold = null;
        }
        else if (GetYDifferenceHold(lineButton.GetComponent<RectTransform>(), lineCurrentHold, holdBlockCenterShift) < 80)
        {
            Good();
            lineCurrentHold = null;
        }
    }

    private void Miss()
    {
        _accuracyTriesCount++;
        _combosValue = 0;
        _guiManager.UpdatePhrase("Miss");
        _guiManager.UpdateCombo("");
    }

    private void Good()
    {
        _accuracyTriesCount++;
        _accuracySuccess++;
        _combosValue++;
        _scoreValue++;
        _guiManager.UpdateScore(_scoreValue);
        _guiManager.UpdatePhrase("Good");
        _guiManager.UpdateCombo("X " + _combosValue);
    }

    private void Perfect()
    {
        _accuracyTriesCount++;
        _accuracySuccess++;
        _combosValue++;
        _scoreValue++;
        _scoreValue++;
        _guiManager.UpdateScore(_scoreValue);
        _guiManager.UpdatePhrase("Perfect");
        _guiManager.UpdateCombo("X " + _combosValue);
    }

    private float GetLineButtonHoldDifference(Button lineButton, List<RectTransform> lineBlocks, float shift)
    {
        return Mathf.Abs(GetYDifferenceHold(
            lineButton.GetComponent<RectTransform>(),
            FindClosestYHold(lineButton.GetComponent<RectTransform>(), lineBlocks, shift), shift));
    }

    public RectTransform FindClosestYHold(RectTransform reference, List<RectTransform> candidates, float shift)
    {
        if (candidates == null || candidates.Count == 0)
            return null;

        RectTransform closest = null;
        float minDiff = float.MaxValue;

        float refY = reference.position.y;

        foreach (RectTransform candidate in candidates)
        {
            float candidateY = candidate.position.y + shift;
            float diff = Mathf.Abs(candidateY - refY);
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = candidate;
            }
        }

        return closest;
    }

    public float GetYDifferenceHold(RectTransform a, RectTransform b, float shift)
    {
        float worldA_Y = a.position.y;
        float worldB_Y = b.position.y + shift;

        return worldA_Y - worldB_Y;
    }

    private float GetLineButtonDifference(Button lineButton, List<RectTransform> lineBlocks)
    {
        return Mathf.Abs(GetYDifference(FindClosestY(lineButton.GetComponent<RectTransform>(), lineBlocks),
            lineButton.GetComponent<RectTransform>()));
    }

    public RectTransform FindClosestY(RectTransform reference, List<RectTransform> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return null;

        RectTransform closest = null;
        float minDiff = float.MaxValue;

        float refY = reference.position.y;

        foreach (RectTransform candidate in candidates)
        {
            float diff = Mathf.Abs(candidate.position.y - refY);
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = candidate;
            }
        }

        return closest;
    }

    public float GetYDifference(RectTransform a, RectTransform b)
    {
        Vector3 worldA = a.position;
        Vector3 worldB = b.position;

        return worldA.y - worldB.y;
    }
}