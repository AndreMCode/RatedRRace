using UnityEngine;

public class GameEvent : MonoBehaviour
{
    // Stores strings for easier messenger calls
    // -----------------------------------------
    
    public const string SET_LEVEL = "SET_LEVEL";
    public const string SET_HEALTH = "SET_HEALTH";
    public const string SET_ABILITY_SLIDE = "SET_ABILITY_SLIDE";
    public const string SET_ABILITY_DIVE = "SET_ABILITY_DIVE";
    public const string SET_AUDIO_TRACK = "SET_AUDIO_TRACK";
    public const string SET_RUN_SPEED = "SET_RUN_SPEED";
    public const string SET_RUN_SCALAR = "SET_RUN_SCALAR";
    public const string SET_GROUND_HEIGHT = "SET_GROUND_HEIGHT";
    public const string ADJ_RUN_SPEED = "ADJ_RUN_SPEED";
    public const string START_RUN = "START_RUN";
    public const string PLAYER_DIED = "PLAYER_DIED";
}
