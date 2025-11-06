using System.Collections;
using UnityEngine;

public class DistanceTrackerTravel : MonoBehaviour
{
    // Controls movement of distance tracker object
    // --------------------------------------------

    public bool running = false;
    public float runSpeed = 0f;
    public float runSpeedScalar = 0f;

    private Coroutine speedLerpRoutine;

    void Update()
    {
        // Travel left as long as running is true
        if (running) transform.Translate(runSpeed * runSpeedScalar * Time.deltaTime * Vector3.left);
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
        Messenger<float>.Broadcast(GameEvent.UI_SET_RUN_RATE, runSpeed * runSpeedScalar);
    }

    // Slows this object to a stop when player dies, -- from PlayerHealth
    private void PlayerDied()
    {
        // Overwrite any active lerp
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

        // Use the scalar as the duration since it will be around 1 to 1.4
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
        Messenger<float>.Broadcast(GameEvent.UI_SET_RUN_RATE, runSpeed * runSpeedScalar);
    }

    // Adjust run speed over time mid-run, -- from SpawnManager
    private void ReactToGameSpeedChange(float newScalar)
    {
        // Stop any previous lerp so they don’t stack
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

        // Use 1 second as the transition duration
        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(runSpeedScalar, newScalar, 0.4f));
    }
}
