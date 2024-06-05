using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    [SerializeField] 
    private Transform leaderBoardEntityHolder;
    [SerializeField] 
    private LeaderBoardEntityDisplay leaderBoardEntityPrefab;

    private NetworkList<LeaderEntityState> leaderBoardEntities;
    private List<LeaderBoardEntityDisplay> entityDisplays = new List<LeaderBoardEntityDisplay>();

    private void Awake() {
        leaderBoardEntities = new NetworkList<LeaderEntityState>();
        
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient){
            leaderBoardEntities.OnListChanged += HandleLeaderBoardEntitiesChanged;
            foreach(LeaderEntityState entity in leaderBoardEntities){
                HandleLeaderBoardEntitiesChanged(new NetworkListEvent<LeaderEntityState>{
                    Type = NetworkListEvent<LeaderEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }
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
        if(IsClient){
            leaderBoardEntities.OnListChanged -= HandleLeaderBoardEntitiesChanged;
        }
        if(IsServer){
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
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


    private void HandleLeaderBoardEntitiesChanged(NetworkListEvent<LeaderEntityState> changeEvent)
    {
        switch(changeEvent.Type){
            case NetworkListEvent<LeaderEntityState>.EventType.Add:
                if(!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId)){
                    LeaderBoardEntityDisplay entityDisplay = Instantiate(leaderBoardEntityPrefab, leaderBoardEntityHolder);
                    entityDisplay.Initialize(
                        changeEvent.Value.ClientId, 
                        changeEvent.Value.PlayerName, 
                        changeEvent.Value.Coins);

                    entityDisplays.Add(entityDisplay);

                }

                break;
            case NetworkListEvent<LeaderEntityState>.EventType.Remove:
                LeaderBoardEntityDisplay displayToRemove = 
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if(displayToRemove != null){
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;  
            case NetworkListEvent<LeaderEntityState>.EventType.Value:
                LeaderBoardEntityDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if(displayToUpdate != null){
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        
        }
    }


}
