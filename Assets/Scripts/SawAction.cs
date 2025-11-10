using UnityEngine;

public class SawAction : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.OBSTACLE_TOGGLE_AUDIO, PauseToggleAudio);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.OBSTACLE_TOGGLE_AUDIO, PauseToggleAudio);
    }

    void PauseToggleAudio()
    {
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.UnPause();
    }
}
