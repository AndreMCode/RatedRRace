using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool running = false;

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
        StartCoroutine(InitialCountdown());
        // StartCoroutine(TestAdjust());
    }

    void Update()
    {
        if (running)
        {
            distanceTraveled = transform.position.x - distanceTracker.transform.position.x - 16.0f; // The offset distance between the spawner and the player

            if (gameLevel == 0)
            {
                // endless run algorithm
            }

            if (gameLevel == 1)
            {
                while (nextSpawnIndex < bracket1.sequence.Length &&
                       distanceTraveled >= bracket1.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket1.sequence[nextSpawnIndex];
                    Vector3 spawnPos = new(10.0f, evt.yPosition, 0);
                    GameObject nextObstacle = Instantiate(evt.obstaclePrefab, spawnPos, evt.obstaclePrefab.transform.rotation);
                    ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
                    obstacleSettings.baseSpeed = runSpeed;
                    obstacleSettings.scalar = runSpeedScalar;
                    nextSpawnIndex++;
                }
            }

            if (gameLevel == 2)
            {
                while (nextSpawnIndex < bracket2.sequence.Length &&
                       distanceTraveled >= bracket2.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket2.sequence[nextSpawnIndex];
                    Vector3 spawnPos = new(10.0f, evt.yPosition, 0);
                    GameObject nextObstacle = Instantiate(evt.obstaclePrefab, spawnPos, evt.obstaclePrefab.transform.rotation);
                    ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
                    obstacleSettings.baseSpeed = runSpeed;
                    obstacleSettings.scalar = runSpeedScalar;
                    nextSpawnIndex++;
                }
            }

            if (gameLevel == 3)
            {
                while (nextSpawnIndex < bracket3.sequence.Length &&
                       distanceTraveled >= bracket3.sequence[nextSpawnIndex].triggerDistance)
                {
                    var evt = bracket3.sequence[nextSpawnIndex];
                    Vector3 spawnPos = new(10.0f, evt.yPosition, 0);
                    GameObject nextObstacle = Instantiate(evt.obstaclePrefab, spawnPos, evt.obstaclePrefab.transform.rotation);
                    ObstacleTravel obstacleSettings = nextObstacle.GetComponent<ObstacleTravel>();
                    obstacleSettings.baseSpeed = runSpeed;
                    obstacleSettings.scalar = runSpeedScalar;
                    nextSpawnIndex++;
                }
            }
        }
    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_LEVEL, InitializeLevel);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SPEED, InitializeRunSpeed);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, InitializeRunScalar);
    }

    private void InitializeLevel(int number)
    {
        gameLevel = number;
    }

    private void InitializeRunSpeed(float value)
    {
        runSpeed = value;
    }

    private void InitializeRunScalar(float value)
    {
        runSpeedScalar = value;
    }

    private IEnumerator InitialCountdown()
    {
        yield return new WaitForSeconds(3f);

        // Current listeners: DistanceTracker, BGManager
        Messenger.Broadcast(GameEvent.START_RUN);
        running = true;
    }

    private IEnumerator TestAdjust()
    { // used to test mid-game speed adjustment
        yield return new WaitForSeconds(6f);

        Messenger<float>.Broadcast(GameEvent.ADJ_RUN_SPEED, 2f);
    }
}
