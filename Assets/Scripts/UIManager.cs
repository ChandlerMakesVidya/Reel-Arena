using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Author: Chandler Hummingbird
 * Date Created: forgot
 * Date Modified: Oct 30, 2020
 * Description: The UI Manager works in tandem with the game manager to load the
 * appropriate canvases at the right times and output data to UI elements.
 */

public class UIManager : MonoBehaviour
{
    public static UIManager UI;
    private void Awake()
    {
        if(UI == null)
        {
            UI = this;
        }
        //destruction of duplicates already handled by game manager

        HideAllCanvases();
    }

    public Canvas mainMenuCanvas;
    public Canvas HUDCanvas;
    public Canvas gameOverCanvas;
    public Canvas creditsCanvas;
    public Canvas howToPlayCanvas;
    public Canvas optionsCanvas;

    [Header("Main Menu Canvas Settings")]
    public Button quitToDesktopButton;

    [Header("HUD Canvas Settings")]
    public Slider healthBar;
    public Image grappleCooldownTimer;
    public Text ammoCounter;
    public Text speedometer;
    public Text currentTrack;
    public Image hitmarker;
    public Image pain;

    float hitmarkerTimer = 0.15f;
    float painTimer = 0.25f;
    float t = 0.0f;
    float p = 0.0f;
    float _grappleCooldown;

    PlayerHealth playerHealth;
    PlayerMovePhys playerMove;

    [Header("Game Over Canvas Settings")]
    public TMP_Text finalStatsText;

    [Header("Options Canvas Settings")]
    public Slider sensitivitySlider;
    public TMP_Text sliderText;

    public void HideAllCanvases()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        HUDCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        creditsCanvas.gameObject.SetActive(false);
        howToPlayCanvas.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
    }

    public void InitHUD()
    {
        HideAllCanvases();
        HUDCanvas.gameObject.SetActive(true);
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMove = FindObjectOfType<PlayerMovePhys>();
        healthBar.maxValue = playerHealth.health;
        grappleCooldownTimer.fillAmount = 0f;
    }

    public void ShowCredits()
    {
        HideAllCanvases();
        creditsCanvas.gameObject.SetActive(true);
    }

    public void ShowHowToPlay()
    {
        HideAllCanvases();
        howToPlayCanvas.gameObject.SetActive(true);
    }

    public void ShowOptions()
    {
        HideAllCanvases();
        optionsCanvas.gameObject.SetActive(true);
    }

    public void ShowFinalStats()
    {
        finalStatsText.text = "You survived " + GameManager.timer.ToString("F2") + " seconds. Final score: " + GameManager.totalScore;
    }

    public void ShowHitmarker()
    {
        t = Time.time + hitmarkerTimer;
    }

    public void ShowPain()
    {
        p = Time.time + painTimer;
    }

    void Start()
    {
        //playerHealth = FindObjectOfType<PlayerHealth>();
        //playerMove = FindObjectOfType<PlayerMovePhys>();
        //healthBar.maxValue = playerHealth.health;
        //grappleCooldownTimer.fillAmount = 0f;

        //Remove the ability to quit to desktop in WebGL build, since you're already on the desktop
        //and attempting to quit will freeze the game.
        //The entire reason I'm even making a WebGL build is for a stupid class requirement.
#if UNITY_WEBGL
        quitToDesktopButton.gameObject.SetActive(false);
#endif
    }

    void Update()
    {
        //call InitHUD() somewhere before doing this.
        if(GameManager.GM.gameState == GameManager.GameState.Playing && playerMove != null)
        {
            if (playerMove.successfulGrappleCooldown > 0f)
            {
                _grappleCooldown = NormalizeFloat(playerMove.successfulGrappleCooldown, playerMove.sgReset);
                grappleCooldownTimer.color = Color.white;
            }
            else
            {
                _grappleCooldown = NormalizeFloat(playerMove.failedGrappleCooldown, playerMove.fgReset);
                grappleCooldownTimer.color = Color.red;
            }

            grappleCooldownTimer.fillAmount = _grappleCooldown;
            healthBar.value = playerHealth.health;

            if(Time.time < t)
            {
                hitmarker.enabled = true;
            }
            else
            {
                hitmarker.enabled = false;
            }

            if(Time.time < p)
            {
                pain.enabled = true;
            }
            else
            {
                pain.enabled = false;
            }
        }
    }

    float NormalizeFloat(float input, float max)
    {
        if (input == 0f) return 0f;
        return input / max;
    }
}
