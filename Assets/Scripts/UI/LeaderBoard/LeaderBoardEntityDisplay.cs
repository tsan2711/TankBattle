using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColor;
    public ulong ClientId {get; private set;}
    public FixedString32Bytes PlayerName {get; private set;}
    public int Coins {get; private set;}
    public void Initialize(ulong clientId, FixedString32Bytes playerName, int coins){
        this.ClientId = clientId;
        this.PlayerName = playerName;
        this.Coins = coins;
        if(clientId == NetworkManager.Singleton.LocalClientId){
            displayText.color = myColor;
        }
        UpdateText();
    }

    public void UpdateCoins(int coins){
        Coins = coins;
        UpdateText();
    }

    public void UpdateText(){
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {PlayerName} ({Coins})";

    }
}
