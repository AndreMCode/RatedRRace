using UnityEngine;

[CreateAssetMenu(fileName = "Bracket2", menuName = "Scriptable Objects/Bracket2")]
public class Bracket2 : ScriptableObject
{
    [System.Serializable]
    public class SpawnEvent
    {
        // Scriptable object used to store obstacle parameters to be used by SpawnManager
        // ------------------------------------------------------------------------------
        
        // The distance traveled by the player that triggers the obstacle to spawn
        public float triggerDistance;

        // The obstacle prefab
        public GameObject obstaclePrefab;

        // The obstacle's height from the ground
        public float yPosition;

        // Used to simulate an obstacle moving faster or slower than the run speed. The Saw or Turret
        public float speedOffsetScalar = 1f;

        // Used to increase or decrease the scale of the Saw
        public float sawScale = 1f;

        // Used to adjust the length of the Mine laser
        public float laserLength = 1f;

        // Used to adjust WHEN the Turret fires, WIP: will be changing this to WHERE
        public float turretTimer = 1f;
    }

    public SpawnEvent[] sequence;
}
