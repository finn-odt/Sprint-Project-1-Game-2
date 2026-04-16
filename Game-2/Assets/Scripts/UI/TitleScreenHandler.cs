using System;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class TitleScreenHandler : MonoBehaviour
{
    [SerializeField, LabelText("Scene Name after Title Screen")] private string firstSceneName;

    public void OnGameStart(CallbackContext context)
    {
        Debug.Log("Test");
        SceneManager.LoadScene(firstSceneName);
    }

    [SerializeField, LabelText("Input Action Asset")] private InputActionAsset actions;
    [SerializeField, LabelText("Action Map Name")] private string actionMapName = "UI";

    private InputActionMap inputMap;
    private InputAction startAction;

    private void Awake()
    {

        inputMap = actions.FindActionMap(actionMapName, true);
        startAction = inputMap.FindAction("StartGame", true);
    }

    private void OnEnable()
    {
        inputMap.Enable();

        startAction.performed += OnGameStart;
    }

    private void OnDisable()
    {
        startAction.performed -= OnGameStart;

        inputMap.Disable();
    }
}