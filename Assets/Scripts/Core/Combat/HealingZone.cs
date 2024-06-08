using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;
    [Header("Settings")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 20;
    [SerializeField] private int healPerTick = 10;

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    private float remainingCooldown;
    private float tickTimer;

    public override void OnNetworkSpawn()
    {
        if(IsClient){
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }
        if(IsServer){
            HealPower.Value = maxHealPower;
        }
    }
    public override void OnNetworkDespawn()
    {
        if(IsClient){
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void Update() {
        if(!IsServer){return;}
        
        if(remainingCooldown > 0f){
            remainingCooldown -= Time.deltaTime;
            if(remainingCooldown <= 0f){
                HealPower.Value = maxHealPower;
            } else {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        if(tickTimer >= 1 / healTickRate){
            foreach (TankPlayer player in playersInZone)
            {
                if(HealPower.Value == 0){break;}
                if(player.Health.CurrentHealth.Value == player.Health.MaxHealth){continue;}
                if(player.CoinWallet.TotalCoin.Value < coinsPerTick){continue;}

                player.CoinWallet.SpendingCoin(coinsPerTick);
                player.Health.RestoreHealth(healPerTick);

                HealPower.Value -= 1;
                if(HealPower.Value == 0){
                    remainingCooldown = healCooldown;
                }
            }
        }
        tickTimer = tickTimer % (1/healTickRate);
    }

    private void HandleHealPowerChanged(int oldHeal, int newHeal)
    {
        healPowerBar.fillAmount = (float) newHeal / maxHealPower;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Add(player);

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Remove(player);
    }

}

