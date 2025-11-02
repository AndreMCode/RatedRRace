using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Manages which tracks play and fades audio volume
    // ------------------------------------------------

    private static readonly WaitForSeconds _waitForSeconds3 = new(3f);
    public AudioSource[] bgmTracks;
    private AudioSource currentTrack;
    private AudioSource deathAudio;
    private bool isFading = false;
    private bool isPaused = false;

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.AddListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.AddListener(GameEvent.UI_AUDIO_ADJUST_VOL, ToggleVolume);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.RemoveListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.RemoveListener(GameEvent.UI_AUDIO_ADJUST_VOL, ToggleVolume);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    // Set the current music track, from GameManager
    void SetCurrentTrack(int trackNumber)
    {
        // Check track number against array bounds
        if (trackNumber < 0 || trackNumber >= bgmTracks.Length) return;

        currentTrack = bgmTracks[trackNumber - 1];
        Debug.Log("current track: " + currentTrack.name);
    }

    // Start the run track
    void PlayBGAudio()
    {
        currentTrack.Play();
    }

    void ToggleVolume()
    {
        if (!isPaused)
        {
            currentTrack.volume *= 0.5f;
            isPaused = true;
        }
        else
        {
            currentTrack.volume *= 2.0f;
            isPaused = false;
        }
    }

    // Play the death audio while quickly fading out the run audio
    void PlayerDied()
    {
        if (!isFading)
        {
            StartCoroutine(PlayDeathAudio());

            StartCoroutine(FadeOutTracks(0.5f));
        }
    }

    // Fade out the death audio after it plays for a moment
    private IEnumerator PlayDeathAudio()
    {
        deathAudio = bgmTracks[^1]; // bgmTracks.Length - 1
        deathAudio.Play();

        yield return _waitForSeconds3;

        currentTrack = deathAudio;
        StartCoroutine(FadeOutTracks(6f));
    }

    // Audio fade function
    private IEnumerator FadeOutTracks(float fadeDuration)
    { 
        isFading = true;

        float startVolume = currentTrack.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);

            currentTrack.volume = newVolume;

            yield return null;
        }

        currentTrack.Stop();

        isFading = false;
    }
}
