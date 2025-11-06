using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform[] controlPoints; // Array of control points
    public int resolution = 10; // Number of points to draw the curve

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawBezierCurve();
    }

    private void DrawBezierCurve()
    {
        if (controlPoints == null || controlPoints.Length < 2)
        {
            Debug.LogError("Not enough control points assigned.");
            return;
        }
        Vector3 previousPoint = GetPointOnCurve(0);
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = GetPointOnCurve(t);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    public Vector3 GetPointOnCurve(float t)
    {
        if (controlPoints == null || controlPoints.Length < 2)
        {
            Debug.LogError("Not enough control points assigned.");
            return Vector3.zero;
        }
        return CalculateBezierPoint(t, controlPoints);
    }

    private Vector3 CalculateBezierPoint(float t, Transform[] points)
    {
        int n = points.Length - 1; // Degree of the Bezier curve
        Vector3 point = Vector3.zero;
        for (int i = 0; i <= n; i++)
        {
            float binomialCoefficient = BinomialCoefficient(n, i);
            float powerT = Mathf.Pow(t, i);
            float powerOneMinusT = Mathf.Pow(1 - t, n - i);
            point += binomialCoefficient * powerT * powerOneMinusT * points[i].position;
        }
        return point;
    }

    private float BinomialCoefficient(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        float res = 1;
        for (int i = 0; i < k; i++)
        {
            res *= (n - i);
            res /= (i + 1);
        }
        return res;
    }
}