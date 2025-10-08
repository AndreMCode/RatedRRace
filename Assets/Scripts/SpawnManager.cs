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
    public Bracket1 bracket1;
    public Bracket2 bracket2;
    public Bracket3 bracket3;
    private int nextSpawnIndex = 0;
    public GameObject distanceTracker;
    public float distanceTraveled = 0f;

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
            if (gameLevel == 1)
            {
                while (nextSpawnIndex < bracket1.sequence.Length &&
                       distanceTraveled >= bracket1.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket1.sequence[nextSpawnIndex];
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
                            StartCoroutine(turretAction.CountdownToFire(evt.turretTimer));
                        }
                    }

                    nextSpawnIndex++;
                }
            }

            if (gameLevel == 2)
            {
                while (nextSpawnIndex < bracket2.sequence.Length &&
                       distanceTraveled >= bracket2.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket2.sequence[nextSpawnIndex];
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
                            StartCoroutine(turretAction.CountdownToFire(evt.turretTimer));
                        }
                    }

                    nextSpawnIndex++;
                }
            }

            if (gameLevel == 3)
            {
                while (nextSpawnIndex < bracket3.sequence.Length &&
                       distanceTraveled >= bracket3.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket3.sequence[nextSpawnIndex];
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
                            StartCoroutine(turretAction.CountdownToFire(evt.turretTimer));
                        }
                    }

                    nextSpawnIndex++;
                }
            }

            if (gameLevel == 4)
            {
                // endless run algorithm
            }
        }
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.START_RUN, ToggleRun);
        Messenger<int>.AddListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.AddListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger.AddListener(GameEvent.PLAYER_DIED, ToggleRun);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.START_RUN, ToggleRun);
        Messenger<int>.RemoveListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
        Messenger<float>.RemoveListener(GameEvent.SET_GROUND_HEIGHT, SetGroundHeight);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, ToggleRun);
    }

    // Toggle running, -- from UIBracketMode
    private void ToggleRun()
    {
        if (running) running = false;
        else running = true;
    }

    // Set game level, -- from GameManager
    private void InitializeLevel(int number)
    {
        gameLevel = number;
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

    // Used to test mid-game speed adjustment
    // (we currently SHOULD NOT spawn obstacles during a speed transition)
    private IEnumerator TestAdjust()
    {
        yield return new WaitForSeconds(9f);

        Messenger<float>.Broadcast(GameEvent.ADJ_RUN_SPEED, 2f);
    }
}
