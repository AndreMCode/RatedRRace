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

    public const string PLAYER_PAUSE_AUDIO = "PLAYER_PAUSE_AUDIO";
    public const string PLAYER_TOGGLE_CONTROLS = "PLAYER_TOGGLE_CONTROLS";

    public const string UI_DECREMENT_BUBBLE = "UI_DECREMENT_BUBBLE";
    public const string UI_SET_RUN_RATE = "UI_SET_RUN_RATE";
    public const string UI_UPDATE_EARNINGS = "UI_UPDATE_EARNINGS";
    public const string UI_UPDATE_BONUS = "UI_UPDATE_BONUS";
    public const string UI_AUDIO_ADJUST_VOL = "UI_AUDIO_ADJUST_VOL";
}
