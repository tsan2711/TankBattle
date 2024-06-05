using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Numerics;

public class PlayerAim : NetworkBehaviour
{   
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

    private void LateUpdate() {
        if(!IsOwner){return;}
        UnityEngine.Vector2 aimScreenPosition = inputReader.AimPosition;
        UnityEngine.Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new UnityEngine.Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
        );
    }
}
