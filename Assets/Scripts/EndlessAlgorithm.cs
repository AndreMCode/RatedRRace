using System.Collections;
using UnityEngine;

public class EndlessAlgorithm : MonoBehaviour
{
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject sawPrefab;
    [SerializeField] GameObject minePrefab;
    [SerializeField] GameObject turretPrefab;
    [SerializeField] GameObject fireballPrefab;
    private GameObject randomObstacle;

    [Header("Spawn Probabilities")]
    [Range(0f, 1f)] public float boxSpawnChance = 0.35f;
    [Range(0f, 1f)] public float sawSpawnChance = 0.1f;
    [Range(0f, 1f)] public float mineSpawnChance = 0.25f;
    [Range(0f, 1f)] public float turretSpawnChance = 0.1f;
    [Range(0f, 1f)] public float fireballSpawnChance = 0.2f;

    private float runSpeed = 5.0f;
    private float storedBoxChance;
    private float storedSawChance;
    private float storedMineChance;
    private float storedTurretChance;
    private float storedFireballChance;
    private bool twoObstaclesNearby = false;
    private bool twoObstaclesFar = false;
    private bool spamming = false;
    private float spamPoint = 0f;

    public bool difficult = false;

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
    public float unlockFireballDistance = 1500.0f;

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

    public float fireballSpeedMin = 0.1f;
    public float fireballSpeedMax = 0.9f;
    public float fireballInflexPointMin = -4.0f;
    public float fireballInflexPointMax = 4.0f;

    void Start()
    {
        storedBoxChance = boxSpawnChance;
        storedSawChance = sawSpawnChance;
        storedMineChance = mineSpawnChance;
        storedTurretChance = turretSpawnChance;
        storedFireballChance = fireballSpawnChance;

        sawSpawnChance = 0f;
        mineSpawnChance = 0f;
        turretSpawnChance = 0f;
        fireballSpawnChance = 0f;

        nextSpeedupPoint += speedupDistance;

        if (difficult) speedupIncrement = 0.01f;
    }

