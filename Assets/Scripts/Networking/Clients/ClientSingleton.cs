using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }
            instance = FindObjectOfType<ClientSingleton>();
            if (instance == null)
            {
                Debug.Log("No ClientSingleton in this scene");
                return null;
            }
            return instance;
        }
    }
    public ClientGameManager GameManager{get; private set;}

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient(){
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }

    private void OnDestroy() {
        GameManager.Dispose();
    }

}
