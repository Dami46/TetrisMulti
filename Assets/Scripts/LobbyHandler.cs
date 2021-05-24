using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAPI;
using System;
using System.Text;
using TMPro;
using MLAPI.Transports.UNET;
using UnityEngine.Networking;
using MLAPI.Messaging;

public class LobbyHandler : NetworkBehaviour
{
    [SerializeField] public InputField inputField;
    public Text hud_error;
    private GameObject panel;
    private GameObject PlaygroudP1;
    private GameObject PlaygroudP2;
    private Canvas gameHud;
    public Text hud_Score1;
    public Text hud_Score2;
    MultiGameLogic gameLogic;
    private string password;

    

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 20;
        hud_error.enabled = false;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        panel = GameObject.FindGameObjectWithTag("Panel");
        gameHud = GameObject.Find("GameHud").GetComponent<Canvas>();
        gameHud.GetComponent<Canvas>().enabled = false;

        PlaygroudP1 = GameObject.FindGameObjectWithTag("Playground P1");
        PlaygroudP2 = GameObject.FindGameObjectWithTag("Playground P2");

        PlaygroudP1.SetActive(false);
        PlaygroudP2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            hud_Score1.text = "You (host): ";

        }
        else if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            hud_Score2.text = "You (client): ";
        }

    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // Debug.Log(GetComponent<NetworkObject>().OwnerClientId);
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            panel.SetActive(false);
            gameHud.GetComponent<Canvas>().enabled = true;

          
            //if (passwordCheck())
            //{
            //    SceneManager.LoadScene("MultiGame");
            //}
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            panel.SetActive(true);
            gameHud.GetComponent<Canvas>().enabled = false;
            PlaygroudP1.SetActive(false);
            PlaygroudP2.SetActive(false);
        }
    }

  

    public void CreateLobby()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.StartHost();
        PlaygroudP1.SetActive(true);
        PlaygroudP1.GetComponent<NetworkObject>().Spawn();
        gameLogic = PlaygroudP1.GetComponent<MultiGameLogic>();
        gameLogic.SpawnBlock(PlaygroudP1.GetComponent<NetworkObject>().OwnerClientId);

        //if (passwordCheck())
        //{
        //    SceneManager.LoadScene("MultiGame");
        //}

    }


    public void JoinLobby()
    { 
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(inputField.text);
        NetworkManager.Singleton.StartClient();

    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = password == inputField.text;

        Vector2 spawnPos = Vector2.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 1:

                spawnPos = new Vector2(29.5f, 15);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);

                break;

        }

        callback(true, null, approveConnection, spawnPos, spawnRot);

        gameLogic = PlaygroudP2.GetComponent<MultiGameLogic>();
        gameLogic.SpawnBlock(2);
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            SceneManager.LoadScene("Menu");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
            SceneManager.LoadScene("Menu");
        }
        panel.SetActive(true);
        gameHud.GetComponent<Canvas>().enabled = false;

        PlaygroudP1.SetActive(false);
        PlaygroudP2.SetActive(false);
        SceneManager.LoadScene("Menu");
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
