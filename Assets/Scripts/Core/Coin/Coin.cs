using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Coin : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    protected int coinValue;

    protected bool alreadyCollected;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract int Collect();

    public void SetValue(int value){
        coinValue = value;
    }

    protected void Show(bool show){
        spriteRenderer.enabled = show;
    }


}
