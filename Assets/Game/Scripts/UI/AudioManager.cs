using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Transform audioSourcesGroup;
    [SerializeField] private List<AudioClip> songs;
    
    private int lowestDeciblesBeforeMute = -20;
    private AudioSource currentMusicSource;
    private AudioSource currentMutedMusicSource;
    public AudioSource CurrentMutedMusicSource => currentMutedMusicSource;

    void Awake()
    {
        G.audio = this;
    }

    public void PlaySongInMenu(int index, int duration)
    {
        AudioClip clip = null;
        if (currentMusicSource != null)
        {
            clip = currentMusicSource.clip;
            Destroy(currentMusicSource.gameObject);
        }
        if (clip != songs[index])
            currentMusicSource = Play(songs[index], duration);
    }

    public float GetSongDuration(int index)
    {
        return songs[index].length;
    }
    public void PlaySongInGame(int index, float delay)
    {
        if (currentMusicSource != null)
        {
            Destroy(currentMusicSource.gameObject);
        }
        //currentMutedMusicSource = Play(songs[index]);
        //currentMutedMusicSource.mute = true;
        currentMusicSource = Play(songs[index], -1, delay);
    }

    public AudioSource Play(AudioClip clip, int duration = -1, float delay = 0, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = CreatePlaySource(clip, audioSourcesGroup, volume, pitch, delay);
        if (duration == -1)
            Destroy(source.gameObject, clip.length + delay);
        else
            Destroy(source.gameObject, duration);
        return source;
    }
    
    public AudioSource CreatePlaySource(AudioClip clip, Transform emitter, float volume, float pitch, float delay = 0,
        bool music = false)
    {
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        if (delay <= 0)
            source.Play();
        else
            source.PlayDelayed(delay);
        return source;
    }
    /*public AudioSource PlayLoop(AudioClip clip, float volume = 1f, float pitch = 1f,
        bool music = true)
    {
        AudioSource source = CreatePlaySource(clip, audioSourcesGroup, volume, pitch, music);
        source.loop = true;
        return source;
    }

    private AudioSource CreatePlaySource(AudioClip clip, Vector3 point, float volume, float pitch, bool music = false)
    {
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        source.Play();
        return source;
    }*/

    /*public void SetVolume(AudioChannel channel, int volume)
    {
        // Converts the 0 - 100 input into decibles | volume of 0 will mute, 1 should be ~the lowestDecibles set,
        // and 100 should be 0 DB offset from the base volume on the channel
        float adjustedVolume = lowestDeciblesBeforeMute + (-lowestDeciblesBeforeMute / 5 * volume / 20);

        // Effectively completed muted if volume is 0
        if (volume == 0)
        {
            adjustedVolume = -100;
        }

        masterMixer.SetFloat(channel.ToString(), adjustedVolume);
    }*/
}