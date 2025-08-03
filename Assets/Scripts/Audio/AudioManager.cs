using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class for playing sound effects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private float masterVolume = 0.5f;
    public float MasterVolume => masterVolume;

    private float musicVolume = 0.5f;
    public float MusicVolume => musicVolume;

    private float sfxVolume = 0.5f;
    public float SfxVolume => sfxVolume;

    private readonly Dictionary<string, AudioSource> soundMap = new();
    private AudioSource musicSource;
    private Sound currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays the passed sound effect. Adds a new AudioSource component to the game object,
    /// if one hasn't already been added for the sound.
    /// </summary>
    /// <param name="sound">The Sound scriptable object</param>
    public void Play(Sound sound)
    {
        if (sound != null)
        {
            if (!soundMap.TryGetValue(sound.name, out AudioSource audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = sound.AudioClip;
                audioSource.volume = sound.Volume;
                audioSource.pitch = sound.Pitch;
                audioSource.loop = sound.Loop;
                soundMap.Add(sound.name, audioSource);
            }

            if (audioSource == musicSource && audioSource.isPlaying)
            {
                return;
            }

            if (sound.IsMusic)
            {
                StopMusic();
                currentMusic = sound;
                musicSource = audioSource;
                UpdateCurrentMusicVolume();
                audioSource.Play();
            } else
            {
                audioSource.PlayOneShot(sound.AudioClip, sfxVolume * masterVolume);
            }
        }
    }

    /// <summary>
    /// Stops the current music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void StopSound(Sound sound)
    {
        if (soundMap.TryGetValue(sound.name, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
    }

    public void SetMasterVolume(float masterVolume)
    {
        this.masterVolume = masterVolume;
        UpdateCurrentMusicVolume();
    }

    public void SetMusicVolume(float musicVolume)
    {
        this.musicVolume = musicVolume;
        UpdateCurrentMusicVolume();
    }

    public void SetSfxVolume(float sfxVolume)
    {
        this.sfxVolume = sfxVolume;
    }

    private void UpdateCurrentMusicVolume()
    {
        musicSource.volume = musicVolume * masterVolume * currentMusic.Volume;
    }
}
