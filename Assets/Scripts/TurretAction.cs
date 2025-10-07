using UnityEngine;
using System.Collections;

public class TurretAction : MonoBehaviour
{
    [SerializeField] ObstacleTravel obstacleTravel;
    private bool shotFired = false;
    public float timeToFire = 0f;
    public float postFireExitSpeed = 1f;

    void Start()
    {

    }

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
