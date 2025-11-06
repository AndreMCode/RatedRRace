using UnityEngine;

public class BezierMovement : MonoBehaviour
{
    public BezierCurve bezierCurve;
    public float speed = 0.5f; // Speed of movement along the curve
    public bool loop = false;
    public bool endOfPath = false;
    private float t = 0f; // Parameter to interpolate along the curve

    private void Update()
    {
        if (bezierCurve == null)
        {
            Debug.LogError("BezierCurve reference is not assigned.");
            return;
        }

        // Move along the curve
        t += Time.deltaTime * speed;
        t = Mathf.Clamp01(t); // Clamp t between 0 and 1

        // Update the character's position
        transform.position = bezierCurve.GetPointOnCurve(t);

        if (t >= 1f)
        {
            if (loop)
            {
                t = 0f;
            }
            else
            {
                endOfPath = true;
            }
        }
    }
}