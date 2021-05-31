using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TetrisGameLogic : MonoBehaviour
{
    public static float dropTime = 1.0f;
    public static float quickDropTime = 0.05f;
    public static int width = 17, height = 30;
    public GameObject[] blocks;
    public Transform[,] grid = new Transform[width, height];
    public bool rotatable = true;

    //score
    private int scoreOneLine = 200;
    private int scoreTwoLine = 600;
    private int scoreThreeLine = 1000;
    private int scoreFourLine = 4000;
    private int numberOfFullRows = 0;
    public Text hud_Score;
    public Text hud_Level;
    public int currentScore = 0;
    private int startingHighScore;

    //pause
    public bool isPaused = true;
    public Text hud_pause;

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
        hud_pause.enabled = false;
        SpawnBlock();
        audioSource = GetComponent<AudioSource>();
        startingHighScore = PlayerPrefs.GetInt("highscore");
        isPaused = true;
        Time.timeScale = 0;
        hud_pause.enabled = true;
    }

    private void Update()
    {
        CheckUserInput();
    }

    void UpdateLevel()
    {
        currentLevel = numberLinesCleared / 10;
    }

    private void UpdateSpeed()
    {
        dropTime = 1.0f - ((float)currentLevel * 0.1f);
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }

        }
    }

    private void PauseGame()
    {
        isPaused = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.Pause();
        Time.timeScale = 0;
        hud_pause.enabled = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Time.timeScale = 1;
        hud_pause.enabled = false;
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

    public void SpawnBlock()
    {
        rotatable = true;
        if (!gameStarted)
        {
            gameStarted = true;
            float guess = RandomTetronimo();
            nextTetronimo = Instantiate(blocks[Mathf.FloorToInt(guess)]);

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }

            guess = RandomTetronimo();
            previewTetronimo = Instantiate(blocks[Mathf.FloorToInt(guess)]);
            previewTetronimo.transform.localPosition = new Vector2(20, 25);
            previewTetronimo.GetComponent<TetrisBlock>().enabled = false;
        }
        else
        {
            previewTetronimo.transform.localPosition = new Vector2(8.5f, 30.5f);
            nextTetronimo = previewTetronimo;
            nextTetronimo.GetComponent<TetrisBlock>().enabled = true;

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }
            float guess = RandomTetronimo();
            previewTetronimo = Instantiate(blocks[Mathf.FloorToInt(guess)]);
            previewTetronimo.transform.localPosition = new Vector2(20, 25);
            previewTetronimo.GetComponent<TetrisBlock>().enabled = false;
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

    public void OnBackClick()
    {
        SceneManager.LoadScene("Menu");
    }

    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }
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
            audioSource.PlayOneShot(clearLineSound);
        }
    }
    public void UpdateUi()
    {
        hud_Score.text = "Score: " + currentScore.ToString();
        hud_Level.text = "Level: " + currentLevel.ToString();
    }



}
