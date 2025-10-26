using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Manages instantiation of game obstacles, using scriptable objects Bracket1, Bracket2, Bracket3
    // ----------------------------------------------------------------------------------------------

    public bool running = false;
    private float groundLevel = 0f;

    // Game variables received by GameManager
    public int gameLevel = -1;
    public float runSpeed = 0f;
    public float runSpeedScalar = 0f;

    // Bracket scripts that contain predetermined spawn points by distance traveled
    public FixedBracket bracket1;
    public FixedBracket bracket2;
    public FixedBracket bracket3;
    private FixedBracket currentFixedBracket;
    private int nextSpawnIndex = 0;
    public GameObject distanceTracker;
    public float distanceTraveled = 0f;

    public EndlessAlgorithm endlessMode;

    void Start()
    {
        // StartCoroutine(TestAdjust());
    }

    void Update()
    {
        distanceTraveled = transform.position.x - distanceTracker.transform.position.x - 16.0f; // The offset distance between the spawner and the player

        // Only act if running
        if (running)
        {
            if (currentFixedBracket != null)
            {
                SpawnObstacle();
            }
            else
            {
                endlessMode.UpdateDistance(distanceTraveled);
            }
        }
    }

    void SpawnObstacle()
    {
        while (nextSpawnIndex < currentFixedBracket.sequence.Length &&
        distanceTraveled >= currentFixedBracket.sequence[nextSpawnIndex].triggerDistance)
        {
            var evt = currentFixedBracket.sequence[nextSpawnIndex];
            Vector3 spawnPos = new(10.0f, groundLevel + evt.yPosition, 0);
            GameObject nextObstacle = Instantiate(evt.obstaclePrefab, spawnPos, evt.obstaclePrefab.transform.rotation);

            // Grab obstacle travel component and set values
            ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
            obstacleSettings.traveling = true;
            obstacleSettings.baseSpeed = runSpeed;
            obstacleSettings.scalar = runSpeedScalar;
            obstacleSettings.offsetScalar = evt.speedOffsetScalar;

            // Apply parameters specific to Saw
            if (evt.obstaclePrefab.name == "Saw")
            {
                Vector3 sawScale = nextObstacle.transform.localScale;
                sawScale.x *= evt.sawScale;
                sawScale.y *= evt.sawScale;
                nextObstacle.transform.localScale = sawScale;
            }

            // Apply parameters specific to Mine
            if (evt.obstaclePrefab.name == "Mine")
            {
                Transform laserLength = nextObstacle.transform.Find("Scale");
                if (laserLength != null)
                {
                    Vector3 s = laserLength.localScale;
                    s.y = evt.laserLength;
                    laserLength.localScale = s;
                }
            }

            // Apply parameters specific to Turret
            if (evt.obstaclePrefab.name == "Turret")
            {
                TurretAction turretAction = nextObstacle.GetComponent<TurretAction>();
                if (turretAction != null)
                {
                    turretAction.xPosForAction = evt.turretXPos;
                }
            }

            nextSpawnIndex++;
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.START_RUN, ToggleRun);
        Messenger<int>.AddListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.AddListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger.AddListener(GameEvent.PLAYER_DIED, EndRun);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.START_RUN, ToggleRun);
        Messenger<int>.RemoveListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.RemoveListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, EndRun);
    }

    void EndRun()
    {
        StartCoroutine(AllowMovementStop());
    }

    // Toggle running, -- from UIBracketMode
    private void ToggleRun()
    {
        if (running) running = false;
        else running = true;
    }

    void UpdateBestScore()
    {
        if (gameLevel == 2)
        {
            float previousBest = PlayerPrefs.GetFloat("BestSilver", 0f);
            if (previousBest < distanceTraveled) PlayerPrefs.SetFloat("BestSilver", distanceTraveled);
        }

        if (gameLevel == 4)
        {
            float previousBest = PlayerPrefs.GetFloat("BestEndless", 0f);
            if (previousBest < distanceTraveled) PlayerPrefs.SetFloat("BestEndless", distanceTraveled);
        }
    }

    private void UpdateEarnings()
    {
        float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
        PlayerPrefs.SetFloat("Money", currentMoney + distanceTraveled);
    }

    // Set game level, -- from GameManager
    private void InitializeLevel(int number)
    {
        gameLevel = number;

        // Set current bracket
        if (gameLevel == 1) currentFixedBracket = bracket1;
        else if (gameLevel == 2) currentFixedBracket = bracket2;
        else if (gameLevel == 3) currentFixedBracket = bracket3;
        else if (gameLevel == 4)
        {
            currentFixedBracket = null;
            endlessMode.InitializeEndless();
        }
        else Debug.Log("Incorrect or no game level received by SpawnManager");
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

    // Set the ground level as the spawn origin, -- from PlayerController (GameManager should probably handle this)
    private void SetGroundHeight(float value)
    {
        groundLevel = value;
    }

    public IEnumerator AllowMovementStop()
    {
        ToggleRun();
        yield return new WaitForSeconds(1.1f);

        UpdateBestScore();
        UpdateEarnings();
    }
}
