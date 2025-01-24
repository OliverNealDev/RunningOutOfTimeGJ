using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource musicSource;
    public AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError("Audio sources are not assigned in AudioManager.");
        }

        SetMusicVolume(0.1f);
        SetSFXVolume(0.2f);
    }

    public void PlaySong(AudioClip song)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music source not assigned.");
            return;
        }
    
        if (!musicSource.gameObject.activeInHierarchy || !musicSource.enabled)
        {
            Debug.LogError("Music source is either disabled or the GameObject is inactive.");
            return;
        }
    
        musicSource.Stop(); // Stop any currently playing audio
        musicSource.clip = song;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music source not assigned.");
            return;
        }

        musicSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null)
        {
            Debug.LogError("SFX source not assigned.");
            return;
        }

        sfxSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }
}