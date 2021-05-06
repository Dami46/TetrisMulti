using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text highScore;
    private SettingsManager settingsManager;
    public AudioSource audioSource;

    private void Start()
    {
        highScore.text = PlayerPrefs.GetInt("highscore").ToString();
        GameSettings gameSettings;
        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
            audioSource.volume = gameSettings.audioVolume;
        }
        
    }

    public void PlaySingle()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayMulti()
    {
        SceneManager.LoadScene("MultiGame");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
