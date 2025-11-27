using UnityEngine;

public class RhythmTrackAnalyzer : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private float energyThreshold = 0.15f;
    [SerializeField] private float sensitivity = 1.5f;

    public float DetectedEnergy { get; private set; }
    public bool IsPeak { get; private set; }

    float[] spectrum = new float[512];
    float lastPeakTime;
    [SerializeField] private float minPeakInterval = 0.1f;

    public void Initialize(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    void Update()
    {
        if (audioSource == null)
        {
            IsPeak = false;
            return;
        }
        
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float sum = 0f;
        for (int i = 0; i < spectrum.Length; i++)
            sum += spectrum[i];

        DetectedEnergy = sum * sensitivity;
        Debug.Log(DetectedEnergy);
        bool peak = DetectedEnergy > energyThreshold;

        if (peak && Time.time - lastPeakTime >= minPeakInterval)
        {
            IsPeak = true;
            lastPeakTime = Time.time;
        }
        else
        {
            IsPeak = false;
        }
    }
}