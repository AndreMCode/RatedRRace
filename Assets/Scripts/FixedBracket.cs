using UnityEngine;

[CreateAssetMenu(fileName = "FixedBracket", menuName = "Scriptable Objects/FixedBracket")]
public class FixedBracket : ScriptableObject
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

        // Used to adjust where the Turret fires [-8, 8]
        public float turretXPos = 0f;

        // Used to adjust inflection points for Fireball [-4, 4]
        public float fireballY2;
        public float fireballY3;

        // Set the speed of the fireball
        public float fireballSpeed;
    }
    public SpawnEvent[] sequence;
}
