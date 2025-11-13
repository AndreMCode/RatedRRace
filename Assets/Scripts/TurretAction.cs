using UnityEngine;
using System.Collections;

public class TurretAction : MonoBehaviour
{
    // Obstacle secondary action: fire laser and exit the screen upwards
    // ----------------------------------------------------------------------

    [SerializeField] ObstacleTravel obstacleTravel;
    [SerializeField] SpriteRenderer turretSprite;
    [SerializeField] GameObject laser;

    // Origin and angle to detect the ground
    [SerializeField] GameObject sight;

    // Visual laser dot on ground
    [SerializeField] GameObject laserDot;
    [SerializeField] LayerMask groundLayer;
    private bool visualWarning = false;
    private bool shotFired = false;
    private bool secondShot = false;
    public float xPosForAction = 0f;
    public float postFireExitSpeed = 1f;
    private GameObject activeDot;
    private float raycastDistance = 50f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceDrone;
    [SerializeField] AudioClip turretWarnSFX;
    public float turretWarnVol;
    [SerializeField] AudioClip turretChargeSFX;
    public float turretChargeVol;
    [SerializeField] AudioClip turretFireSFX;
    public float turretFireVol;

    [SerializeField] GameObject explosionParticle3;
    private Vector2 hitPoint;

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

            // Cast ray and place laser dot on ground
            PlaceDotAtRaycastHit();

            // Start a visual and audio warning before firing
            StartCoroutine(FlickerSprite(0.1f, 3));
            visualWarning = true;
            PlayTurretChargeSFX();
        }

        // Ascend after firing
        if (visualWarning && shotFired) transform.Translate(postFireExitSpeed * Time.deltaTime * Vector3.up);
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.OBSTACLE_TOGGLE_AUDIO, PauseToggleAudio);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.OBSTACLE_TOGGLE_AUDIO, PauseToggleAudio);
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
        Instantiate(explosionParticle3, hitPoint, explosionParticle3.transform.rotation);

        // Remove the laser dot after firing
        if (activeDot != null)
        {
            Destroy(activeDot);
            activeDot = null;
        }

        yield return new WaitForSeconds(0.25f);
        laser.SetActive(false);

        if (!secondShot)
        {
            // Hard-coded probability
            int oneInTwelve = Random.Range(0, 12);
            if (oneInTwelve == 1)
            {
                yield return new WaitForSeconds(0.5f);
                visualWarning = false;
                secondShot = true;
            }
            else
            {
                shotFired = true;
                obstacleTravel.traveling = true;
            }
        }
        else
        {
            shotFired = true;
            obstacleTravel.traveling = true;
        }
    }

    private void PlaceDotAtRaycastHit()
    {
        if (sight == null || laserDot == null) return;

        // Cast ray from sight object's position in its local left direction
        RaycastHit2D hit = Physics2D.Raycast(
            sight.transform.position,
            sight.transform.TransformDirection(Vector2.left),
            raycastDistance,
            groundLayer
        );

        // If we hit ground, spawn dot at hit point
        if (hit.collider != null)
        {
            activeDot = Instantiate(laserDot, hit.point, Quaternion.identity);
            // Store hit point for effect reference
            hitPoint = hit.point;
        }

#if UNITY_EDITOR
        // Visualize the raycast in scene view
        Debug.DrawRay(
            sight.transform.position,
            sight.transform.TransformDirection(Vector2.left) * raycastDistance,
            Color.red,
            1f
        );
#endif
    }

    void PauseToggleAudio()
    {
        if (audioSourceDrone.isPlaying) audioSourceDrone.Pause();
        else audioSourceDrone.UnPause();
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
