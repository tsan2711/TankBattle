using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton instance;
    public static ServerSingleton Instance{
        get
        {
            if(instance != null){return instance;}
            instance = FindObjectOfType<ServerSingleton>();
            if(instance == null){
                Debug.Log("No ServerSingleton in this scene");
            }
            return instance;
        }
    }

    public ServerGameManager GameManager {get; private set;}

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServer(){
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager();
    }

    private void OnDestroy() {
        GameManager?.Dispose();
    }

}
