using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text lobbyName;
    [SerializeField]
    private TMP_Text lobbySlot;

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initialize(LobbiesList lobbiesList, Lobby lobby){
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyName.text = lobby.Name;
        lobbySlot.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

    }

    public void Join(){
        lobbiesList.JoinAsync(lobby);
    }

       
}
