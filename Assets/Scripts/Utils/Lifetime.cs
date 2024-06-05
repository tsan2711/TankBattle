using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 2f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }


}
