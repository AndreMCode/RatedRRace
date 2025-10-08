using UnityEngine;
using System.Collections;

public class TurretAction : MonoBehaviour
{
    // Obstacle secondary action, fire projectile and exit the screen upwards
    // WORK IN PROGRESS: will be changing this to global x-coordinate
    // ----------------------------------------------------------------------

    [SerializeField] ObstacleTravel obstacleTravel;
    private bool shotFired = false;
    public float timeToFire = 0f;
    public float postFireExitSpeed = 1f;

    void Update()
    {
        if (obstacleTravel.traveling && shotFired) transform.Translate(postFireExitSpeed * Time.deltaTime * Vector3.up);
    }

    public IEnumerator CountdownToFire(float value)
    {
        yield return new WaitForSeconds(value);

        shotFired = true;
    }
}
