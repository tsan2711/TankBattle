using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] 
    private CinemachineVirtualCamera followCam;
    [SerializeField] 
    private SpriteRenderer minimapIconRenderer;
    [field: SerializeField] 

    public Health Health {get; private set; }
    [field: SerializeField]
    public CoinWallet CoinWallet {get; private set; }
    [Header("Settings")]

    [SerializeField] 
    private int camPriority = 15;
    [SerializeField] private Color minimapIconColor;

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if(IsServer){
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserData(OwnerClientId);
            PlayerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }
        if(IsOwner){
            followCam.Priority = camPriority;
            minimapIconRenderer.color = minimapIconColor;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer){
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
