using TMPro;
using UnityEngine;
using System.Collections;

public class UIBracketMode : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    [SerializeField] SpawnManager distanceSource;
    [SerializeField] TextMeshProUGUI distanceTxt;
    [SerializeField] TextMeshProUGUI countdownTxt;
    [SerializeField] TextMeshProUGUI gameOverTxt;
    public float uiUpdateInterval = 0.1f;
    private float uiUpdateTimer = 0f;

    // Previous variable for whole number display version
    // private int lastDisplayedDistance = -1;

    void Start()
    {
        countdownTxt.enabled = false;
        gameOverTxt.enabled = false;
        StartCoroutine(InitialCountdown());
    }

    void Update()
    {
        uiUpdateTimer += Time.deltaTime;
        if (uiUpdateTimer >= uiUpdateInterval)
        {
            uiUpdateTimer = 0f;
            UpdateDistanceText();
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void PlayerDied()
    {
        gameOverTxt.enabled = true;
    }

    private void UpdateDistanceText()
    {
        if (distanceSource == null || distanceTxt == null) return;

        // Previous version if we prefer whole numbers only
        // int display = Mathf.FloorToInt(distanceSource.distanceTraveled);
        // if (display != lastDisplayedDistance)
        // {
        //     lastDisplayedDistance = display;
        //     distanceTxt.text = display.ToString() + "m";
        // }

        // Decimal to two places for added effect version
        float display = distanceSource.distanceTraveled;
        distanceTxt.text = display.ToString("F2");
    }

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
}
