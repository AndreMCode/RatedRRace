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
    [SerializeField] TextMeshProUGUI distanceTxt;
    [SerializeField] TextMeshProUGUI countdownTxt;
    [SerializeField] TextMeshProUGUI gameOverTxt;
    [SerializeField] GameObject retryTxt;
    public float uiUpdateInterval = 0.1f;
    private float uiUpdateTimer = 0f;

    void Start()
    {
        countdownTxt.enabled = false;
        gameOverTxt.enabled = false;
        retryTxt.SetActive(false);
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
        DistanceCounterUpdate();
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    // Display game over message
    void PlayerDied()
    {
        gameOverTxt.enabled = true;
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
