using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void OnEnable() {
        spawnPoints.Add(this);
    }

    private void OnDestroy() {
        spawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPoint(){
        if(spawnPoints.Count == 0){
            return Vector3.zero;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count -1)].transform.position;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
