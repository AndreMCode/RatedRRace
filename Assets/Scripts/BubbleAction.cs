using System.Collections;
using UnityEngine;

public class BubbleAction : MonoBehaviour
{
    [SerializeField] GameObject bubbleParticle;
    [SerializeField] SpriteRenderer bubbleSprite;
    public float alphaIdle = 0.07f;
    public float alphaSpin = 0.75f;
    public float alphaHit = 0.3f;

    [SerializeField] GameObject bubble;
    public float spinSpeedMin = 15;
    public float spinSpeedMax = 2160;
    public int rotationDirection = 1;
    public float spinDuration = 0.4f;
    public bool isBroken = false;

    private float elapsedTime = 0f;
    private float currentSpinSpeed;
    private bool isLerping = false;

    void Start()
    {
        currentSpinSpeed = spinSpeedMax;

        // Fallback
        if (bubble == null)
        {
            bubble = gameObject;
        }

        // Set proper sprite alpha level
        if (bubbleSprite != null)
        {
            float a = isLerping ? alphaSpin : alphaIdle;
            Color c = bubbleSprite.color;
            c.a = a;
            bubbleSprite.color = c;
        }
    }

    void Update()
    {
        // Update the spin speed
        UpdateSpinSpeed();

        // Apply rotation
        bubble.transform.Rotate(0f, 0f, currentSpinSpeed * rotationDirection * Time.deltaTime);
    }

    void UpdateSpinSpeed()
    {
        if (!isLerping) return;

        if (elapsedTime < spinDuration)
        {
            // Calculate lerp progress (0 to 1)
            float t = elapsedTime / spinDuration;

            // Lerp from max to min speed
            currentSpinSpeed = Mathf.Lerp(spinSpeedMax, spinSpeedMin, t);

            // Update elapsed time
            elapsedTime += Time.deltaTime;
        }
        else
        {
            // Lerping complete, lock to min speed
            currentSpinSpeed = spinSpeedMin;
            isLerping = false;

            // Update sprite alpha
            if (bubbleSprite != null)
            {
                Color c = bubbleSprite.color;
                c.a = alphaIdle;
                bubbleSprite.color = c;
            }
        }
    }

    public void RestartSpin()
    {
        elapsedTime = 0f;
        currentSpinSpeed = spinSpeedMax;
        isLerping = true;

        // Update sprite alpha
        if (bubbleSprite != null)
        {
            Color c = bubbleSprite.color;
            c.a = alphaSpin;
            bubbleSprite.color = c;
        }
    }

    public IEnumerator FlickerRoutine()
    {
        if (bubbleSprite != null)
        {
            Color c = bubbleSprite.color;
            c.a = alphaHit;
            bubbleSprite.color = c;
        }

        while (isBroken)
        {
            bubbleSprite.enabled = !bubbleSprite.enabled;
            yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
        }
        bubbleSprite.enabled = true;

        Instantiate(bubbleParticle, transform.position, bubbleParticle.transform.rotation);
    }
}
