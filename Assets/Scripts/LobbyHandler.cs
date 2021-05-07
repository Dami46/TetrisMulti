using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    public InputField inputField;
    public Text hud_error;

    private string password;

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 20;
        hud_error.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

   
    public void CreateLobby()
    {
        if (passwordCheck())
        {
            SceneManager.LoadScene("MultiGame");
        }

    }

    public void JoinLobby()
    {
        if (passwordCheck())
        {
            SceneManager.LoadScene("MultiGame");
        }
     
    }

    private bool passwordCheck()
    {
        if (inputField.text.Length > 3 && inputField.text.Length <= 20)
        {
            return true;
        }
        else
        {
            hud_error.enabled = true;
            hud_error.text = "Write more than 4 characters!";
            return false;
        }
    }
}
