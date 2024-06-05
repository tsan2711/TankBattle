using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField]
    public int MaxHealth {get; private set;}

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;

    // Use Health as reference to verify that this is instance assigning to OnDie
    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if(!IsServer){ return; }
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage){
        ModifyHealth(-damage);
    }

    public void RestoreHealth(int heal){
        ModifyHealth(heal);
    }

    private void ModifyHealth(int value){
        if(isDead){ return; }
        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0 ,MaxHealth);


        if(CurrentHealth.Value == 0){
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
