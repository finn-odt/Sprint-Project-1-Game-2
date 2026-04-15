using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private InputActionAsset actions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private int _playerIndex = 0;

    private InputActionMap inputMap;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction skillCheck1Action;
    private InputAction skillCheck2Action;
    private InputAction skillCheck3Action;

    private int skillCheck1Count = 0, skillCheck2Count = 0, skillCheck3Count = 0;
    private int skillButtonIndexToPress;

    private bool isControllable = true;  // set false, if Game Over
    private bool skillCheckActive = false;  // set true, when skill check event is triggered

    private void Start()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.GameStateChanged += OnGameStateChange;

            GameManager.Instance.SkillCheck += OnSkillCheck;
        }
    }

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        inputMap = actions.FindActionMap(actionMapName, true);
        moveAction = inputMap.FindAction("Move", true);

        
        crouchAction = inputMap.FindAction("Crouch", true);
        interactAction = inputMap.FindAction("Interact", true);

        skillCheck1Action = inputMap.FindAction("SkillCheck1", true);
        skillCheck2Action = inputMap.FindAction("SkillCheck2", true);
        skillCheck3Action = inputMap.FindAction("SkillCheck3", true);

        //pickUpPlayerAction = inputMap.FindAction("PickUpPlayer");  // can be null for Player2 (Tiny)
    }

    void Update()
    {
        InteractionCollisionDetection();

        if(skillCheckActive)
        { 
            // were multiple or the wrong buttons pressed -> failure
            if(CountPressedSkillButtons() > 1 || (CountPressedSkillButtons() > 0 && !CheckForSkillButtonPress()))
            {
                GameManager.Instance.TakeTimeAway();
                DeactivateSkillCheck();
            } else if(CheckForSkillButtonPress()) {  // was correct button pressed?
                DeactivateSkillCheck();
            }
        }
    }

    private void DeactivateSkillCheck()
    {
        GameManager.Instance.SkillCheckFinished.Invoke();  // trigger end of skill check

        Debug.Log("Skill Check End");
        skillCheck1Count = skillCheck2Count = skillCheck3Count = 0;
        skillCheckActive = false;
        isControllable = true;
    }

    private bool CheckForSkillButtonPress()
    {
        // checks if correct button was pressed
        if(skillCheck1Count > 0 && skillButtonIndexToPress == 1)
            return true;
        if(skillCheck2Count > 0 && skillButtonIndexToPress == 2)
            return true;
        if(skillCheck3Count > 0 && skillButtonIndexToPress == 3)
            return true;
        return false;
    }

    private int CountPressedSkillButtons()
    {
        int counter = 0;
        if(skillCheck1Count > 0)
            counter++;
        if(skillCheck2Count > 0)
            counter++;
        if(skillCheck3Count > 0)
            counter++;
        return counter;
    }

    private void OnGameStateChange(GameState newState)
    {
        // set isControllable=false, when Game Over/Won/Paused
        if(newState != GameState.Intro && newState != GameState.Playing)
            isControllable = false;
        else
            isControllable = true;
    }

    private void OnSkillCheck(int playerIndex, int buttonIndex)
    {
        if(playerIndex != _playerIndex)
            return;  // ignore, if Skill Check is for other player

        isControllable = false;
        skillCheckActive = true;
        skillButtonIndexToPress = buttonIndex;
    }

    private void OnSkillCheck1Input(CallbackContext context)
    {
        if(skillCheckActive)
            skillCheck1Count++;
    }
    private void OnSkillCheck2Input(CallbackContext context)
    {
        if(skillCheckActive)
            skillCheck2Count++;
    }
    private void OnSkillCheck3Input(CallbackContext context)
    {
        if(skillCheckActive)
            skillCheck3Count++;
    }

    private void OnEnable()
    {
        inputMap.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        crouchAction.performed += OnCrouch;
        crouchAction.canceled += OnCrouch;

        interactAction.performed += OnInteract;

        skillCheck1Action.performed += OnSkillCheck1Input;
        skillCheck2Action.performed += OnSkillCheck2Input;
        skillCheck3Action.performed += OnSkillCheck3Input;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        crouchAction.performed -= OnCrouch;
        crouchAction.canceled -= OnCrouch;

        interactAction.performed -= OnInteract;

        skillCheck1Action.performed -= OnSkillCheck1Input;
        skillCheck2Action.performed -= OnSkillCheck2Input;
        skillCheck3Action.performed -= OnSkillCheck3Input;

        inputMap.Disable();

        if (GameManager.Instance != null)
            GameManager.Instance.GameStateChanged -= OnGameStateChange;
    }

    public void OnMove(CallbackContext context)
    {
        if(!isControllable)
            playerMovement.SetMovement(Vector2.zero);
        else
            playerMovement.SetMovement(context.ReadValue<Vector2>());
    }

    public void OnCrouch(CallbackContext context)
    {
        if(!isControllable)
            return;
        
        playerMovement.SetCrouching(context.ReadValue<bool>());
    }

    public void OnInteract(CallbackContext context)
    {
        if(!isControllable)
            return;

        Debug.Log("Interaction wanted");
        
        StartInteraction();  // returns true, if interaction was started
    }

    public Transform interactionCollider;  // collider for interaction
    public LayerMask interactionLayer;  // to which layer the collision is restricted

    private int lastHitColliderCount = 0;

    // Collision Detection for Interactable Game Objects nearby
    private void InteractionCollisionDetection()
    {
        Collider[] hitColliders = Physics.OverlapBox(interactionCollider.position,
            interactionCollider.localScale,
            Quaternion.identity,
            interactionLayer);

        // deactivate indicator, when none are in radius
        if(hitColliders.Length == 0 && lastHitColliderCount != hitColliders.Length)
            GameManager.Instance?.InteractionStatusChanged.Invoke(null);

        lastHitColliderCount = hitColliders.Length;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            // execute interaction
            IInteractable interactable = hitColliders[i].GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                // set transform of interactable for UI Indicator
                GameManager.Instance?.InteractionStatusChanged.Invoke(hitColliders[i].transform);
            }
        }
    }
    private bool StartInteraction()
    {
        Collider[] hitColliders = Physics.OverlapBox(interactionCollider.position,
            interactionCollider.localScale,
            Quaternion.identity,
            interactionLayer);

        // check all collisions
        for (int i = 0; i < hitColliders.Length; i++)
        {
            // execute interaction
            IInteractable interactable = hitColliders[i].GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                // Trigger skill check for this player
                int playerID = int.Parse(LayerMask.LayerToName(gameObject.layer).Replace("Player", ""));
                GameManager.Instance.TriggerSkillCheck(playerID, Random.Range(1, 4));
                
                interactable.Interact(gameObject);  // parameter = player object for interaction
                return true;  // interaction started
            }
        }
        
        return false;
    }
}
