using UnityEngine;

public class DigitParticleSpawner : MonoBehaviour
{
    public ParticleSystem particlePrefab;
    public Transform spawnPoint;
    private int currentValue = 0;

    public void OnBounceComboIncrement()
    {
        currentValue++;
        currentValue = Mathf.Clamp(currentValue, 0, 9);

        if (currentValue > 1)
        {
            if (particlePrefab == null) return;
            Vector3 spawnRotation = new(-90, 0, 0);
            ParticleSystem digitParticle = Instantiate(particlePrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.Euler(spawnRotation));

            var digit = digitParticle.textureSheetAnimation;
            digit.startFrame = currentValue / 10f;

            digitParticle.Play();

            float lifetime = digitParticle.main.duration + digitParticle.main.startLifetime.constantMax;
            Destroy(digitParticle.gameObject, lifetime + 0.1f);
        }
    }

    public void ResetCombo()
    {
        currentValue = 0;
    }
}