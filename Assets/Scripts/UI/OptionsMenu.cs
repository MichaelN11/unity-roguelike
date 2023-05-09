using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for controlling the options menu.
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
        musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
        sfxVolumeSlider.value = AudioManager.Instance.SfxVolume;
    }

    public void SetMasterVolume(float masterVolume)
    {
        AudioManager.Instance.SetMasterVolume(masterVolume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        AudioManager.Instance.SetMusicVolume(musicVolume);
    }

    public void SetSfxVolume(float sfxVolume)
    {
        AudioManager.Instance.SetSfxVolume(sfxVolume);
    }
}
