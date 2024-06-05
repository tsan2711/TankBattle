using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using System.Numerics;
using System;
using System.Collections;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    private InputReader inputReader;
    [SerializeField]
    private Transform playerBody;

    [SerializeField]
    private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField]
    private float moveSpeed = 4f;
    [SerializeField]
    private float turningRate = 30f;

    private UnityEngine.Vector2 previousMovementInput;
    private UnityEngine.Vector2 previousMousePosition;

    public override void OnNetworkSpawn(){
        if(!IsOwner) {return;}
        inputReader.MoveEvent += HandleMove;
        inputReader.MoveEvent += HandleAim;
    }
    
    public override void OnNetworkDespawn(){
        if(!IsOwner) {return;}
        inputReader.MoveEvent -= HandleMove;
        inputReader.MoveEvent -= HandleAim;
    }


    private void Update(){
        if(!IsOwner) { return; }
        HandleRotate();
    }
    
    private void FixedUpdate() {
        if(!IsOwner) { return; }
        
        rb.velocity = (UnityEngine.Vector2)playerBody.up * previousMovementInput.y * moveSpeed;
    }
    
    private void HandleMove(UnityEngine.Vector2 offset){
        previousMovementInput = offset;
    }

    private void HandleAim(UnityEngine.Vector2 mousePosition)
    {
        previousMousePosition = mousePosition;
    }

    private void HandleRotate(){
        // Rotate Body
        float zRotation = previousMovementInput.x * (-turningRate) * Time.deltaTime;
        playerBody.Rotate(0f,0f, zRotation);


    }

}
