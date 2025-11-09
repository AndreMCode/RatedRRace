using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class UIBracketMode : MonoBehaviour
{
    // Display text, update distance counter, listen for non-player-action inputs
    // --------------------------------------------------------------------------

    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    private static WaitForSeconds _waitForSeconds1 = new(1f);
    [SerializeField] SpawnManager distanceSource;
    [SerializeField] TextMeshProUGUI bubbleCountTxt;
    [SerializeField] TextMeshProUGUI fundsTxt;
    [SerializeField] TextMeshProUGUI bestRunTxt;
    [SerializeField] TextMeshProUGUI distanceTxt;
    [SerializeField] TextMeshProUGUI speedTxt;
    [SerializeField] TextMeshProUGUI countdownTxt;
    [SerializeField] GameObject pauseWindow;
    [SerializeField] GameObject gameOverWindow;
    [SerializeField] GameObject earningsTxt;
    [SerializeField] GameObject totalTxt;
    public int gameLevel;
    private int playerDefense;
    private float bonus;
    public bool isPaused = false;
    public bool isAlive = false;
    private bool earningsUpdated = false;

    public InputAction pauseAction;
    public InputAction restartRunAction;
    public InputAction goToMainMenuAction;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;       

        pauseWindow.SetActive(false);
        countdownTxt.enabled = false;
        gameOverWindow.SetActive(false);

        DisplayFunds();

        StartCoroutine(InitialCountdown());
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame() && isAlive)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (restartRunAction.WasPressedThisFrame() && (isPaused || earningsUpdated))
        {
            if (isPaused) Time.timeScale = 1f;
            ReloadScene();
        }

        if (goToMainMenuAction.WasPressedThisFrame() && (isPaused || earningsUpdated))
        {
            if (isPaused) Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        // Update the distance counter display
        UpdateDistanceText();
    }

    void OnEnable()
    {
        pauseAction.Enable();
        restartRunAction.Enable();
        goToMainMenuAction.Enable();
        Messenger<int>.AddListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<int>.AddListener(GameEvent.SET_HEALTH, SetDefense);
        Messenger<float>.AddListener(GameEvent.UI_SET_RUN_RATE, UpdateRunRate);
        Messenger.AddListener(GameEvent.UI_DECREMENT_BUBBLE, DecrementBubbleCount);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<float>.AddListener(GameEvent.UI_UPDATE_EARNINGS, UpdateEarnings);
        Messenger<float>.AddListener(GameEvent.UI_UPDATE_BONUS, UpdateBonus);
    }

    void OnDisable()
    {
        pauseAction.Disable();
        restartRunAction.Disable();
        goToMainMenuAction.Disable();
        Messenger<int>.RemoveListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<int>.RemoveListener(GameEvent.SET_HEALTH, SetDefense);
        Messenger<float>.RemoveListener(GameEvent.UI_SET_RUN_RATE, UpdateRunRate);
        Messenger.RemoveListener(GameEvent.UI_DECREMENT_BUBBLE, DecrementBubbleCount);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<float>.RemoveListener(GameEvent.UI_UPDATE_EARNINGS, UpdateEarnings);
        Messenger<float>.RemoveListener(GameEvent.UI_UPDATE_BONUS, UpdateBonus);
    }

    void DisplayBestRun()
    {
        if (gameLevel == 2)
        {
            float best = PlayerPrefs.GetFloat("BestSilver", 0f);
            bestRunTxt.text = "Best Run: " + best.ToString("F2") + "m";
        }

        if (gameLevel == 4)
        {
            float best = PlayerPrefs.GetFloat("BestEndless", 0f);
            bestRunTxt.text = "Best Run: " + best.ToString("F2") + "m";
        }
    }

    void DisplayFunds()
    {
        fundsTxt.text = "$ " + PlayerPrefs.GetFloat("Money", 0).ToString("F2");
    }

    private void InitializeLevel(int number)
    {
        gameLevel = number;
        DisplayBestRun();
    }

    void SetDefense(int defense)
    {
        playerDefense = defense;
        if (defense > 0) bubbleCountTxt.text = playerDefense.ToString();
    }

    void UpdateRunRate(float value)
    {
        value *= 3.6f;
        speedTxt.text = value.ToString("F1") + " km/h";
    }

    void DecrementBubbleCount()
    {
        if (playerDefense > 0) playerDefense--;
        bubbleCountTxt.text = playerDefense.ToString();
    }

    void UpdateBonus(float value)
    {
        bonus = value;
    }

    void UpdateEarnings(float value)
    {
        earningsUpdated = true;

        if (!earningsTxt.TryGetComponent<TextMeshProUGUI>(out var retryMsg)) return;

        retryMsg.text = "Earnings: $" + value.ToString("F2") + "  |  Bonus $" + bonus.ToString("F2");

        if (!totalTxt.TryGetComponent<TextMeshProUGUI>(out var totalMsg)) return;

        totalMsg.text = "+$" + (value + bonus).ToString("F2");
    }

    // Display game over message
    void PlayerDied()
    {
        gameOverWindow.SetActive(true);
        DisablePause();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClickNextRun()
    {
        if (isPaused) Time.timeScale = 1f;
        ReloadScene();
    }

    public void OnClickMainMenu()
    {
        if (isPaused) Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Update the distance text object
    private void UpdateDistanceText()
    {
        if (distanceSource == null || distanceTxt == null) return;

        // Decimal to two places
        float display = distanceSource.distanceTraveled;
        distanceTxt.text = display.ToString("F2");
    }

    // Display countdown at run start
    private IEnumerator InitialCountdown()
    {
        int countdown = 3;
        yield return _waitForSeconds1;
        Messenger.Broadcast(GameEvent.PLAYER_READY);
        countdownTxt.enabled = true;
        countdownTxt.text = countdown.ToString();
        yield return _waitForSeconds0_5;
        countdown--;
        countdownTxt.text = countdown.ToString();
        yield return _waitForSeconds0_5;
        countdown--;
        countdownTxt.text = countdown.ToString();
        yield return _waitForSeconds0_5;
        countdownTxt.text = "GO!";

        // Current listeners: DistanceTracker, BGManager, SpawnManager
        Messenger.Broadcast(GameEvent.START_RUN);
        EnablePause();

        yield return _waitForSeconds1;
        countdownTxt.enabled = false;
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        pauseWindow.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Messenger.Broadcast(GameEvent.PLAYER_TOGGLE_CONTROLS);
        Messenger.Broadcast(GameEvent.UI_AUDIO_ADJUST_VOL);
        Messenger.Broadcast(GameEvent.PLAYER_PAUSE_AUDIO);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        pauseWindow.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Messenger.Broadcast(GameEvent.PLAYER_TOGGLE_CONTROLS);
        Messenger.Broadcast(GameEvent.UI_AUDIO_ADJUST_VOL);
    }

    void EnablePause()
    {
        isAlive = true;
    }

    void DisablePause()
    {
        isAlive = false;
    }

    // Restart the run
    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}