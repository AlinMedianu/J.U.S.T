using UnityEngine;

[CreateAssetMenu]
public class Waypoints : ScriptableObject
{
    [SerializeField]
    private Vector3[] positions;

    public Vector3[] Positions
    {
        get
        {
            return positions;
        }
    }
}
