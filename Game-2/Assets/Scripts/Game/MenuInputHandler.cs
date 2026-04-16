using System.Linq;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class MenuInputHandler : MonoBehaviour
{

    [SerializeField, LabelText("Input Action Asset")] private InputActionAsset actions;
    [SerializeField, LabelText("Action Map Name")] private string actionMapName = "UI";

    private InputActionMap inputMap;
    private InputAction escapeAction;

    private void Awake()
    {

        inputMap = actions.FindActionMap(actionMapName, true);
        escapeAction = inputMap.FindAction("Pause/Restart", true);
    }

    private void OnEnable()
    {
        inputMap.Enable();

        escapeAction.performed += OnESC;
    }

    private void OnDisable()
    {
        escapeAction.performed -= OnESC;

        inputMap.Disable();
    }

    public void OnESC(CallbackContext context)
    {
        GameState currState = GameManager.Instance.GetGameState();

        if(currState == GameState.End || currState == GameState.Lose)
        {
            GameManager.Instance.RestartGame();
            return;
        }

        Debug.Log(currState);

        if(currState == GameState.Pause) {
            GameManager.Instance.ContinueGame();  // back to game
        } else {
            GameManager.Instance.PauseGame();  // pause game
        }
    }

}
