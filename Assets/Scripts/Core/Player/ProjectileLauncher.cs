using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    private InputReader inputReader;
    [SerializeField]
    private Transform projectileSpawnPoint;
    private CoinWallet wallet;
    [SerializeField]
    private Collider2D playerCollider;
    [SerializeField] 
    private GameObject buzzleFlashPrefab;
    [SerializeField]
    private GameObject clientProjectile;
    [SerializeField]
    private GameObject serverProjectile;

    [Header("Settings")]
    [SerializeField]
    private float projectileSpeed;
    [SerializeField] 
    private float buzzleFlashDuration;
    [SerializeField] 
    private float fireRate;
    [SerializeField] 
    private int costToFire;

    private float timer;

    private bool shouldFire;
    private float countBuzzleFlashTime;


    void Start()
    {
        wallet = GetComponent<CoinWallet>();
    }

    void Update()
    {
        if(countBuzzleFlashTime > 0f ){
            countBuzzleFlashTime -= Time.deltaTime;

            if(countBuzzleFlashTime <= 0f){
                buzzleFlashPrefab.SetActive(false);
            }
        }

        if(!IsOwner) { return; }
        if(timer > 0){
            timer -= Time.deltaTime;
        }

        if(!shouldFire){ return; }

        if(timer > 0) { return; }

        if(wallet.TotalCoin.Value < costToFire){ return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner){ return; }
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner){ return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire){
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction){
        
        if(wallet.TotalCoin.Value < costToFire){ return; }
        
        wallet.SpendingCoin(costToFire);
        
        GameObject projectile = Instantiate(
            serverProjectile, 
            spawnPos, 
            Quaternion.identity);

        projectile.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());
        
        if(projectile.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamge)){
            dealDamge.SetOwner(OwnerClientId);
        }
        
        
        if(projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.velocity = rb.transform.up * projectileSpeed;
        }


        PrimaryFireClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void PrimaryFireClientRpc(Vector3 spawnPos, Vector3 direction){
        if(IsOwner){ return; }
        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // Spawn Projectile and set direction to it
        GameObject projectile = Instantiate(
            clientProjectile, 
            spawnPos, 
            Quaternion.identity);
        projectile.transform.up = direction;
        // Spawn buzzle flash and count duration of it, then count time for next fire
        buzzleFlashPrefab.SetActive(true);
        countBuzzleFlashTime = buzzleFlashDuration;
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());
            

        if(projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }




}
