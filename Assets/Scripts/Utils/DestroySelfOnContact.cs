using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D other) {
        Destroy(gameObject);
        Debug.Log(other.gameObject.name);
    }
}
