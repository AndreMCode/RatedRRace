using UnityEngine;

public class GameSFX : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public float spamAlertVol;
    [SerializeField] AudioClip spamAlertSFX;

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.SPAM_ALERT, PlaySpamAlertSFX);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.SPAM_ALERT, PlaySpamAlertSFX);
    }

    public void PlaySpamAlertSFX()
    {
        audioSource.PlayOneShot(spamAlertSFX, spamAlertVol);
    }
}
