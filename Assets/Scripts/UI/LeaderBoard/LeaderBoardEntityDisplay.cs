using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderBoardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    public ulong ClientId {get; private set;}
    public FixedString32Bytes PlayerName {get; private set;}
    public int Coins {get; private set;}
    public void Initialize(ulong clientId, FixedString32Bytes playerName, int coins){
        this.ClientId = clientId;
        this.PlayerName = playerName;
        this.Coins = coins;
        UpdateText();
    }

    public void UpdateCoins(int coins){
        Coins = coins;
        UpdateText();
    }

    private void UpdateText(){
        displayText.text = $"1. {PlayerName} ({Coins})";

    }
}
