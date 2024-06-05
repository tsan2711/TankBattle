using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class JoinSever : MonoBehaviour
{

    public void Join(){
        NetworkManager.Singleton.StartClient();
    }
}
