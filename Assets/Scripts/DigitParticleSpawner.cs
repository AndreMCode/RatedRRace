using UnityEngine;

public class DigitParticleSpawner : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bonusSFX;
    public float bonusVol;
    public ParticleSystem particlePrefab;
    public Transform spawnPoint;
    private int currentValue = 0;

    public void OnBounceComboIncrement()
    {
        currentValue++;
        currentValue = Mathf.Clamp(currentValue, 0, 9);

        // Display a specific digit 1-9
        if (currentValue > 0)
        {
            if (particlePrefab == null) return;
            Vector3 spawnRotation = new(-90, 0, 0);
            ParticleSystem digitParticle = Instantiate(particlePrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.Euler(spawnRotation));

            var digit = digitParticle.textureSheetAnimation;
            digit.startFrame = currentValue / 10f;

            digitParticle.Play();

            float lifetime = digitParticle.main.duration + digitParticle.main.startLifetime.constantMax;
            Destroy(digitParticle.gameObject, lifetime + 0.1f);

            // Apply specific sound pitch for each digit
            float bonusPitch = 0.9f + (0.16f * currentValue);
            audioSource.pitch = bonusPitch;
            audioSource.PlayOneShot(bonusSFX, bonusVol);
        }
    }

    public void ResetCombo()
    {
        currentValue = 0;
    }
}