    void Update()
    {
        // Game loop
        if (endless)
        {
            // Check for speed increase
            if (distance > nextSpeedupPoint)
            {
                nextSpawnPoint += nextSpeedupPoint;
                StartCoroutine(IncrementRunSpeed());
                nextSpeedupPoint += speedupDistance;
            }
            // Check for next spawn point
            if (distance > nextSpawnPoint)
            {
                if (!spamming) ChanceForTypeSpam();
                if (spamming) EndTypeSpam();
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

    // Speed adjust coroutine
    private IEnumerator IncrementRunSpeed()
    {
        runScalar += speedupIncrement;
        Messenger<float>.Broadcast(GameEvent.ADJ_RUN_SPEED, runScalar);

        // Increase the distance to the next speed up point
        speedupDistance += 100f;

        // Prototype increased difficulty
        if (difficult)
        {
            // Increase spawn range
            nextSpawnMax += nextSpawnIncrement;
            // Increase the spawn increment
            speedupIncrement += speedupIncrement;
        }

        yield return new WaitForSeconds(0.5f);
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
            return;
        }

        if (randomObstacle.name == "Fireball - offset")
        {
            SpawnFireball();
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

    void SpawnFireball()
    {
        Vector3 spawnPos = new(10.0f, groundLevel + 4.0f, 0);
        GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Apply parameters specific to Fireball
        CubicBZFireball bzPositions = nextObstacle.GetComponentInChildren<CubicBZFireball>();
        if (bzPositions != null)
        {
            bzPositions.SetFireballCurvePoints(Random.Range(fireballInflexPointMin, fireballInflexPointMax), Random.Range(fireballInflexPointMin, fireballInflexPointMax));
            bzPositions.SetFireballSpeed(Random.Range(fireballSpeedMin, fireballSpeedMax));
        }
    }

    void EndTypeSpam()
    {
        // Restore probabilities
        if (distance > spamPoint)
        {
            boxSpawnChance = storedBoxChance;
            sawSpawnChance = storedSawChance;
            mineSpawnChance = storedMineChance;
            turretSpawnChance = storedTurretChance;
            fireballSpawnChance = storedFireballChance;

            spamming = false;
        }
    }

    void BoxSpam()
    {
        spamming = true;
        spamPoint = distance + 50.0f;
        sawSpawnChance = 0f;
        mineSpawnChance = 0f;
        turretSpawnChance = 0f;
        fireballSpawnChance = 0f;

        Debug.Log("Spamming Boxes for 50m!");
    }

    void SawSpam()
    {
        spamming = true;
        spamPoint = distance + 50.0f;
        boxSpawnChance = 0f;
        mineSpawnChance = 0f;
        turretSpawnChance = 0f;
        fireballSpawnChance = 0f;

        Debug.Log("Spamming Saws for 50m!");
    }

    void TurretSpam()
    {
        spamming = true;
        spamPoint = distance + 50.0f;
        boxSpawnChance = 0f;
        sawSpawnChance = 0f;
        mineSpawnChance = 0f;
        fireballSpawnChance = 0f;

        Debug.Log("Spamming Turrets for 50m!");
    }

    void FireballSpam()
    {
        spamming = true;
        spamPoint = distance + 50.0f;
        boxSpawnChance = 0f;
        sawSpawnChance = 0f;
        mineSpawnChance = 0f;
        turretSpawnChance = 0f;

        Debug.Log("Spamming Fireballs for 50m!");
    }

    void ChanceForTypeSpam()
    {
        if (distance > unlockSawDistance)
        {
            // Hard-coded probability
            int fireballChance = Random.Range(0, 60);
            int turretChance = Random.Range(0, 80);
            int boxChance = Random.Range(0, 80);
            int sawChance = Random.Range(0, 100);

            if (fireballChance == 2)
            {
                FireballSpam();
                Messenger.Broadcast(GameEvent.SPAM_ALERT);
                return;
            }
            else if (turretChance == 2)
            {
                TurretSpam();
                Messenger.Broadcast(GameEvent.SPAM_ALERT);
                return;
            }
            else if (boxChance == 2)
            {
                BoxSpam();
                Messenger.Broadcast(GameEvent.SPAM_ALERT);
                return;
            }
            else if (sawChance == 2)
            {
                SawSpam();
                Messenger.Broadcast(GameEvent.SPAM_ALERT);
            }
        }
    }

    void SelectNextObstacle()
    {
        // Calculate the combined weight (sum of all spawn chances)
        float total = boxSpawnChance + sawSpawnChance + mineSpawnChance + turretSpawnChance + fireballSpawnChance;

        // If all chances are zero or negative, assign equal probabilities to each obstacle
        if (total <= 0f)
        {
            Debug.LogWarning("All spawn chances are 0! Using fallback.");
            boxSpawnChance = sawSpawnChance = mineSpawnChance = turretSpawnChance = fireballSpawnChance = 0.25f;
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
        else if (random < boxSpawnChance + sawSpawnChance + mineSpawnChance + turretSpawnChance)
        {
            randomObstacle = turretPrefab;
        }
        else
        {
            randomObstacle = fireballPrefab;
        }
    }

    void SetNextSpawnPoint()
    {
        // Check constraints before choosing a spawn point
        if (twoObstaclesNearby)
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMax * 0.5f, nextSpawnMax);
            twoObstaclesNearby = false;
        }
        else if (twoObstaclesFar)
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMin, nextSpawnMax * 0.5f);
            twoObstaclesFar = false;
        }
        else
        {
            nextSpawnPoint = distance + Random.Range(nextSpawnMin, nextSpawnMax);
        }

        // Apply constraints, if applicable
        if (nextSpawnPoint - distance < lowEndThreshold) twoObstaclesNearby = true;
        if (nextSpawnPoint - distance > highEndThreshold) twoObstaclesFar = true;

        // Unlock next obstacle if requirement met
        if (!spamming)
        {
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

            if (distance > unlockFireballDistance)
            {
                fireballSpawnChance = storedFireballChance;
            }
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
