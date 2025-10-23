using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LaserDotFlicker : MonoBehaviour
{
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        // Flicker the sprite until destroyed
        StartCoroutine(FlickerRoutine());
    }
    
    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
        }
    }
}
