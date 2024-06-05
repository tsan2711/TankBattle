using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] 
    private Health health;
    [SerializeField] 
    private Image healthBarImg;

    public override void OnNetworkSpawn()
    {
        if(!IsClient){ return; }
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsClient){ return; }
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth){
        healthBarImg.fillAmount = (float)newHealth / health.MaxHealth;
    }

    
}   
