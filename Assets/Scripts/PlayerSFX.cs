using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceRun;
    [SerializeField] AudioSource audioSourceSlide; // Because this sound can be canceled
    [SerializeField] AudioSource audioSourceStatic; // Unaffected by pitch changes

    // [SerializeField] AudioClip runSFX;
    public float runVol;
    [SerializeField] AudioClip jumpSFX;
    public float jumpVol;
    [SerializeField] AudioClip slideSFX;
    public float slideVol;
    [SerializeField] AudioClip shieldHitSFX;
    public float shieldHitVol;
    [SerializeField] AudioClip shieldPopSFX;
    public float shieldPopVol;
    [SerializeField] AudioClip boxBreakSFX;
    public float boxBreakVol;
    [SerializeField] AudioClip sawSliceSFX;
    public float sawSliceVol;
    [SerializeField] AudioClip mineExploSFX;
    public float mineExploVol;

    void VaryPitch()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
    }

    public void PlayRunSFX()
    {
        audioSourceRun.volume = runVol;
        audioSourceRun.Play();
    }

    public void StopRunSFX()
    {
        audioSourceRun.Stop();
    }

    public void PlayJumpSFX()
    {
        VaryPitch();
        audioSource.PlayOneShot(jumpSFX, jumpVol);
    }

    public void StartSlideSFX()
    {
        audioSourceSlide.PlayOneShot(slideSFX, slideVol);
    }

    public void StopSlideSFX()
    {
        audioSourceSlide.Stop();
    }

    public void PlayShieldHitSFX()
    {
        audioSourceStatic.PlayOneShot(shieldHitSFX, shieldHitVol);
    }

    public void PlayShieldPopSFX()
    {
        audioSourceStatic.PlayOneShot(shieldPopSFX, shieldPopVol);
    }

    public void PlayBoxBreakSFX()
    {
        VaryPitch();
        audioSource.PlayOneShot(boxBreakSFX, boxBreakVol);
    }

    public void PlaySawSliceSFX()
    {
        VaryPitch();
        audioSource.PlayOneShot(sawSliceSFX, sawSliceVol);
    }
    
    public void PlayMineExploSFX()
    {
        VaryPitch();
        audioSource.PlayOneShot(mineExploSFX, mineExploVol);
    }
}
