using System.Collections;
using UnityEngine;

public class ObstacleTravel : MonoBehaviour
{
    public float baseSpeed = 7f;
    public float scalar = 1f;
    public float xBound = -10f;

    private Coroutine speedLerpRoutine;

    void Start()
    {
        // StartCoroutine(TestAdjust());
    }

    void Update()
    {
        transform.Translate(baseSpeed * scalar * Time.deltaTime * Vector3.left);

        if (transform.position.x < xBound)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
    }

    void OnDisable()
    {
        Messenger<float>.AddListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
    }

    private IEnumerator TestAdjust()
    {
        yield return new WaitForSeconds(1f);

        ReactToGameSpeedChange(2f);
    }

    private IEnumerator LerpToNewSpeed(float startScalar, float targetScalar, float duration)
    {
        // adjustingSpeed = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scalar = Mathf.Lerp(startScalar, targetScalar, elapsed / duration);
            yield return null;
        }

        scalar = targetScalar; // snap exactly to target
        // adjustingSpeed = false;
    }

    private void ReactToGameSpeedChange(float newScalar)
    {
        // stop any previous lerp so they don’t stack
        if (speedLerpRoutine != null)
            StopCoroutine(speedLerpRoutine);

        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(scalar, newScalar, 1f));
    }
}
