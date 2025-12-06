using UnityEngine;
using DG.Tweening;

public class BubbleShieldEffect : MonoBehaviour
{
    [SerializeField] GameObject bubbleParticle;
    [SerializeField] private float duration = 0.25f;

    private int lineThickness = Shader.PropertyToID("_LineThickness");

    private SpriteRenderer spriteRenderer;
    private Material bubbleMaterial;
    private float lerpAmount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bubbleMaterial = spriteRenderer.material;
    }

    public void Tween()
    {
        // duration = newDuration;
        lerpAmount = 0f;
        DOTween.To(GetEffectAmount, SetEffectAmount, 0.1f, duration).SetEase(Ease.InSine).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete);
    }

    public void BubbleParticle()
    {
        Instantiate(bubbleParticle, transform.position, bubbleParticle.transform.rotation);
    }

    private void OnLerpUpdate()
    {
        bubbleMaterial.SetFloat(lineThickness, GetEffectAmount());
    }

    private void OnLerpComplete()
    {
        bubbleMaterial.SetFloat(lineThickness, 0.01f);
        // Go in reverse
        // DOTween.To(GetEffectAmount, SetEffectAmount, 0.01f, duration).OnUpdate(OnLerpUpdate);
    }

    private float GetEffectAmount()
    {
        return lerpAmount;
    }

    private void SetEffectAmount(float value)
    {
        lerpAmount = value;
    }
}

