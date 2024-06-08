using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Services.Lobbies.Models;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentAfterDie = 50;
    public override void OnNetworkSpawn()
    {
        if(!IsServer){return;}

        // Check already have in default Scene
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach(TankPlayer player in players){
            HandlePlayerSpawned(player);
        }

        // Check if having new Tankplayer is added
        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }
    public override void OnNetworkDespawn()
    {
        if(!IsServer){return;}
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);

    }

    private void HandlePlayerDie(TankPlayer player)
    {
        int coinAfterLose = player.CoinWallet.TotalCoin.Value * (int)(keptCoinPercentAfterDie / 100);
        Destroy(player.gameObject);
        // losing coins
        StartCoroutine(RespawningPlayer(player.OwnerClientId, coinAfterLose));
    }

    IEnumerator RespawningPlayer(ulong ownerClientId, int coinAfterLose)
    {
        yield return new WaitForSecondsRealtime(1);

        TankPlayer newPlayer = 
            Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);

        newPlayer.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        newPlayer.CoinWallet.TotalCoin.Value += coinAfterLose;
    }
}
