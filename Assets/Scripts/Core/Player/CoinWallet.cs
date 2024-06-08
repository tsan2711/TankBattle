using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private BountyCoin bountyCoinPrefab;
    [Header("Settings")]
    [SerializeField] private float coinSpread = 2f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private float bountyPercentage = 50;
    [SerializeField] private int minBoutyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;
    public NetworkVariable<int> TotalCoin = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandlePlayerDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        health.OnDie -= HandlePlayerDie;
    }
    private void HandlePlayerDie(Health health)
    {
        int bountyValue = (int)(TotalCoin.Value * (bountyPercentage / 100));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBoutyCoinValue) { return; }


        for (int i = 0; i < bountyCoinCount; i++)
        {
            Vector2 spawnPoint = GetSpawnPoint();
            BountyCoin bountyCoinInstance = 
                Instantiate(bountyCoinPrefab, spawnPoint, Quaternion.identity);
            bountyCoinInstance.SetValue(bountyCoinValue);
            bountyCoinInstance.NetworkObject.Spawn();
        }
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 newPos = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;
            int numColliders =
                Physics2D.OverlapCircleNonAlloc(newPos, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0) { return newPos; }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (!other.TryGetComponent<RespawningCoin>(out RespawningCoin coin)) { return; }
        int coinValue = coin.Collect();
        TotalCoin.Value += coinValue;
    }

    public void SpendingCoin(int coins)
    {
        if (!IsServer) { return; }
        TotalCoin.Value -= coins;
    }


}
