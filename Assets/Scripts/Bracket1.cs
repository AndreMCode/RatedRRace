using UnityEngine;

[CreateAssetMenu(fileName = "Bracket1", menuName = "Scriptable Objects/Bracket1")]
public class Bracket1 : ScriptableObject
{
    [System.Serializable]
    public class SpawnEvent
    {
        public float triggerDistance;
        public GameObject obstaclePrefab;
        public float yPosition;
        public float speedOffsetScalar = 1;
    }

    public SpawnEvent[] sequence;
}
