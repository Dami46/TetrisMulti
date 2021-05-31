using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiGameLogic : MonoBehaviour
{
    public static float dropTime = 1.0f;
    public static float quickDropTime = 0.05f;
    public static int width = 17, height = 30;
    public GameObject[] blocks;
    public Transform[,] grid = new Transform[width, height];
    public bool rotatable = true;
    private GameObject PlaygroudP1;
    private GameObject PlaygroudP2;

    public Button leaveButton;

    //score
    private int scoreOneLine = 200;
    private int scoreTwoLine = 600;
    private int scoreThreeLine = 1000;
    private int scoreFourLine = 4000;
    private int numberOfFullRows = 0;
    public Text hud_Score;
    public Text hud_Level;
    public int currentScore = 0;



    //level
    public int currentLevel = 0;
    private int numberLinesCleared = 0;

    //audio
    private AudioSource audioSource;
    public AudioClip clearLineSound;

    //to display next tetronimo
    private GameObject previewTetronimo;
    public GameObject TetrominoO;
    private GameObject nextTetronimo;
    private bool gameStarted = false;


    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        hud_Level.text = "Level: 0";
        hud_Score.text = "Score: 0";

    }

    private void Update()
    {
       
    }

    void UpdateLevel()
    {
        currentLevel = numberLinesCleared / 10;
    }

    private void UpdateSpeed()
    {
        dropTime = 1.0f - ((float)currentLevel * 0.1f);
    }


    public void UpdatePlayground()
    {
        for (int y = 0; y < height; y++)
        {
            deleteFullRows();
        }

        UpdateScore();
        UpdateUi();
        UpdateLevel();
        UpdateSpeed();
    }

    bool IsLineComplete(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        numberOfFullRows++;

        return true;
    }

    public void deleteRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void decreaseRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] != null)
            {
                // Move one towards bottom
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void decreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
        {
            decreaseRow(i);
        }
    }

    public void deleteFullRows()
    {
        for (int y = 0; y < height; ++y)
        {
            if (IsLineComplete(y))
            {
                deleteRow(y);
                decreaseRowsAbove(y + 1);
                --y;
            }
        }
    }

    public void SpawnBlock(int playerID)
    {
        PlaygroudP1 = GameObject.FindGameObjectWithTag("Playground P1");
        PlaygroudP2 = GameObject.FindGameObjectWithTag("Playground P2");
        rotatable = true;
        if (!gameStarted)
        {
            gameStarted = true;
            float guess = RandomTetronimo();

            if (playerID == 1)
            {   
                nextTetronimo = PhotonNetwork.Instantiate(PlaygroudP1.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(8.5f, 30.5f), Quaternion.identity, 0);
            }
            else
            {
                nextTetronimo = PhotonNetwork.Instantiate(PlaygroudP2.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(29.5f, 30.5f), Quaternion.identity, 0);
            }

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }

            guess = RandomTetronimo();
            if (playerID == 1)
            {
                previewTetronimo = PhotonNetwork.Instantiate(PlaygroudP1.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(-8, 26), Quaternion.identity, 0);
            }
            else
            {
                previewTetronimo = PhotonNetwork.Instantiate(PlaygroudP2.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(44, 26), Quaternion.identity, 0);
            }

            previewTetronimo.GetComponent<MultiTetrisBlock>().enabled = false;
        }
        else
        {
            if (playerID == 1)
            {
                previewTetronimo.transform.localPosition = new Vector2(8.5f, 30.5f);
            }
            else
            {
                previewTetronimo.transform.localPosition = new Vector2(29.5f, 30.5f);
            }
            nextTetronimo = previewTetronimo;
            nextTetronimo.GetComponent<MultiTetrisBlock>().enabled = true;

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }

            float guess = RandomTetronimo();
            if (playerID == 1)
            {
                previewTetronimo = PhotonNetwork.Instantiate(PlaygroudP1.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(-8, 26), Quaternion.identity, 0);
            }
            else
            {
                previewTetronimo = PhotonNetwork.Instantiate(PlaygroudP2.GetComponent<MultiGameLogic>().blocks[Mathf.FloorToInt(guess)].name, new Vector2(44, 26), Quaternion.identity, 0);
            }

            previewTetronimo.GetComponent<MultiTetrisBlock>().enabled = false;
        }


    }

    private float RandomTetronimo()
    {
        float guess = UnityEngine.Random.Range(0, 1f);
        guess *= blocks.Length;

        return guess;
    }




    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }


    public void UpdateScore()
    {
        if (numberOfFullRows > 0)
        {
            switch (numberOfFullRows)
            {
                case 1:
                    currentScore += scoreOneLine;
                    numberLinesCleared++;
                    break;
                case 2:
                    currentScore += scoreTwoLine;
                    numberLinesCleared += 2;
                    break;
                case 3:
                    currentScore += scoreThreeLine;
                    numberLinesCleared += 3;
                    break;
                case 4:
                    currentScore += scoreFourLine;
                    numberLinesCleared += 4;
                    break;

            }
            numberOfFullRows = 0;
            //audioSource.PlayOneShot(clearLineSound);
        }
    }
    public void UpdateUi()
    {
        hud_Score.text = "Score: " + currentScore.ToString();
        hud_Level.text = "Level: " + currentLevel.ToString();
    }

  
}
