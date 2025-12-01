using System.Collections;
using UnityEngine;

public class MovementScroller : MonoBehaviour
{
    // This moves background images from right to left to simulate movement
    // Can be modified to produce a parallax effect with additional layers
    // --------------------------------------------------------------------

    // Transform of GameObject containing the image to scroll and the relative speed factor
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform transform;
        public float factor = 1f;
    }
    [SerializeField] ParallaxLayer[] layers;
    public float bgRepeatWidth = 40.96f;

    private bool running = false;
    private float runSpeed = 0f;
    private float runSpeedScalar = 0f;
    private Vector2 startPos;
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
        if (!running) return;

        // Precompute left movement per second multiplier
        float baseMove = runSpeed * runSpeedScalar * Time.deltaTime;
        Vector2 left = Vector2.left;

        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            if (layer.transform == null) continue;

            // Reset position when it moves past xLimit
            if (layer.transform.position.x < xLimit)
            {
                float offset = Mathf.Abs(layer.transform.position.x - xLimit);
                layer.transform.position = new Vector2(startPos.x - offset, layer.transform.position.y);
            }

            // Translate with the layer-specific factor
            float factor = layer.factor;
            layer.transform.Translate(baseMove * factor * left);
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.START_RUN, SetRunning);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.AddListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger.AddListener(GameEvent.PLAYER_WON, PlayerDied);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.START_RUN, SetRunning);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.RemoveListener(GameEvent.ADJ_RUN_SPEED, ReactToGameSpeedChange);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger.RemoveListener(GameEvent.PLAYER_WON, PlayerDied);
    }

    // Toggle running, -- from UIBracketMode
    private void SetRunning()
    {
        if (running) running = false;
        else running = true;
    }

    // Slows this object to a stop when player dies or wins, -- from PlayerHealth
    private void PlayerDied()
    {
        // Overwrite any active lerp
        if (speedLerpRoutine != null)
        {
            StopCoroutine(speedLerpRoutine);
            speedLerpRoutine = null;
        }

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
        speedLerpRoutine = StartCoroutine(LerpToNewSpeed(runSpeedScalar, newScalar, 0.4f));
    }
}
