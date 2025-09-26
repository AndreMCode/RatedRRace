using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Manage game settings: level, "run" speed, "health" modifiers,
    private int level = 1;
    public float baseRunSpeed = 5f;
    public float runSpeedScalar = 1f;

    public int defense = 0;

    // PlayerPrefs for more permanent settings. This script will retrieve SelectedBracket and other game variables

    void Start()
    {
        // Retrieve selected bracket
        level = PlayerPrefs.GetInt("SelectedBracket", 1);

        Messenger<int>.Broadcast(GameEvent.SET_LEVEL, level);
        Messenger<int>.Broadcast(GameEvent.SET_AUDIO_TRACK, level); // Level number is also track number for now
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SPEED, baseRunSpeed);
        Messenger<float>.Broadcast(GameEvent.SET_RUN_SCALAR, runSpeedScalar);

        // Apply any defensive hit points
        Messenger<int>.Broadcast(GameEvent.SET_HEALTH, defense);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Temporary for prototype
            ReloadScene();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // Temporary for prototype
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
