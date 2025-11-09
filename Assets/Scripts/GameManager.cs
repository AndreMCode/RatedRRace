using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Manage game settings: level, run speed, buffs, and abilities
    // ------------------------------------------------------------

    [SerializeField] GameObject bracketBG;
    [SerializeField] GameObject endlessBG;

    private int level = 1;
    public float baseRunSpeed = 5f;
    public float runSpeedScalar = 1f;

    public int defense = 0;

    private bool canSlide = false;
    private bool canDive = false;

    void Start()
    {
        bracketBG.SetActive(true);
        endlessBG.SetActive(false);

        // Retrieve selected bracket
        level = PlayerPrefs.GetInt("SelectedBracket", 1);

        defense += PlayerPrefs.GetInt("BubbleShieldCountBronze", 0);

        // Allow ability based on level
        if (level > 1)
        {
            canSlide = true;
            runSpeedScalar = 1.2f; // was 1.2f
            defense = 0;
            defense += PlayerPrefs.GetInt("BubbleShieldCountSilver", 0);
        }

        if (level > 2)
        {
            canDive = true;
            runSpeedScalar = 1.4f; // was 1.4f
            defense = 0;
            defense += PlayerPrefs.GetInt("BubbleShieldCountGold", 0);
        }

        if (level > 3)
        {
            bracketBG.SetActive(false);
            endlessBG.SetActive(true);
            runSpeedScalar = 1.3f;
            defense = 0;
            defense += PlayerPrefs.GetInt("BubbleShieldCount", 0);
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
