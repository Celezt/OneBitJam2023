using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[HideMonoScript]
public class PlayerInputReceiver : Receiver
{
    public override GameObject Receive => PlayerInput.gameObject;
    public PlayerInput PlayerInput
    {
        get
        {
            if (_playerInput == null)
                _playerInput = PlayerInput.GetPlayerByIndex(_playerIndex);

            return _playerInput;
        }
    }
    public int PlayerIndex
    {
        get => _playerIndex;
        set => SetPlayerInput(value);
    }

    [SerializeField, Space(8), MinValue(0), OnValueChanged(nameof(SetPlayerInput))]
    private int _playerIndex;

    private PlayerInput _playerInput;

    public void SetPlayerInput(int playerIndex)
    {
        if (_playerInput && _playerIndex == playerIndex)
            return;

        _playerIndex = playerIndex;
        _playerInput = PlayerInput.GetPlayerByIndex(_playerIndex);
    }
}
