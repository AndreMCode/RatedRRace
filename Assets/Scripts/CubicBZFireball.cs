using UnityEngine;

public class CubicBZFireball : MonoBehaviour
{
    [SerializeField] GameObject rootObject;
    [SerializeField] BezierMovement bzMovement;
    public GameObject secondPoint;
    public GameObject thirdPoint;
    public float inflectionRange = 8.0f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireballWarnSFX;
    public float fireballWarnVol;

    void Start()
    {
        PlayFireballWarnSFX();
    }

    void Update()
    {
        if (bzMovement.endOfPath)
        {
            Destroy(rootObject);
        }
    }

    public void SetFireballCurvePoints(float second, float third)
    {
        second = Mathf.Clamp(second, -inflectionRange, inflectionRange);
        third = Mathf.Clamp(third, -inflectionRange, inflectionRange);
        
        // Adjust y-position for second and third points
        Vector3 secondPos = secondPoint.transform.localPosition;
        secondPos.y = second;
        secondPoint.transform.localPosition = secondPos;

        Vector3 thirdPos = thirdPoint.transform.localPosition;
        thirdPos.y = third;
        thirdPoint.transform.localPosition = thirdPos;
    }

    public void SetFireballSpeed(float value)
    {
        value = Mathf.Clamp(value, 0.1f, 0.5f);
        bzMovement.speed = value;
    }

    void PlayFireballWarnSFX()
    {
        audioSource.PlayOneShot(fireballWarnSFX, fireballWarnVol);
    }

    public void DestroyOnCollision()
    {
        Destroy(rootObject);
    }
}
