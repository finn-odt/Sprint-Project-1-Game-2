using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerInitialization : MonoBehaviour
{
    [SerializeField] private InputActionAsset player1Actions;
    [SerializeField] private InputActionAsset player2Actions;

    private void Start()
    {
        SetupInputs();

        // Add Event for Device Changing (during runtime)
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad)
            return;

        // if device is gamepad and was added/removed/...
        if (change == InputDeviceChange.Added ||
            change == InputDeviceChange.Removed ||
            change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Reconnected)
        {
            SetupInputs();
        }
    }

    private void SetupInputs()
    {
        int pads = Gamepad.all.Count;

        player1Actions.Disable();
        player2Actions.Disable();

        if (pads == 0)
        {
            player1Actions.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            player1Actions.devices = GetKeyboardMouseDevices();

            player2Actions.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            player2Actions.devices = GetKeyboardMouseDevices();

            Debug.Log("Player1 and Player2 with Keyboard");
        }
        else if (pads == 1)
        {
            player1Actions.bindingMask = InputBinding.MaskByGroup("Gamepad");
            player1Actions.devices = new InputDevice[] { Gamepad.all[0] };

            player2Actions.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
            player2Actions.devices = GetKeyboardMouseDevices();

            Debug.Log("Player1 with Gamepad, Player2 with Keyboard");
        }
        else
        {
            player1Actions.bindingMask = InputBinding.MaskByGroup("Gamepad");
            player1Actions.devices = new InputDevice[] { Gamepad.all[0] };

            player2Actions.bindingMask = InputBinding.MaskByGroup("Gamepad");
            player2Actions.devices = new InputDevice[] { Gamepad.all[1] };

            Debug.Log("Player1 and Player2 with Gamepads");
        }

        player1Actions.Enable();
        player2Actions.Enable();
    }

    private InputDevice[] GetKeyboardMouseDevices()
    {
        var devices = new List<InputDevice>();

        if (Keyboard.current != null)
            devices.Add(Keyboard.current);

        if (Mouse.current != null)
            devices.Add(Mouse.current);

        return devices.ToArray();
    }
}