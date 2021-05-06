using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    GameLogic gameLogic;
    bool movable = true;
    float timer = 0f;
    public GameObject rig;
    double height = 29;
    private float fallSpeed;

    //audio
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameLogic = FindObjectOfType<GameLogic>();
    }


    void RegiserBlock()
    {

        foreach (Transform subBlock in rig.transform)
        {
            try
            {
                height = subBlock.position.y;
                gameLogic.grid[Mathf.FloorToInt(subBlock.position.x), Mathf.FloorToInt(subBlock.position.y)] = subBlock;
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
            if (subBlock.transform.position.x >= GameLogic.width || subBlock.transform.position.x < 0 || subBlock.transform.position.y < 0)
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
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (movable && !gameLogic.isPaused)
        {
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
                        gameLogic.GameOver();
                    }
                    RegiserBlock();
                    audioSource.PlayOneShot(landSound);
                    gameLogic.currentScore += 10;
                    gameLogic.UpdatePlayground();
                    FindObjectOfType<GameLogic>().UpdateHighScore();
                    gameLogic.SpawnBlock();

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
                        gameLogic.GameOver();
                    }
                    RegiserBlock();
                    gameLogic.currentScore += 10;
                    gameLogic.UpdatePlayground();
                    gameLogic.SpawnBlock();

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
                if(gameLogic.rotatable)
                {
                    rig.transform.eulerAngles -= new Vector3(0, 0, 90);
                    if (!CheckValid())
                    {
                        rig.transform.eulerAngles += new Vector3(0, 0, 90);
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
