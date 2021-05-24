using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MultiTetrisBlock : NetworkBehaviour
{
    MultiGameLogic gameLogic;
    bool movable = true;
    float timer = 0f;
    public GameObject rig;
    double height = 30.5f;
    private float fallSpeed;
    //audio
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private GameObject PlaygroudP1;
    private GameObject PlaygroudP2;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameLogic = FindObjectOfType<MultiGameLogic>();

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
                gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)] = subBlock;
                //Debug.Log(Mathf.FloorToInt(subBlock.position.x) + "  ,  " + Mathf.FloorToInt(subBlock.position.y));
            }
            catch (IndexOutOfRangeException ex)
            {
                ex.ToString();
            }


        }
    }

    bool CheckValid()
    {
        foreach (Transform subBlock in rig.transform)
        {
            if (IsHost)
            {

                if (subBlock.transform.position.x >= PlaygroudP1.transform.position.x + 8.5f || subBlock.transform.position.x < PlaygroudP1.transform.position.x - 8.5f || subBlock.transform.position.y < 0)
                {
                    height = subBlock.position.y;
                    return false;
                }

                if (subBlock.position.y < GameLogic.height && gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)] != null)
                {
                   
                    height = subBlock.position.y;
                    return false;
                }
            }

            if (IsClient && !IsHost)
            {

                if (subBlock.transform.position.x >= PlaygroudP2.transform.position.x + 8.5f || subBlock.transform.position.x < PlaygroudP2.transform.position.x - 8.5f || subBlock.transform.position.y < 0)
                {
                    height = subBlock.position.y;
                    return false;
                }

                // Debug.Log(Mathf.FloorToInt(subBlock.position.x) + "----" + Mathf.FloorToInt(subBlock.position.y));
                // Debug.Log(gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)]);

                if (subBlock.position.y < GameLogic.height && gameLogic.grid[Mathf.FloorToInt(subBlock.position.x ), Mathf.FloorToInt(subBlock.position.y)] != null)
                {
                    height = subBlock.position.y;
                    return false;
                }
            }
        }


        return true;
    }



    // Update is called once per frame
    void Update()
    {
        if (movable  && !gameLogic.isPaused)
        {
            if (!IsOwner) { return; }
            //update the timer
            timer += 1 * Time.deltaTime;

            //drop
            if (Input.GetKey(KeyCode.DownArrow) && timer > GameLogic.quickDropTime)
            {
                gameObject.transform.position -= new Vector3(0, 1, 0);
                timer = 0;
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    audioSource.PlayOneShot(moveSound);
                }

                if (!CheckValid())
                {
                    movable = false;
                    gameObject.transform.position += new Vector3(0, 1, 0);

                    if (height == 30.5 || height == 28.5 || height == 29.5)
                    {
                       // gameLogic.GameOver();
                    }
                    RegiserBlock();
                    audioSource.PlayOneShot(landSound);
                    gameLogic.currentScore += 10;
                    gameLogic.UpdatePlayground();
                    
                    Debug.Log("LocalClientID przy spawnie blocku" + NetworkManager.Singleton.LocalClientId);
                    gameLogic.SpawnBlock(NetworkManager.Singleton.LocalClientId);

                }
            }
            else if (timer > GameLogic.dropTime)
            {
                gameObject.transform.position -= new Vector3(0, 1, 0);
                timer = 0;
                if (!CheckValid())
                {
                    movable = false;
                    gameObject.transform.position += new Vector3(0, 1, 0);

                    if (height == 30.5 || height == 28.5 || height == 29.5)
                    {
                        //gameLogic.GameOver();
                    }
                    RegiserBlock();
                    gameLogic.currentScore += 10;
                    gameLogic.UpdatePlayground();

                    Debug.Log("LocalClientID przy spawnie blocku" + NetworkManager.Singleton.LocalClientId);
                    gameLogic.SpawnBlock(NetworkManager.Singleton.LocalClientId);

                }
            }


            //sideways
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameObject.transform.position -= new Vector3(1, 0, 0);
                if (!CheckValid())
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
                if (!CheckValid())
                {
                    gameObject.transform.position -= new Vector3(1, 0, 0);
                }
                else
                {
                    audioSource.PlayOneShot(moveSound);
                }

            }


            //rotation
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (gameLogic.rotatable)
                {
                    gameObject.transform.eulerAngles -= new Vector3(0, 0, 90);
                    if (!CheckValid())
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
