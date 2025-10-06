using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);

    // Music array
    public AudioSource[] bgmTracks;

    // BGM reference
    private AudioSource currentTrack;
    private AudioSource deathAudio;
    private bool isFading = false;

    void Start()
    {

    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.AddListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.RemoveListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
    }

    void SetCurrentTrack(int trackNumber)
    {
        // Check track number against array bounds
        if (trackNumber < 0 || trackNumber >= bgmTracks.Length) return;

        currentTrack = bgmTracks[trackNumber - 1];
        Debug.Log("current track: " + currentTrack.name);
    }

    void PlayBGAudio()
    {
        currentTrack.Play();
    }

    void PlayerDied()
    {
        if (!isFading)
        {
            StartCoroutine(PlayDeathAudio());

            StartCoroutine(FadeOutTracks(0.5f));
        }
    }

    void OnPlayerWin()
    { // Long fade on win
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(3.5f));
        }
    }

    void OnPlayerReset()
    { // Short fade on reset
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(1.0f));
        }
    }

    private IEnumerator PlayDeathAudio()
    {
        deathAudio = bgmTracks[^1]; // bgmTracks.Length - 1
        deathAudio.Play();

        yield return _waitForSeconds1;

        currentTrack = deathAudio;
        StartCoroutine(FadeOutTracks(8f));
    }

    private IEnumerator FadeOutTracks(float fadeDuration)
    { // Fade function
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
