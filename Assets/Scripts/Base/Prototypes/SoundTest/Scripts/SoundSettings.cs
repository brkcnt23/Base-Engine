using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base.UI;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour {

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Toggle muteToggle;

    [SerializeField] Button openButton;
    [SerializeField] Button closeButton;

    [SerializeField] GameObject soundSettingsPanel;

    float masterVolume {
        get => PlayerPrefs.GetFloat("masterVolume", 1);
        set => PlayerPrefs.SetFloat("masterVolume", value);
    }

    bool isMuted {
        get => PlayerPrefs.GetInt("isMuted", 1) == 1;
        set => PlayerPrefs.SetInt("isMuted", value ? 1 : 0);
    }

    public void Activate() {

        masterVolumeSlider.value = masterVolume;
        muteToggle.isOn = !isMuted;

        openButton.onClick.AddListener(OpenSoundSettings);
        closeButton.onClick.AddListener(CloseSoundSettings);
        
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        muteToggle.onValueChanged.AddListener(SetMute);
        
        
        SetMasterVolume(masterVolume);
        SetMute(!isMuted);
    }

    public void OpenSoundSettings() {
        B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_Paused);
    }

    public void CloseSoundSettings() {
        B_GUIManager.DeactivateOnePanel(Enum_MenuTypes.Menu_Paused);
    }

    public void SetMasterVolume(float value) {
        masterVolume = value;
        if (!isMuted) return;
        AudioListener.volume = masterVolume;
    }

    public void SetMute(bool value) {
        isMuted = !value;
        AudioListener.pause = !isMuted;
    }
    
    void OpenURL(string url) {
        Application.OpenURL(url);
    }
    
    public void OpenReddit() {
        OpenURL("https://www.reddit.com/r/DirtyLeds/");
    }
    
    public void OpenTwitter() {
        OpenURL("https://twitter.com/TheDirtyLeds");
    }
    
    public void OpenInstagram() {
        OpenURL("https://www.instagram.com/thedirtyleds/");
    }

}