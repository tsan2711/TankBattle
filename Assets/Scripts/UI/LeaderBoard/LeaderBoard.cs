using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    [SerializeField] 
    private Transform leaderBoardEntityHolder;
    [SerializeField] 
    private LeaderBoardEntityDisplay leaderBoardEntityPrefab;

    NetworkList<LeaderEntityState> leaderBoardEntities;

    private void Awake() {
        leaderBoardEntities = new NetworkList<LeaderEntityState>();
        
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer){
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

            foreach(TankPlayer player in players){
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
        
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer){return;}
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player){
        leaderBoardEntities.Add(new LeaderEntityState{
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });
    }

    private void HandlePlayerDespawned(TankPlayer player){
        if(leaderBoardEntities == null){return;}

        foreach(LeaderEntityState entity in leaderBoardEntities){
            if(entity.ClientId != player.OwnerClientId){continue;}

            leaderBoardEntities.Remove(entity);
            break;
        }
    }

}
