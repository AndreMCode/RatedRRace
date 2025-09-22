using System.Collections;
using UnityEngine;

public class ObstacleTravel : MonoBehaviour
{
    public bool traveling = false;
    public float baseSpeed = 5f;
    public float offsetScalar = 1f;
    public float scalar = 1f;
    public float xBound = -10f;

    private Coroutine speedLerpRoutine;

    void Start()
    {

    }

    void Update()
    {
        if (traveling) transform.Translate((baseSpeed * offsetScalar) * scalar * Time.deltaTime * Vector3.left);

        if (transform.position.x < xBound)
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

    private void PlayerDied()
    {
        StartCoroutine(LerpToNewSpeed(scalar, 0f, scalar));
    }

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

    private void ReactToGameSpeedChange(float newScalar)
    {
        // stop any previous lerp so they don’t stack
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(scalar, newScalar, 1f));
    }
}
