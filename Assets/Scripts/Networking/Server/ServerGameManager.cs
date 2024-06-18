using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    private const string GameSceneName = "GameScene";
    private string ip;
    private int port;
    private int queryPort;
    private NetworkServer networkServer;
    private MultiplayAllocationService multiplayAllocationService;
    public ServerGameManager(string ip, int port, int queryPort, NetworkManager manager){
        this.ip = ip;
        this.port = port;
        this.queryPort = queryPort;
        this.networkServer = new NetworkServer(manager);
        this.multiplayAllocationService = new MultiplayAllocationService();
    }
    public async void StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();
        if(!networkServer.OpenConnection(ip,port)){
            Debug.LogError("NetworkServer did not start as expected. ");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
    public void Dispose()
    {
        networkServer?.Dispose();
        multiplayAllocationService?.Dispose();
    }


}
