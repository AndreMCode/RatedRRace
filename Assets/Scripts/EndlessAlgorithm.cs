using System.Collections;
using UnityEngine;

public class EndlessAlgorithm : MonoBehaviour
{
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject sawPrefab;
    [SerializeField] GameObject minePrefab;
    [SerializeField] GameObject turretPrefab;
    private GameObject randomObstacle;

    [Header("Spawn Probabilities")]
    [Range(0f, 1f)] public float boxSpawnChance = 0.4f;
    [Range(0f, 1f)] public float sawSpawnChance = 0.3f;
    [Range(0f, 1f)] public float mineSpawnChance = 0.2f;
    [Range(0f, 1f)] public float turretSpawnChance = 0.1f;

    private float runSpeed = 5.0f;
    private float storedSawChance;
    private float storedMineChance;
    private float storedTurretChance;
    private bool twoObstaclesNearby = false;
    private bool twoObstaclesFar = false;

    public float groundLevel = 0f;
    public bool endless = false;
    public float distance = 0f;
    public float nextSpawnPoint = 10.0f;
    public float runScalar = 1.2f;
    public float speedupDistance = 500f;
    public float speedupIncrement = 0.02f;
    public float nextSpeedupPoint = 0f;

    public float lowEndThreshold = 2.0f;
    public float nextSpawnMin = 0.5f;
    public float nextSpawnMax = 12.0f;
    public float nextSpawnIncrement = 0.25f;
    public float highEndThreshold = 8.0f;

    public float unlockSawDistance = 1000.0f;
    public float unlockMineDistance = 500.0f;
    public float unlockTurretDistance = 2000.0f;

    public float boxHeightMin = 0.5f;
    public float boxHeightMax = 3.0f;

    public float sawHeightMin = 0f;
    public float sawHeightMax = 4.0f;
    public float sawScaleMin = 1.2f;
    public float sawScaleMax = 2.5f;
    public float sawSpeedOffsetMin = 0.6f;
    public float sawSpeedOffsetMax = 1.6f;

    public float mineLengthMin = 0.5f;
    public float mineLengthMax = 3.0f;

    public float turretHeightMin = 3.0f;
    public float turretHeightMax = 8.0f;
    public float turretMinX = -5.0f;
    public float turretMaxX = 2.0f;
    public float turretSpeedOffsetMin = 0.6f;
    public float turretSpeedOffsetMax = 1.6f;

    void Start()
    {
        storedSawChance = sawSpawnChance;
        storedMineChance = mineSpawnChance;
        storedTurretChance = turretSpawnChance;

        sawSpawnChance = 0f;
        mineSpawnChance = 0f;
        turretSpawnChance = 0f;

        nextSpeedupPoint += speedupDistance;
    }

