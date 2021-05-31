using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersReady : Photon.MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    //pause
    public bool isPaused = true;
    private GameObject ReadyStatusP1;
    private GameObject ReadyStatusP2;

    public GameObject ReadyTextP1;
    public GameObject ReadyTextP2;
    public int playersReady = 2;

    private GameObject messageP1;
    private GameObject messageP2;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(messageP1.GetComponent<Text>().text);
            stream.SendNext(messageP2.GetComponent<Text>().text);
        }
        else if (stream.isReading)
        {
            messageP1.GetComponent<Text>().text = (string) stream.ReceiveNext();
            messageP1.GetComponent<Text>().text = (string) stream.ReceiveNext();
        }


    }


    public void CheckUserInput()
    {
        //if(!photonView.isMine) { return; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playersReady != 2)
            {
                if (PhotonNetwork.player.ID == 1)
                {
                    photonView.RPC("PlayerP1ready", PhotonTargets.All);
                }
                else if (PhotonNetwork.player.ID == 2)
                {
                    photonView.RPC("PlayerP2ready", PhotonTargets.All);
                }
            }
        }
    }

    [PunRPC]
    private void PlayerP1Ready()
    {

        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();
        playersReady = playersReady + 1;
        messageP1.GetComponent<Text>().text = "READY";
        messageP1.GetComponent<Text>().color = Color.green;

    }

    [PunRPC]
    private void PlayerP2Ready()
    {
        // audioSource = GetComponent<AudioSource>();
        // audioSource.Play();
        playersReady = playersReady + 1;
        messageP2.GetComponent<Text>().text = "READY";
        messageP2.GetComponent<Text>().color = Color.green;

    }

    [PunRPC]
    public void PlayerP1Unready()
    {
        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();

        ReadyStatusP1 = GameObject.FindGameObjectWithTag("Pauza");

        playersReady = playersReady - 1;
        messageP1 = Instantiate(ReadyTextP1, new Vector2(0, 0), Quaternion.identity);
        messageP1.transform.SetParent(ReadyStatusP1.transform, false);
        messageP1.GetComponent<Text>().text = "UNREADY";
        messageP1.GetComponent<Text>().color = Color.red;

    }

    [PunRPC]
    public void PlayerP2Unready()
    {
        //  audioSource = GetComponent<AudioSource>();
        //  audioSource.Pause();

        ReadyStatusP2 = GameObject.FindGameObjectWithTag("PauzaP2");
        playersReady = playersReady - 1;
        messageP2 = Instantiate(ReadyTextP2, new Vector2(0, 0), Quaternion.identity);
        messageP2.transform.SetParent(ReadyStatusP2.transform, false);
        messageP2.GetComponent<Text>().text = "UNREADY";
        messageP2.GetComponent<Text>().color = Color.red;

    }


}
