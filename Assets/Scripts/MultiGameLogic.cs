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
    private MultiGameManager gameManager;

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
    private int startingScore = 0;

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

    //pauza
    public Text hud_Pause;
    private GameObject message;

    private int playersReady;

    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        if (PhotonNetwork.player.ID == 1 & this.tag == "Playground P1")
        {
            hud_Score = GameObject.FindGameObjectWithTag("score_text P1").GetComponent<Text>();
            SpawnBlock(1);
        }
        else if (PhotonNetwork.player.ID == 2 & this.tag == "Playground P2")
        {
            hud_Score = GameObject.FindGameObjectWithTag("score_text P2").GetComponent<Text>();
            SpawnBlock(2);
        }

        hud_Level.text = "Level: 0";
        hud_Score.text = "Score: 0";
        gameManager = FindObjectOfType<MultiGameManager>();
        GameObject.FindGameObjectWithTag(string.Format("pauza P{0}", PhotonNetwork.player.ID)).SetActive(true);

    }

    private void Update()
    {
        if (PhotonNetwork.playerList.Length == 2)
        {
            CheckUserInput();
        }

        if (gameManager.playerP1Ready && gameManager.playerP2Ready && gameManager.isPaused)
        {
            GetComponent<PhotonView>().RPC("ResumeGame", PhotonTargets.All);
        }
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
        UpdateLevel();
        UpdateSpeed();
    }

    [PunRPC]
    public void UpdateHighScore()
    {
        PlayerPrefs.SetInt("endScore", currentScore);
        PlayerPrefs.Save();
    }

    [PunRPC]
    public void UpdateHighLevel()
    {
        PlayerPrefs.SetInt("endLevel", currentLevel);
        PlayerPrefs.Save();
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
                this.nextTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(8.5f, 30.5f), Quaternion.identity, 0);
            }
            else
            {
                this.nextTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(29.5f, 30.5f), Quaternion.identity, 0);
            }

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }

            guess = RandomTetronimo();
            if (playerID == 1)
            {
                this.previewTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(-8, 26), Quaternion.identity, 0);
            }
            else
            {
                this.previewTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(44, 26), Quaternion.identity, 0);
            }

            this.previewTetronimo.GetComponent<MultiTetrisBlock>().enabled = false;
        }
        else
        {
            if (playerID == 1)
            {
                this.previewTetronimo.transform.localPosition = new Vector2(8.5f, 30.5f);
            }
            else
            {
                this.previewTetronimo.transform.localPosition = new Vector2(29.5f, 30.5f);
            }
            this.nextTetronimo = previewTetronimo;
            this.nextTetronimo.GetComponent<MultiTetrisBlock>().enabled = true;

            if (nextTetronimo.tag == "Tetromino O")
            {
                rotatable = false;
            }

            float guess = RandomTetronimo();
            if (playerID == 1)
            {
                this.previewTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(-8, 26), Quaternion.identity, 0);
            }
            else
            {
                this.previewTetronimo = PhotonNetwork.Instantiate(this.blocks[Mathf.FloorToInt(guess)].name, new Vector2(44, 26), Quaternion.identity, 0);
            }

            this.previewTetronimo.GetComponent<MultiTetrisBlock>().enabled = false;
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
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiLose");
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

        GetComponent<PhotonView>().RPC("UpdateHighScore", PhotonTargets.All);
        GetComponent<PhotonView>().RPC("UpdateHighLevel", PhotonTargets.All);
        GetComponent<PhotonView>().RPC("UpdatePoints", PhotonTargets.All, PhotonNetwork.player.ID, currentScore, currentLevel);
    }

    [PunRPC]
    private void UpdatePoints(int player, int points, int level)
    {
        GameObject.FindGameObjectWithTag(string.Format("score_text P{0}", player)).GetComponent<Text>().text = "Score: " + points.ToString();
        GameObject.FindGameObjectWithTag(string.Format("level_text P{0}", player)).GetComponent<Text>().text = "Level: " + level.ToString();
    }



    public void CheckUserInput()
    {
        if (!GetComponent<PhotonView>().isMine) { return; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetComponent<PhotonView>().RPC("PlayerReady", PhotonTargets.All, PhotonNetwork.player.ID);
        }
    }

    [PunRPC]
    private void PlayerReady(int player)
    {
        if (PhotonNetwork.player.ID == 1 & this.tag == "Playground P1")
        {
            gameManager.playerP1Ready = true;
        }
        else
        {
            gameManager.playerP2Ready = true;
        }
        playersReady = playersReady + 1;
        GameObject.FindGameObjectWithTag(string.Format("pauza P{0}", player)).GetComponent<Text>().text = "Ready";
        GameObject.FindGameObjectWithTag(string.Format("pauza P{0}", player)).GetComponent<Text>().color = Color.green;
    }


    [PunRPC]
    private void ResumeGame()
    {
        if (GameObject.FindGameObjectWithTag("pauza P1") != null && GameObject.FindGameObjectWithTag("pauza P1") != null)
        {
            GameObject.FindGameObjectWithTag("pauza P1").SetActive(false);
            GameObject.FindGameObjectWithTag("pauza P2").SetActive(false);
        }
        gameManager.isPaused = false;
        Time.timeScale = 1;
    }
}
