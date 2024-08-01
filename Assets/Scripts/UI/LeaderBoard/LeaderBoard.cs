using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    [SerializeField]
    private Transform leaderBoardEntityHolder;
    [SerializeField]
    private LeaderBoardEntityDisplay leaderBoardEntityPrefab;

    [SerializeField]
    private int entityToDisplay = 7;
    private NetworkList<LeaderEntityState> leaderBoardEntities;
    private List<LeaderBoardEntityDisplay> entityDisplays = new List<LeaderBoardEntityDisplay>();


    private void Awake()
    {
        leaderBoardEntities = new NetworkList<LeaderEntityState>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged += HandleLeaderBoardEntitiesChanged;
            foreach (LeaderEntityState entity in leaderBoardEntities)
            {
                HandleLeaderBoardEntitiesChanged(new NetworkListEvent<LeaderEntityState>
                {
                    Type = NetworkListEvent<LeaderEntityState>.EventType.Add,
                    Value = entity
                });
            }

        }
        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }

    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged -= HandleLeaderBoardEntitiesChanged;
        }
        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderBoardEntities.Add(new LeaderEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });
        player.CoinWallet.TotalCoin.OnValueChanged += (oldCoins, newCoins) =>
            HandleCoinChanged(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (leaderBoardEntities == null) { return; }

        foreach (LeaderEntityState entity in leaderBoardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }

            leaderBoardEntities.Remove(entity);
            break;
        }
        player.CoinWallet.TotalCoin.OnValueChanged -= (oldCoins, newCoins) =>
            HandleCoinChanged(player.OwnerClientId, newCoins);
    }


    private void HandleLeaderBoardEntitiesChanged(NetworkListEvent<LeaderEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderEntityState>.EventType.Add:
                if (!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
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
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderEntityState>.EventType.Value:
                LeaderBoardEntityDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entityToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderBoardEntityDisplay myDisplay = entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entityToDisplay)
            {
                leaderBoardEntityHolder.GetChild(entityToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);

            }
        }
    }

    private void HandleCoinChanged(ulong clientId, int newCoin)
    {
        // if(!IsServer){return;}   

        for (int i = 0; i < leaderBoardEntities.Count; i++)
        {
            if (leaderBoardEntities[i].ClientId != clientId) { continue; }

            leaderBoardEntities[i] = new LeaderEntityState
            {
                ClientId = leaderBoardEntities[i].ClientId,
                PlayerName = leaderBoardEntities[i].PlayerName,
                Coins = newCoin
            };
            return;
        }
    }


}
