using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private const string GameSceneName = "GameScene";

    public NetworkServer NetworkServer {get; private set;}
    private Allocation allocation;

    private const int maxAllocations = 20;

    private string joinCode;
    private string lobbyId;
    public async Task StartHostAsync(){
        try{
            allocation = await Relay.Instance.CreateAllocationAsync(maxAllocations);
        }catch(Exception e){
            Debug.LogError(e);
        }
        try{
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
              Debug.Log($"Join code: {joinCode}");
        }catch(Exception e){
            Debug.LogError(e);
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();


        RelayServerData relayServer = new RelayServerData(allocation, "dtls");

        transport.SetRelayServerData(relayServer);

        // Make a Lobby

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>(){
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }

            };
                
            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey);
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                $"{playerName}'s Lobby",
                maxAllocations, 
                lobbyOptions);

            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData{
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name.."),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartHost();

        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }

    private IEnumerator HeartBeatLobby(int timeWaiting)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(timeWaiting);
        while(true){

            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return delay;
        }
    }

    public  void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown(){
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));
        
        if(!string.IsNullOrEmpty(lobbyId)){
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }
        lobbyId = string.Empty;
        joinCode = string.Empty;

        NetworkServer.OnClientLeft -= HandleClientLeft;

        NetworkServer?.Dispose();
    }


    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}
