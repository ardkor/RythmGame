using UnityEngine;
using System.Collections.Generic;

public class OfflineAudioAnalyzer : MonoBehaviour
{
    [Header("List of clips to analyze")]
    public List<AudioClip> clips;

    public int samplesPerFrame = 1024;
    public float peakThreshold = 0.15f;

    // Результаты анализа: список пиков для каждого клипа
    public List<List<float>> peakTimesPerClip = new List<List<float>>();

    private float[] clipData;
// текущий индекс пика для каждого клипа
    private List<int> peakIndices = new List<int>();

    public void InitPeakIndices()
    {
        peakIndices.Clear();
        for (int i = 0; i < peakTimesPerClip.Count; i++)
            peakIndices.Add(0);
    }
    public bool IsTimeToSpawnNoteFromOfflineData(int clipIndex, float audioTime)
    {
        if (clipIndex < 0 || clipIndex >= peakTimesPerClip.Count)
            return false;

        List<float> peaks = peakTimesPerClip[clipIndex];
        int index = peakIndices[clipIndex];

        if (index >= peaks.Count)
            return false; // пики закончились

        float nextPeakTime = peaks[index];

        if (audioTime >= nextPeakTime)
        {
            peakIndices[clipIndex]++; // переходим к следующему пику
            return true;
        }

        return false;
    }

    void Start()
    {
        AnalyzeAllClips();
        Debug.Log($"Analyzed {clips.Count} clips.");
    }

    void AnalyzeAllClips()
    {
        peakTimesPerClip.Clear();

        for (int c = 0; c < clips.Count; c++)
        {
            AudioClip clip = clips[c];
            if (clip == null)
            {
                Debug.LogWarning($"Clip {c} is NULL, skipping...");
                peakTimesPerClip.Add(new List<float>());
                continue;
            }

            Debug.Log($"Analyzing: {clip.name}");

            // Анализируем один клип
            List<float> peaks = AnalyzeSingleClip(clip);

            peakTimesPerClip.Add(peaks);

            Debug.Log($"Clip #{c} ({clip.name}) → Peaks: {peaks.Count}");
        }
    }

    List<float> AnalyzeSingleClip(AudioClip clip)
    {
        int totalSamples = clip.samples * clip.channels;
        clipData = new float[totalSamples];

        clip.GetData(clipData, 0);

        List<float> peakTimes = new List<float>();

        int totalFrames = totalSamples / samplesPerFrame;

        for (int frame = 0; frame < totalFrames; frame++)
        {
            float[] window = new float[samplesPerFrame];

            int offset = frame * samplesPerFrame;

            // наполняем окно
            for (int i = 0; i < samplesPerFrame; i++)
            {
                if (offset + i < clipData.Length)
                    window[i] = clipData[offset + i];
            }

            float energy = AnalyzeWindow(window);

            if (energy > peakThreshold)
            {
                float time = (float)offset / clip.frequency;
                peakTimes.Add(time);
            }
        }

        return peakTimes;
    }

    float AnalyzeWindow(float[] data)
    {
        int n = data.Length;
        Complex[] buffer = new Complex[n];

        for (int i = 0; i < n; i++)
            buffer[i] = new Complex(data[i]);

        FFTUtility.FFT(buffer);

        float sum = 0f;

        // берём только низко-средние частоты — ритм
        for (int i = 1; i < n / 4; i++)
            sum += buffer[i].Magnitude;

        return sum / (n / 4);
    }
}
