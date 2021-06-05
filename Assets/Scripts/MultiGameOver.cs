using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiGameOver : MonoBehaviour
{

    public Text loseText;
    public Text scoreText;
    public Text levelText;
    private int endScore = 0;
    private int endLevel= 0;


    // Start is called before the first frame update
    void Start()
    {
        loseText.text = "YOU LOSE !";
        loseText.color = Color.red;

        endScore = PlayerPrefs.GetInt("endScore");
        scoreText.text = "Score: " + endScore;

        endLevel = PlayerPrefs.GetInt("endLevel");
        levelText.text = "Level: " + endLevel;
        levelText.color = Color.black;
    }

    


    public void LeaveRoom()
    {
        SceneManager.LoadScene("MultiLobby");
    }

}
