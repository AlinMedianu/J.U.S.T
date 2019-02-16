using System.Linq;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("In how many areas (out of the ones available) " +
        "should the enemies be able to spawn?")]
    private int noAreasInUse = 3;
    [SerializeField]
    [Range(0, 1)]
    private float gizmoOppacity = 0.5f;
    private Vector3[] spawnPoints;
    private Rect[] spawnAreas;

    private void OnValidate()
    {
        Transform[] spawnLocations = transform.GetDirectChildren();
        spawnPoints = (from spawnLocation in spawnLocations
                       where !spawnLocation.HasChildren()
                       select spawnLocation.position).ToArray();
        spawnAreas = (from spawnLocation in spawnLocations
                      where spawnLocation.HasChildren()
                      select Rect.MinMaxRect(
                      spawnLocation.GetChildrenPositions()[0].x,
                      spawnLocation.GetChildrenPositions()[0].z,
                      spawnLocation.GetChildrenPositions()[1].x,
                      spawnLocation.GetChildrenPositions()[1].z)).ToArray();
    }

    private void Awake()
    {
        Transform[] spawnLocations = transform.GetDirectChildren();
        spawnPoints = (from spawnLocation in spawnLocations
                       where !spawnLocation.HasChildren()
                       select spawnLocation.position).ToArray();
        spawnAreas = (from spawnLocation in spawnLocations
                      where spawnLocation.HasChildren()
                      select Rect.MinMaxRect(
                      spawnLocation.GetChildrenPositions()[0].x,
                      spawnLocation.GetChildrenPositions()[0].z,
                      spawnLocation.GetChildrenPositions()[1].x,
                      spawnLocation.GetChildrenPositions()[1].z)).ToArray();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Scout-Model",
                spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)],
                Quaternion.identity).transform.Find("Agent").GetComponent<EnemyScout>().InitialiseWaypoints();
            int[] spawnAreaIDs = ChooseSpawnAreaIDs(noAreasInUse);
            foreach (int spawnAreaID in spawnAreaIDs)
            {
                int noEnemiesInArea = UnityEngine.Random.Range(3, 5);
                for (int i = 0; i < noEnemiesInArea; ++i)
                    PhotonNetwork.Instantiate("Guardian-Model", spawnAreas[spawnAreaID].RandomPosition(), Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, gizmoOppacity);
        foreach (Rect spawnArea in spawnAreas)
            Gizmos.DrawCube(spawnArea.center.XYToX0Z(), new Vector3(spawnArea.width, 1, spawnArea.height));
    }

    private int[] ChooseSpawnAreaIDs(int number)
    {
        int[] areaIDs = new int[number];
        for (int i = 0; i < number; ++i)
            areaIDs[i] = -1;
        for (int i = 0; i < number; ++i)
            do
            {
                areaIDs[i] = UnityEngine.Random.Range(0, spawnAreas.Length);
            } while (areaIDs.NumberOfOccurences(areaIDs[i]) > 1);
        return areaIDs;
    }
}
