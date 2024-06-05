using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : MonoBehaviour, IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    public NetworkServer(NetworkManager networkManager){
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetWorkReady;
    }

    private void OnNetWorkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId)){
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth.Add(request.ClientNetworkId, userData.userAuthId);
        authIdToUserData.Add(userData.userAuthId, userData);



        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPoint();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    public void Dispose()
    {
        if(!networkManager){ return; }

        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnServerStarted -= OnNetWorkReady;

        if(networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }

    public UserData GetUserData(ulong clientId){
        if(clientIdToAuth.TryGetValue(clientId, out string clientAuth)){
            if(authIdToUserData.TryGetValue(clientAuth, out UserData userData)){
                return userData;
            }
            return null;
        }
        return null;
    }
}
