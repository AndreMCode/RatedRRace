using UnityEngine;

[CreateAssetMenu(fileName = "Bracket2", menuName = "Scriptable Objects/Bracket2")]
public class Bracket2 : ScriptableObject
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
