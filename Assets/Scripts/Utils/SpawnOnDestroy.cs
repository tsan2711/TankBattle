using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] 
    private GameObject smokeEffect;

    private void OnDestroy() {
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
    }
}
