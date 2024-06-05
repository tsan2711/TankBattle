using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameInputField;
    [SerializeField]
    private TankPlayer player;

    private void Start() {
        HandlePlayerNameChanged(string.Empty,player.PlayerName.Value);
        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void OnDestroy() {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
        
    }
    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        nameInputField.text = newName.ToString();
    }
}
