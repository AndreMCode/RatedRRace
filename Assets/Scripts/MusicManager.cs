using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Music array
    public AudioSource[] bgmTracks;

    // BGM reference
    private AudioSource currentTrack;
    private bool isFading = false;

    void Start()
    {

    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.AddListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.AddListener(GameEvent.PLAYER_DIED, FadeOutStop);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_AUDIO_TRACK, SetCurrentTrack);
        Messenger.RemoveListener(GameEvent.START_RUN, PlayBGAudio);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, FadeOutStop);
    }

    void SetCurrentTrack(int trackNumber)
    {
        // Check track number against array bounds
        if (trackNumber < 0 || trackNumber >= bgmTracks.Length) return;

        currentTrack = bgmTracks[trackNumber];
    }

    void PlayBGAudio()
    {
        currentTrack.Play();
    }

    public void OnPlayerWin()
    { // Long fade on win
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(3.5f));
        }
    }

    public void FadeOutStop()
    { // Short fade on fail/pause
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(0.5f));
        }
    }

    public void OnPlayerReset()
    { // Short fade on reset
        if (!isFading)
        {
            StartCoroutine(FadeOutTracks(1.0f));
        }
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