    void Update()
    {
        if (endless)
        {
            if (distance > nextSpeedupPoint)
            {
                nextSpawnPoint += nextSpeedupPoint;
                StartCoroutine(IncrementRunSpeed());
                nextSpeedupPoint += speedupDistance;
            }
            if (distance > nextSpawnPoint)
            {
                SelectNextObstacle();
                IDAndSpawnObstacle();
                SetNextSpawnPoint();
            }
        }
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);

    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
    }

    private IEnumerator IncrementRunSpeed()
    {
        runScalar += speedupIncrement;
        Messenger<float>.Broadcast(GameEvent.ADJ_RUN_SPEED, runScalar);

        nextSpawnMax += nextSpawnIncrement;
        speedupDistance += 100f;

        yield return new WaitForSeconds(1f);
        SetNextSpawnPoint();
    }

    void IDAndSpawnObstacle()
    {
        if (randomObstacle.name == "Box")
        {
            SpawnBox();
            return;
        }

        if (randomObstacle.name == "Saw")
        {
            SpawnSaw();
            return;
        }

        if (randomObstacle.name == "Mine")
        {
            SpawnMine();
            return;
        }

        if (randomObstacle.name == "Turret")
        {
            SpawnTurret();
        }
    }

    void SpawnBox()
    {
        Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(boxHeightMin, boxHeightMax), 0);
        GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Set common parameters
        ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
        obstacleSettings.traveling = true;
        obstacleSettings.baseSpeed = runSpeed;
        obstacleSettings.scalar = runScalar;
    }

    void SpawnSaw()
    {
        Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(sawHeightMin, sawHeightMax), 0);
        GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Set common parameters
        ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
        obstacleSettings.traveling = true;
        obstacleSettings.baseSpeed = runSpeed;
        obstacleSettings.scalar = runScalar;

        // Set unique parameters
        float newSawScale = Random.Range(sawScaleMin, sawScaleMax);
        Vector3 sawScale = nextObstacle.transform.localScale;
        sawScale.x *= newSawScale;
        sawScale.y *= newSawScale;
        nextObstacle.transform.localScale = sawScale;
        obstacleSettings.offsetScalar = Random.Range(sawSpeedOffsetMin, sawSpeedOffsetMax);
    }

    void SpawnMine()
    {
        Vector3 spawnPos = new(10.0f, groundLevel, 0);
        GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Set common parameters
        ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
        obstacleSettings.traveling = true;
        obstacleSettings.baseSpeed = runSpeed;
        obstacleSettings.scalar = runScalar;

        // Set unique parameters
        Transform laserLength = nextObstacle.transform.Find("Scale");
        if (laserLength != null)
        {
            Vector3 s = laserLength.localScale;
            s.y = Random.Range(mineLengthMin, mineLengthMax);
            laserLength.localScale = s;
        }
    }

    void SpawnTurret()
    {
        Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(turretHeightMin, turretHeightMax), 0);
        GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Set common parameters
        ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
        obstacleSettings.traveling = true;
        obstacleSettings.baseSpeed = runSpeed;
        obstacleSettings.scalar = runScalar;

        // Set unique parameters
        obstacleSettings.offsetScalar = Random.Range(turretSpeedOffsetMin, turretSpeedOffsetMax);
        if (nextObstacle.TryGetComponent<TurretAction>(out var turretAction))
        {
            turretAction.xPosForAction = Random.Range(turretMinX, turretMaxX);
        }
    }

    void SelectNextObstacle()
    {
        // Calculate the combined weight (sum of all spawn chances)
        float total = boxSpawnChance + sawSpawnChance + mineSpawnChance + turretSpawnChance;

        // If all chances are zero or negative, assign equal probabilities to each obstacle
        if (total <= 0f)
        {
            Debug.LogWarning("All spawn chances are 0! Using fallback.");
            boxSpawnChance = sawSpawnChance = mineSpawnChance = turretSpawnChance = 0.25f;
            total = 1f; // Normalized total (each chance sums to 1)
        }

        // Generate a random value between 0 and the total weight
        float random = Random.value * total;

        // Choose the obstacle based on cumulative weighted ranges
        if (random < boxSpawnChance)
        {
            randomObstacle = boxPrefab;
        }
        else if (random < boxSpawnChance + sawSpawnChance)
        {
            randomObstacle = sawPrefab;
        }
        else if (random < boxSpawnChance + sawSpawnChance + mineSpawnChance)
        {
            randomObstacle = minePrefab;
        }
        else
        {
            randomObstacle = turretPrefab;
        }
    }

    void SetNextSpawnPoint()
    {
        if (twoObstaclesNearby)
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMax * 0.5f, nextSpawnMax);
            twoObstaclesNearby = false;

            Debug.Log("Enforced spawn rule twoObstaclesNearby");
        }
        else if (twoObstaclesFar)
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMin, nextSpawnMax * 0.5f);
            twoObstaclesFar = false;

            Debug.Log("Enforced spawn rule twoObstaclesFar");
        }
        else
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMin, nextSpawnMax);
        }

        if (nextSpawnPoint - distance < lowEndThreshold) twoObstaclesNearby = true;
        if (nextSpawnPoint - distance > highEndThreshold) twoObstaclesFar = true;

        if (distance > unlockSawDistance)
        {
            sawSpawnChance = storedSawChance;
        }

        if (distance > unlockMineDistance)
        {
            mineSpawnChance = storedMineChance;
        }

        if (distance > unlockTurretDistance)
        {
            turretSpawnChance = storedTurretChance;
        }
    }

    public void InitializeEndless()
    {
        endless = true;
    }

    // Set base run speed, -- from GameManager
    private void InitializeRunSpeed(float value)
    {
        runSpeed = value;
    }

    // Set the scalar to calculate the current level run speed, -- from GameManager
    private void InitializeRunScalar(float value)
    {
        runScalar = value;
    }

    void SetGroundHeight(float baseHeight)
    {
        groundLevel = baseHeight;
    }

    public void UpdateDistance(float value)
    {
        distance = value;
    }
}
