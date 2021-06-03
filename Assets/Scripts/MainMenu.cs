using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text highScore;
    public AudioSource audioSource;
    public GameSettings gameSettings;

    private void Start()
    {
        highScore.text = PlayerPrefs.GetInt("highscore").ToString();
        gameSettings = new GameSettings();
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
        SceneManager.LoadScene("MultiLobby");
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
