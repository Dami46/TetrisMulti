using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Text;


public class LobbyHandler : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private GameObject GameViev;
    [SerializeField] private GameObject StartButton;

    public Text hud_error;
    private GameObject PlaygroudP1;
    private GameObject PlaygroudP2;


    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }


    // Start is called before the first frame update
    void Start()
    {
        ConnectPanel.SetActive(true);
        inputField.characterLimit = 20;
        hud_error.enabled = false;

    }


    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }


    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(inputField.text, new RoomOptions() { maxPlayers = 2 }, null);

    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 2;
        PhotonNetwork.JoinRoom(inputField.text);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiGame");
    }

    public void Leave()
    {
        SceneManager.LoadScene("Menu");
    }


}
