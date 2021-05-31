using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiGameManager : Photon.MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject StartCanvas;
    private MultiGameLogic gameLogic;

    private PhotonView photonView;
    private GameObject playerReady;

    //pause
    public bool isPaused = true;
    public GameObject ReadyStatusP1;
    public GameObject ReadyStatusP2;

    public GameObject ReadyTextP1;
    public GameObject ReadyTextP2;
    [SerializeField] private int playersReady = 2;

    private GameObject messageP1;
    private GameObject messageP2;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = true;
        Time.timeScale = 0;

    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();

        if(playersReady == 2)
        {
            Destroy(ReadyStatusP1);
            Destroy(ReadyStatusP2);
            isPaused = false;
            Time.timeScale = 1;
        }
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
               
                gameLogic = PlayerPrefabs[0].GetComponent<MultiGameLogic>();
                gameLogic.SpawnBlock(1);
                photonView = player1.GetComponent<PhotonView>();
               // photonView.RPC("PlayerP1Unready", PhotonTargets.All);
                PlayerP1Unready();
                break;

            case 2:
                GameObject player2 = PhotonNetwork.Instantiate(PlayerPrefabs[1].name, new Vector2(29.5f, 15), Quaternion.identity, 0);
 
                gameLogic = PlayerPrefabs[1].GetComponent<MultiGameLogic>();
                gameLogic.SpawnBlock(2);
                photonView = player2.GetComponent<PhotonView>();
                //photonView.RPC("PlayerP2Unready", PhotonTargets.All);
                PlayerP2Unready();
                break;

        }

        StartCanvas.SetActive(false);
    }

  

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiLobby");
    }


    void CheckUserInput()
    {
        //if(!photonView.isMine) { return; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playersReady != 2)
            {
                if (PhotonNetwork.player.ID == 1)
                {
                    PlayerP1Ready();
                }
                else if (PhotonNetwork.player.ID == 2)
                {
                    PlayerP2Ready();
                }
            }
        }
    }

    private void PlayerP1Ready()
    {

        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();
        playersReady = playersReady + 1;
        messageP1.GetComponent<Text>().text = "READY";
        messageP1.GetComponent<Text>().color = Color.green;

    }
    private void PlayerP2Ready()
    {
        // audioSource = GetComponent<AudioSource>();
        // audioSource.Play();
        playersReady = playersReady + 1;
        messageP2.GetComponent<Text>().text = "READY";
        messageP2.GetComponent<Text>().color = Color.green;

    }

    [PunRPC]
    private void PlayerP1Unready()
    {
        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();
        playersReady = playersReady - 1;
        messageP1 = Instantiate(ReadyTextP1, new Vector2(0, 0), Quaternion.identity);
        messageP1.transform.SetParent(ReadyStatusP1.transform, false);
        messageP1.GetComponent<Text>().text = "UNREADY";
        messageP1.GetComponent<Text>().color = Color.red;

    }

    [PunRPC]
    private void PlayerP2Unready()
    {
        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();
        playersReady = playersReady - 1;
        messageP2 = Instantiate(ReadyTextP2, new Vector2(0, 0), Quaternion.identity);
        messageP2.transform.SetParent(ReadyStatusP2.transform, false);
        messageP2.GetComponent<Text>().text = "UNREADY";
        messageP2.GetComponent<Text>().color = Color.red;

    }

}
