using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider audioVolumeSlider;

    public Resolution[] resolutions;
    public AudioSource audioSource;
    public GameSettings gameSettings;
    public Button saveButton;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource source1 = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        audioSource = source1;
        gameSettings = new GameSettings();
        resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            LoadSettings();
        }
        audioSource.volume = gameSettings.audioVolume;
    }

    // Update is called once per frame
    void Update()
    {
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        audioVolumeSlider.onValueChanged.AddListener(delegate { OnAudioVolumeChange(); });
        saveButton.onClick.AddListener(delegate { onSaveButtonClisk(); });
    }

    public void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnAudioVolumeChange()
    {
        audioSource.volume = gameSettings.audioVolume = audioVolumeSlider.value;
    }

    public void onSaveButtonClisk()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        audioVolumeSlider.value = gameSettings.audioVolume;
        fullscreenToggle.isOn = gameSettings.fullscreen;
        resolutionDropdown.value = gameSettings.resolutionIndex;

        Screen.fullScreen = gameSettings.fullscreen;
        resolutionDropdown.RefreshShownValue();
    }

    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }
}
