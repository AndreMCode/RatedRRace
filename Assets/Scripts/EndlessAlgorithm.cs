using UnityEngine;

public class EndlessAlgorithm : MonoBehaviour
{
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject sawPrefab;
    [SerializeField] GameObject minePrefab;
    [SerializeField] GameObject turretPrefab;
    private GameObject[] obstaclePrefabs;
    private GameObject randomObstacle;
    public float groundLevel = 0f;
    public bool endless = false;
    public float distance = 0f;
    public float nextSpawnPoint = 10.0f;

    public float nextSpawnMin = 0.5f;
    public float nextSpawnMax = 10.0f;

    public float boxHeightMin = 0.5f;
    public float boxHeightMax = 5.0f;

    public float sawHeightMin = 0f;
    public float sawHeightMax = 5.0f;
    public float sawScaleMin = 0.5f;
    public float sawScaleMax = 3.0f;
    public float sawSpeedOffsetMin = 0.3f;
    public float sawSpeedOffsetMax = 2.0f;

    public float mineLengthMin = 0.5f;
    public float mineLengthMax = 3.0f;

    public float turretHeightMin = 3.0f;
    public float turretHeightMax = 8.0f;
    public float turretMinX = -5.0f;
    public float turretMaxX = 2.0f;

    void Start()
    {
        // Add each prefab to an array to set the next obstacle randomly
        obstaclePrefabs = new GameObject[] { boxPrefab, sawPrefab, minePrefab, turretPrefab };
        NextObstacle();
    }

    void Update()
    {
        if (endless)
        {
            if (distance > nextSpawnPoint)
            {
                SpawnObstacle();
                NextSpawnPoint();
                NextObstacle();
            }
        }
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);

    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
    }

    void SpawnObstacle()
    {
        if (randomObstacle.name == "Box")
        {
            Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(boxHeightMin, boxHeightMax), 0);
            GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

            // Grab obstacle travel component and set values
            ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
            obstacleSettings.traveling = true;
            obstacleSettings.baseSpeed = 5.0f;
            obstacleSettings.scalar = 1.4f;

            return;
        }

        if (randomObstacle.name == "Saw")
        {
            Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(sawHeightMin, sawHeightMax), 0);
            GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

            // Grab obstacle travel component and set values
            ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
            obstacleSettings.traveling = true;
            obstacleSettings.baseSpeed = 5.0f;
            obstacleSettings.scalar = 1.4f;

            float newSawScale = Random.Range(sawScaleMin, sawScaleMax);
            Vector3 sawScale = nextObstacle.transform.localScale;
            sawScale.x *= newSawScale;
            sawScale.y *= newSawScale;
            nextObstacle.transform.localScale = sawScale;

            obstacleSettings.offsetScalar = Random.Range(sawSpeedOffsetMin, sawSpeedOffsetMax);

            return;
        }

        if (randomObstacle.name == "Mine")
        {
            Vector3 spawnPos = new(10.0f, groundLevel, 0);
            GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

            // Grab obstacle travel component and set values
            ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
            obstacleSettings.traveling = true;
            obstacleSettings.baseSpeed = 5.0f;
            obstacleSettings.scalar = 1.4f;

            Transform laserLength = nextObstacle.transform.Find("Scale");
            if (laserLength != null)
            {
                Vector3 s = laserLength.localScale;
                s.y = Random.Range(mineLengthMin, mineLengthMax);
                laserLength.localScale = s;
            }
        }

        if (randomObstacle.name == "Turret")
        {
            Vector3 spawnPos = new(10.0f, groundLevel + Random.Range(turretHeightMin, turretHeightMax), 0);
            GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

            // Grab obstacle travel component and set values
            ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
            obstacleSettings.traveling = true;
            obstacleSettings.baseSpeed = 5.0f;
            obstacleSettings.scalar = 1.4f;

            TurretAction turretAction = nextObstacle.GetComponent<TurretAction>();
            if (turretAction != null)
            {
                turretAction.xPosForAction = Random.Range(turretMinX, turretMaxX);
            }
        }

        //Vector3 spawnPos = new(10.0f, groundLevel, 0);
        //GameObject nextObstacle = Instantiate(randomObstacle, spawnPos, randomObstacle.transform.rotation);

        // Grab obstacle travel component and set values
        // ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
        // obstacleSettings.traveling = true;
        // obstacleSettings.baseSpeed = 5.0f;
        // obstacleSettings.scalar = 1.4f;

        // Apply parameters specific to Box
        // if (nextObstacle.name == "Box")
        // {
        //     nextObstacle.transform.position = new(10.0f, groundLevel + Random.Range(boxHeightMin, boxHeightMax), 0);

        //     return;
        // }

        // Apply parameters specific to Saw
        // if (nextObstacle.name == "Saw")
        // {
        //     nextObstacle.transform.position = new(10.0f, groundLevel + Random.Range(sawHeightMin, sawHeightMax), 0);

        //     float newSawScale = Random.Range(sawScaleMin, sawScaleMax);
        //     Vector3 sawScale = nextObstacle.transform.localScale;
        //     sawScale.x *= newSawScale;
        //     sawScale.y *= newSawScale;
        //     nextObstacle.transform.localScale = sawScale;

        //     obstacleSettings.offsetScalar = Random.Range(sawSpeedOffsetMin, sawSpeedOffsetMax);

        //     return;
        // }

        // Apply parameters specific to Mine
        // if (nextObstacle.name == "Mine")
        // {
        //     Transform laserLength = nextObstacle.transform.Find("Scale");
        //     if (laserLength != null)
        //     {
        //         Vector3 s = laserLength.localScale;
        //         s.y = Random.Range(mineLengthMin, mineLengthMax);
        //         laserLength.localScale = s;
        //     }
        // }

        // Apply parameters specific to Turret
        // if (nextObstacle.name == "Turret")
        // {
        //     nextObstacle.transform.position = new(10.0f, groundLevel + Random.Range(turretHeightMin, turretHeightMax), 0);

        //     TurretAction turretAction = nextObstacle.GetComponent<TurretAction>();
        //     if (turretAction != null)
        //     {
        //         turretAction.xPosForAction = Random.Range(turretMinX, turretMaxX);
        //     }
        // }
    }

    void NextObstacle()
    {
        int index = Random.Range(0, obstaclePrefabs.Length);
        randomObstacle = obstaclePrefabs[index];
    }

    void NextSpawnPoint()
    {
        nextSpawnPoint = distance + Random.Range(nextSpawnMin, nextSpawnMax);
    }

    public void InitializeEndless()
    {
        endless = true;
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
