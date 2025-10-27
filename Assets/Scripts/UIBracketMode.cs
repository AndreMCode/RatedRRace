using TMPro;
using UnityEngine;
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
    [SerializeField] GameObject gameOverTxt;
    [SerializeField] GameObject retryTxt;
    public float uiUpdateInterval = 0.1f;
    private float uiUpdateTimer = 0f;
    public int gameLevel;
    private int playerDefense;

    void Start()
    {
        countdownTxt.enabled = false;
        gameOverTxt.SetActive(false);
        retryTxt.SetActive(false);

        DisplayFunds();

        StartCoroutine(InitialCountdown());
    }

    void Update()
    {
        // Temporary for prototype, used to restart the run
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        // Temporary for prototype, used to return to the Main Menu
        if (Input.GetKeyDown(KeyCode.M))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        // Update the distance counter display
        // DistanceCounterUpdate();
        UpdateDistanceText();
    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<int>.AddListener(GameEvent.SET_HEALTH, SetDefense);
        Messenger<float>.AddListener(GameEvent.UI_SET_RUN_RATE, UpdateRunRate);
        Messenger.AddListener(GameEvent.UI_DECREMENT_BUBBLE, DecrementBubbleCount);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<int>.RemoveListener(GameEvent.SET_HEALTH, SetDefense);
        Messenger<float>.RemoveListener(GameEvent.UI_SET_RUN_RATE, UpdateRunRate);
        Messenger.RemoveListener(GameEvent.UI_DECREMENT_BUBBLE, DecrementBubbleCount);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void DisplayBestRun()
    {
        if (gameLevel == 2)
        {
            float best = PlayerPrefs.GetFloat("BestSilver", 0f);
            bestRunTxt.text = "Best: " + best.ToString("F2") + "m";
        }

        if (gameLevel == 4)
        {
            float best = PlayerPrefs.GetFloat("BestEndless", 0f);
            bestRunTxt.text = "Best: " + best.ToString("F2") + "m";
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

    // Display game over message
    void PlayerDied()
    {
        gameOverTxt.SetActive(true);
        retryTxt.SetActive(true);
    }

    // Update the counter using the specified interval
    void DistanceCounterUpdate()
    {
        uiUpdateTimer += Time.deltaTime;

        if (uiUpdateTimer >= uiUpdateInterval)
        {
            uiUpdateTimer = 0f;
            UpdateDistanceText();
        }
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

        yield return _waitForSeconds1;
        countdownTxt.enabled = false;
    }

    public void OnClickRetry()
    {
        ReloadScene();
    }

    // Restart the run
    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}