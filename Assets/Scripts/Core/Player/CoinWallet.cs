using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoin = new NetworkVariable<int>();


    private void OnTriggerEnter2D(Collider2D other) {
        if(!IsServer) {return;}
        if(!other.TryGetComponent<RespawningCoin>(out RespawningCoin coin)){return;}
        int coinValue = coin.Collect();
        TotalCoin.Value += coinValue;
    }

    public void SpendingCoin(int coins){
        if(!IsServer) {return;}
        TotalCoin.Value -= coins;
    }
}
