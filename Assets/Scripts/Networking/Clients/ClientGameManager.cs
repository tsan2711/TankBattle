using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string MenuSceneName = "MainMenu";
    private JoinAllocation allocation;

    private NetworkClient networkClient;

    private string userName;


    public async Task<bool> InitAsync(){
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuth();
        if(authState == AuthState.Authenticated){
            return true;
        }
        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServer = new RelayServerData(allocation, "dtls");

        transport.SetRelayServerData(relayServer);

        // Send UserData
        UserData userData = new UserData{
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name.."),
            userAuthId = AuthenticationService.Instance.PlayerId
        
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }
    public void Disconnect()
    {
        networkClient.Disconnect();
    }
    public void Dispose()
    {
        networkClient.Dispose();
    }


}
