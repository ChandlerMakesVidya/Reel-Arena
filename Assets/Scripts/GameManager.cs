using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author: Chandler Hummingbird
 * Date Created: Sep 12, 2020 (recycled from Coulomb's Law)
 * Date Modified Oct 30, 2020
 * Description: The Game Manager manages basic game resources such as score, health, enemy spawns.
 */

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager GM;
    private void Awake()
    {
        if(GM == null)
        {
            GM = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (GM != this)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Variables
    public static float timer;
    public static int gameScore;
    public static int totalScore; //totalScore = gameScore + timer(int round down)

    public static int roundNum;

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState gameState = GameState.MainMenu;
    private bool gameOver;
    [Space]

    public GameObject player;
    public GameObject[] enemyTypes;
    public int enemyCap;
    public Enemy[] activeEnemies;
    [Space]

    public UIManager UIManager;
    public Randomizer Randomizer;

    public AudioClip hitMarker;
    public AudioClip killMarker;

    public float mouseSensitivity;

    #endregion

    private void Start()
    {
        if(gameState == GameState.MainMenu)
        {
            UIManager.mainMenuCanvas.gameObject.SetActive(true);
            LockCursor(false);
        } else if(gameState == GameState.Playing)
        {
            BeginGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameState != GameState.MainMenu)
        {
            ReturnToMainMenu();
        }

        if(Input.GetKeyDown(KeyCode.R) && gameState == GameState.Playing)
        {
            RestartCurrentLevel();
        }

        switch (gameState)
        {
            case GameState.Playing:
                timer += Time.deltaTime;
                break;
            case GameState.GameOver:
                if (!gameOver)
                {
                    LockCursor(false);
                    totalScore = gameScore + Mathf.FloorToInt(timer);
                    UIManager.HideAllCanvases();
                    UIManager.gameOverCanvas.gameObject.SetActive(true);
                    UIManager.ShowFinalStats();
                    gameOver = true;
                }
                break;
        }
    }

    public void BeginGame()
    {
        timer = 0.0f;
        gameScore = 0;
        totalScore = 0;
        roundNum = 1;
        UIManager.InitHUD();
        gameState = GameState.Playing;
        gameOver = false;
        player = GameObject.FindGameObjectWithTag("Player");
        Randomizer.ResetRandomizer();
        Randomizer.randomizerActive = true;
        LockCursor(true);
    }

    public void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Randomizer.ResetRandomizer();
        gameState = GameState.Playing;
    }

    public void ReturnToMainMenu()
    {
        if(SceneManager.GetActiveScene().name != "mainMenu")
        {
            SceneManager.LoadScene("mainMenu");
        }
        UIManager.HideAllCanvases();
        UIManager.mainMenuCanvas.gameObject.SetActive(true);
        gameState = GameState.MainMenu;
        Randomizer.randomizerActive = false;
        LockCursor(false);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
        gameState = GameState.Playing;
    }

    private void OnLevelWasLoaded(int level)
    {
        if(gameState == GameState.Playing)
        {
            BeginGame();
        }
    }

    void LockCursor(bool trueOrFalse)
    {
        if(trueOrFalse == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ChangeSensitivity()
    {
        float value = UIManager.sensitivitySlider.value;
        mouseSensitivity = value;
        UIManager.sliderText.text = value.ToString("F0");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
