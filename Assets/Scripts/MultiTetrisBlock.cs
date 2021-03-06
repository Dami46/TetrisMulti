using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiTetrisBlock : Photon.MonoBehaviour
{
    MultiGameLogic gameLogic;
    bool movable = true;
    float timer = 0f;
    public GameObject rig;
    double height = 30.5f;
    private float fallSpeed;
    private MultiGameManager gameManager;

    //audio
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private GameObject PlaygroudP1;
    private GameObject PlaygroudP2;
    private AudioSource audioSource;

    //Photon
    public PhotonView photonView;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameLogic = FindObjectOfType<MultiGameLogic>();
        gameManager = FindObjectOfType<MultiGameManager>();
        PlaygroudP1 = GameObject.FindGameObjectWithTag("Playground P1");
        PlaygroudP2 = GameObject.FindGameObjectWithTag("Playground P2");

    }


    void RegiserBlock()
    {

        foreach (Transform subBlock in rig.transform)
        {
            try
            {
                height = subBlock.position.y;
                if (PlaygroudP1)
                {
                    if (PhotonNetwork.player.ID == 1)
                    {
                        gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)] = subBlock;
                    }


                }
                if (PlaygroudP2)
                {
                    if (PhotonNetwork.player.ID == 2)
                    {
                        gameLogic.grid[Mathf.FloorToInt(subBlock.position.x - 21), Mathf.FloorToInt(subBlock.position.y)] = subBlock;
                    }
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                ex.ToString();
            }


        }
    }

    bool CheckValid(int clientId)
    {
        foreach (Transform subBlock in rig.transform)
        {
            if (PlaygroudP1)
            {
                if (clientId == 1)
                {
                    gameLogic = PlaygroudP1.GetComponent<MultiGameLogic>();
                    if (subBlock.transform.position.x >= PlaygroudP1.transform.position.x + 8.5f || subBlock.transform.position.x < PlaygroudP1.transform.position.x - 8.5f || subBlock.transform.position.y < 0)
                    {
                        height = subBlock.position.y;
                        return false;
                    }

                    if (subBlock.position.y < MultiGameLogic.height && gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)] != null)
                    {

                        height = subBlock.position.y;
                        return false;
                    }
                }
            }
            if (PlaygroudP2)
            {
                if (clientId == 2)
                {
                    gameLogic = PlaygroudP2.GetComponent<MultiGameLogic>();
                    if (subBlock.transform.position.x >= PlaygroudP2.transform.position.x + 8.5f || subBlock.transform.position.x < PlaygroudP2.transform.position.x - 8.5f || subBlock.transform.position.y < 0)
                    {
                        height = subBlock.position.y;
                        return false;
                    }
                

                    if (subBlock.position.y < MultiGameLogic.height && gameLogic.grid[Mathf.FloorToInt(subBlock.position.x - 21), Mathf.FloorToInt(subBlock.position.y)] != null)
                    {
                        height = subBlock.position.y;
                        return false;
                    }
                }
            }

        }

        return true;
    }



    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {

            if (movable && !gameManager.isPaused)
            {

                //update the timer
                timer += 1 * Time.deltaTime;

                //drop
                if (Input.GetKey(KeyCode.DownArrow) && timer > MultiGameLogic.quickDropTime)
                {
                    gameObject.transform.position -= new Vector3(0, 1, 0);
                    timer = 0;
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        audioSource.PlayOneShot(moveSound);
                    }

                    if (!CheckValid(PhotonNetwork.player.ID))
                    {
                        movable = false;
                        gameObject.transform.position += new Vector3(0, 1, 0);

                        if (height == 30.5 || height == 28.5 || height == 29.5)
                        {
                             gameLogic.GameOver();
                        }
                        RegiserBlock();
                        audioSource.PlayOneShot(landSound);
                        gameLogic.currentScore += 10;
                        gameLogic.UpdatePlayground();

                        gameLogic.SpawnBlock(PhotonNetwork.player.ID);

                    }
                }
                else if (timer > MultiGameLogic.dropTime)
                {
                    gameObject.transform.position -= new Vector3(0, 1, 0);
                    timer = 0;
                    if (!CheckValid(PhotonNetwork.player.ID))
                    {
                        movable = false;
                        gameObject.transform.position += new Vector3(0, 1, 0);

                        if (height == 30.5 || height == 28.5 || height == 29.5)
                        {
                            gameLogic.GameOver();
                        }
                        RegiserBlock();
                        gameLogic.currentScore += 10;
                        gameLogic.UpdatePlayground();

                        gameLogic.SpawnBlock(PhotonNetwork.player.ID);

                    }
                }


                //sideways
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    gameObject.transform.position -= new Vector3(1, 0, 0);
                    if (!CheckValid(PhotonNetwork.player.ID))
                    {
                        gameObject.transform.position += new Vector3(1, 0, 0);

                    }
                    else
                    {
                        audioSource.PlayOneShot(moveSound);
                    }

                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    gameObject.transform.position += new Vector3(1, 0, 0);
                    if (!CheckValid(PhotonNetwork.player.ID))
                    {
                        gameObject.transform.position -= new Vector3(1, 0, 0);
                    }
                    else
                    {
                        audioSource.PlayOneShot(moveSound);
                    }

                }


                //rotation
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (gameLogic.rotatable)
                    {
                        gameObject.transform.eulerAngles -= new Vector3(0, 0, 90);
                        if (!CheckValid(PhotonNetwork.player.ID))
                        {
                            gameObject.transform.eulerAngles += new Vector3(0, 0, 90);
                        }
                        else
                        {
                            audioSource.PlayOneShot(rotateSound);
                        }
                    }
                }
            }

        }
    }
}