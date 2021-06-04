using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiGameOver : MonoBehaviour
{

    public Text loseText;


    // Start is called before the first frame update
    void Start()
    {
        loseText.text = "YOU LOSE !";
        loseText.color = Color.red;
    }


    public void LeaveRoom()
    {
        SceneManager.LoadScene("MultiLobby");
    }

}
