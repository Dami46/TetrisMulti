using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiGameManager : Photon.PunBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject StartCanvas;

    private PhotonView photonView;
    private GameObject playerReady;

    //pause
    public bool isPaused = true;
    private bool clientDisconnected = false;
    public Text endScreen;

    public bool playerP1Ready = false;
    public bool playerP2Ready = false;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = true;
        Time.timeScale = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (!clientDisconnected)
        {
            if (PhotonNetwork.playerList.Length < 2)
            {
                endScreen.text = "Waiting for another player...";
                endScreen.color = Color.yellow;
                Time.timeScale = 0;
                isPaused = true;
            }
            else
            {
               
                endScreen.text = "";
            }
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);

        int playerID = otherPlayer.ID == 1 ? 1 : 0;

        clientDisconnected = true;
        endScreen.text = "YOU WON !!!! ";
        endScreen.color = Color.green;
        Time.timeScale = 0;
        isPaused = true;
    }


    private void Awake()
    {
        StartCanvas.SetActive(true);
    }

    public void SpawnPlayer()
    {
        int idOfPlayers = PhotonNetwork.player.ID;
        switch (idOfPlayers)
        {
            case 1:
                GameObject player1 =  PhotonNetwork.Instantiate(PlayerPrefabs[0].name, new Vector2(8.5f, 15), Quaternion.identity, 0);
                break;

            case 2:
                GameObject player2 = PhotonNetwork.Instantiate(PlayerPrefabs[1].name, new Vector2(29.5f, 15), Quaternion.identity, 0);
                break;

        }

        StartCanvas.SetActive(false);
    }

  

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiLobby");
    }


}
