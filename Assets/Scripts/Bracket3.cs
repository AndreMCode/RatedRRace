using UnityEngine;

[CreateAssetMenu(fileName = "Bracket3", menuName = "Scriptable Objects/Bracket3")]
public class Bracket3 : ScriptableObject
{
    [System.Serializable]
    public class SpawnEvent
    {
        public float triggerDistance;
        public GameObject obstaclePrefab;
        public float yPosition;
        public float speedOffsetScalar = 1f;
        public float sawScale = 1f;
        public float laserLength = 1f;
        public float turretTimer = 1f;
    }

    public SpawnEvent[] sequence;
}
