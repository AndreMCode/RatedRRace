using UnityEngine;
using System.Collections;

public class TurretAction : MonoBehaviour
{
    // Obstacle secondary action: fire laser and exit the screen upwards
    // ----------------------------------------------------------------------

    [SerializeField] ObstacleTravel obstacleTravel;
    [SerializeField] SpriteRenderer turretSprite;
    [SerializeField] GameObject laser;
    private bool visualWarning = false;
    private bool shotFired = false;
    public float xPosForAction = 0f;
    public float postFireExitSpeed = 1f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip turretWarnSFX;
    public float turretWarnVol;
    [SerializeField] AudioClip turretChargeSFX;
    public float turretChargeVol;
    [SerializeField] AudioClip turretFireSFX;
    public float turretFireVol;

    void Start()
    {
        laser.SetActive(false);
        PlayTurretWarnSFX();
    }

    void Update()
    {
        // Stop traveling when at specified x-coordinate
        if (!visualWarning && transform.position.x < xPosForAction)
        {
            obstacleTravel.traveling = false;

            // Start a visual and audio warning before firing
            StartCoroutine(FlickerSprite(0.1f, 3));
            visualWarning = true;
            PlayTurretChargeSFX();
        }

        // Ascend after firing
        if (visualWarning && shotFired) transform.Translate(postFireExitSpeed * Time.deltaTime * Vector3.up);
    }

    // Flicker the sprite a number of times before firing
    private IEnumerator FlickerSprite(float interval, int count)
    {
        for (int i = 0; i < count; i++)
        {
            turretSprite.enabled = false;
            yield return new WaitForSeconds(interval);
            turretSprite.enabled = true;
            yield return new WaitForSeconds(interval);
        }

        StartCoroutine(FireLaser());
        PlayTurretFireSFX();
    }

    private IEnumerator FireLaser()
    {
        laser.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        laser.SetActive(false);

        shotFired = true;
        obstacleTravel.traveling = true;
    }

    void PlayTurretWarnSFX()
    {
        audioSource.PlayOneShot(turretWarnSFX, turretWarnVol);
    }

    void PlayTurretChargeSFX()
    {
        audioSource.PlayOneShot(turretChargeSFX, turretChargeVol);
    }

    void PlayTurretFireSFX()
    {
        audioSource.PlayOneShot(turretFireSFX, turretFireVol);
    }
}
