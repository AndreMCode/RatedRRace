using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Manage game settings: level, "run" speed,
    public int level = 1;
    public float runSpeed = 5f;
    public float runSpeedScalar = 1f;

    // PlayerPrefs for more permanent settings. This script will retrieve and update levels cleared, endless access, etc.

    void Start()
    {
        Messenger<int>.Broadcast(GameEvent.SET_LEVEL, level);
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SPEED, runSpeed);
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SCALAR, runSpeedScalar);
    }
}
