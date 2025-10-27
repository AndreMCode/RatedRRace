using System.Collections;
using UnityEngine;

public class MovementScroller : MonoBehaviour
{
    // This moves background images from right to left to simulate movement
    // Can be modified to produce a parallax effect with additional layers
    // --------------------------------------------------------------------

    [SerializeField] GameObject foreground;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject mountainFront;
    [SerializeField] GameObject mountain;
    [SerializeField] GameObject sky;
    public bool running = false;
    public float runSpeed = 0f;
    public float runSpeedScalar = 0f;
    public float fgFactor = 1.2f;
    public float mtnFrontFactor = 0.2f;
    public float mtnFactor = 0.2f;
    public float skyFactor = 0.1f;
    private Vector2 startPos;
    public float bgRepeatWidth = 40.96f;
    private float xLimit;
    private Coroutine speedLerpRoutine;

    void Start()
    {
        // Gather asset positions and dimensions
        startPos = transform.position;
        xLimit = startPos.x - bgRepeatWidth;
    }

    void Update()
    {
        if (running)
        {
            // Check position and reposition if half-length reached
            if (foreground.transform.position.x < xLimit)
            {
                foreground.transform.position = new Vector2(startPos.x, foreground.transform.position.y);
            }
            if (ground.transform.position.x < xLimit)
            {
                ground.transform.position = new Vector2(startPos.x, ground.transform.position.y);
            }
            if (mountainFront.transform.position.x < xLimit)
            {
                mountainFront.transform.position = new Vector2(startPos.x, mountainFront.transform.position.y);
            }
            if (mountain.transform.position.x < xLimit)
            {
                mountain.transform.position = new Vector2(startPos.x, mountain.transform.position.y);
            }
            if (sky.transform.position.x < xLimit)
            {
                sky.transform.position = new Vector2(startPos.x, sky.transform.position.y);
            }

            // Move foreground at increased run speed
            foreground.transform.Translate(runSpeed * runSpeedScalar * fgFactor * Time.deltaTime * Vector2.left);

            // Move ground at full run speed
            ground.transform.Translate(runSpeed * runSpeedScalar * Time.deltaTime * Vector2.left);

            // Move behindGround at near-full run speed
            mountainFront.transform.Translate(runSpeed * runSpeedScalar * mtnFrontFactor * Time.deltaTime * Vector2.left);

            // Move background at a decreased factor of current run speed
            mountain.transform.Translate(runSpeed * runSpeedScalar * mtnFactor * Time.deltaTime * Vector2.left);

            // Move background at a decreased factor of current run speed
            sky.transform.Translate(runSpeed * runSpeedScalar * skyFactor * Time.deltaTime * Vector2.left);
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.START_RUN, SetRunning);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.AddListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.START_RUN, SetRunning);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.RemoveListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    // Toggle running, -- from UIBracketMode
    private void SetRunning()
    {
        if (running) running = false;
        else running = true;
    }

    // Slows this object to a stop when player dies, -- from PlayerHealth
    private void PlayerDied()
    {
        StartCoroutine(LerpToNewSpeed(runSpeedScalar, 0f, runSpeedScalar));
    }

    // Set base run speed, -- from GameManager
    private void InitializeRunSpeed(float value)
    {
        runSpeed = value;
    }

    // Set the scalar to calculate the current level run speed, -- from GameManager
    private void InitializeRunScalar(float value)
    {
        runSpeedScalar = value;
    }

    // Changes the speed of this object over a specified length of time
    private IEnumerator LerpToNewSpeed(float startScalar, float targetScalar, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            runSpeedScalar = Mathf.Lerp(startScalar, targetScalar, elapsed / duration);
            yield return null;
        }

        runSpeedScalar = targetScalar; // snap exactly to target
    }

    // Adjust run speed over time mid-run, -- from SpawnManager
    private void ReactToGameSpeedChange(float newScalar)
    {
        // stop any previous lerp so they don’t stack
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

        // Use 1 second as the transition duration
        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(runSpeedScalar, newScalar, 1f));
    }
}
