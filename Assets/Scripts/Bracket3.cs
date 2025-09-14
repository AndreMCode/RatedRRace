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
    }

    public SpawnEvent[] sequence;
}
