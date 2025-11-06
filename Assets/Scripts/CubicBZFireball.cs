using UnityEngine;

public class CubicBZFireball : MonoBehaviour
{
    [SerializeField] GameObject rootObject;
    [SerializeField] BezierMovement bzMovement;
    public GameObject secondPoint;
    public GameObject thirdPoint;

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
        second = Mathf.Clamp(second, -4f, 4f);
        third = Mathf.Clamp(third, -4f, 4f);
        
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
