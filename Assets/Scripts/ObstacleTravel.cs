using System.Collections;
using UnityEngine;

public class ObstacleTravel : MonoBehaviour
{
    // Manages simulated obstacle movement across the screen and checks bounds
    // -----------------------------------------------------------------------

    public bool traveling = false;
    public float baseSpeed = 5f;
    public float offsetScalar = 1f;
    public float scalar = 1f;
    public float xBound = -10f;
    public float yBound = 7f;

    private Coroutine speedLerpRoutine;

    void Update()
    {
        // Travel as long as traveling is true
        if (traveling) transform.Translate(baseSpeed * offsetScalar * scalar * Time.deltaTime * Vector3.left);

        // Self-destruct if out of bounds
        if (transform.position.x < xBound || transform.position.y > yBound || transform.position.y < -yBound)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    // Slows this object to a stop when player dies, -- from PlayerHealth
    private void PlayerDied()
    {
        // Use the scalar as the duration since it will be around 1 to 1.4
        StartCoroutine(LerpToNewSpeed(scalar, 0f, scalar));
    }

    // Changes the speed of this object over a specified length of time
    private IEnumerator LerpToNewSpeed(float startScalar, float targetScalar, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scalar = Mathf.Lerp(startScalar, targetScalar, elapsed / duration);
            yield return null;
        }

        scalar = targetScalar; // snap exactly to target
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
        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(scalar, newScalar, 0.4f));
    }
}
