using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Manage game settings: level, run speed, buffs, and abilities
    // ------------------------------------------------------------

    private int level = 1;
    public float baseRunSpeed = 5f;
    public float runSpeedScalar = 1f;

    public int defense = 0;

    private bool canSlide = false;
    private bool canDive = false;

    void Start()
    {
        // Retrieve selected bracket
        level = PlayerPrefs.GetInt("SelectedBracket", 1);

        // Allow ability based on level
        if (level > 1)
        {
            canSlide = true;
            runSpeedScalar = 1.2f; // 6 meters/sec
        }

        if (level > 2)
        {
            canDive = true;
            runSpeedScalar = 1.4f; // 7 meters/sec
        }

        // Relay game mode settings to listeners
        Messenger<int>.Broadcast(GameEvent.SET_LEVEL, level);
        Messenger<bool>.Broadcast(GameEvent.SET_ABILITY_SLIDE, canSlide);
        Messenger<bool>.Broadcast(GameEvent.SET_ABILITY_DIVE, canDive);
        Messenger<int>.Broadcast(GameEvent.SET_AUDIO_TRACK, level); // Level number is also track number for now
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SPEED, baseRunSpeed);
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SCALAR, runSpeedScalar);
        Messenger<int>.Broadcast(GameEvent.SET_HEALTH, defense);
    }
}
