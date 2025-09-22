using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BGManager : MonoBehaviour
{
    public bool running = false;
    public float runSpeed = 0f;
    public float runSpeedScalar = 0f;
    private float repeatWidth;
    private Vector2 startPos;
    private Coroutine speedLerpRoutine;

    void Start()
    {
        startPos = transform.position;
        repeatWidth = GetComponent<BoxCollider2D>().size.x / 2;
    }

    void Update()
    {
        if (running)
        {
            if (transform.position.x < startPos.x - repeatWidth)
            {
                transform.position = startPos;
            }
            transform.Translate(runSpeed * runSpeedScalar * Time.deltaTime * Vector3.left);
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

    private void SetRunning()
    {
        running = true;
    }

    private void PlayerDied()
    {
        StartCoroutine(LerpToNewSpeed(runSpeedScalar, 0f, runSpeedScalar));
    }

    private void InitializeRunSpeed(float value)
    {
        runSpeed = value;
    }

    private void InitializeRunScalar(float value)
    {
        runSpeedScalar = value;
    }

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

    private void ReactToGameSpeedChange(float newScalar)
    {
        // stop any previous lerp so they don’t stack
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(runSpeedScalar, newScalar, 1f));
    }
}
