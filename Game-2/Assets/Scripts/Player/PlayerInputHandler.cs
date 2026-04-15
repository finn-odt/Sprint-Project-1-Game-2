using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private InputActionAsset actions;
    [SerializeField] private string actionMapName = "Player";

    private InputActionMap inputMap;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction interactAction;

    private bool isControllable = true;  // set false, if Game Over

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameStateChanged += OnGameStateChange;
    }

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        inputMap = actions.FindActionMap(actionMapName, true);
        moveAction = inputMap.FindAction("Move", true);

        
        crouchAction = inputMap.FindAction("Crouch", true);
        interactAction = inputMap.FindAction("Interact", true);

        //pickUpPlayerAction = inputMap.FindAction("PickUpPlayer");  // can be null for Player2 (Tiny)
    }

    void Update()
    {
        InteractionCollisionDetection();
    }

    private void OnGameStateChange(GameState newState)
    {
        // set isControllable=false, when Game Over/Won/Paused
        if(newState != GameState.Intro && newState != GameState.Playing)
            isControllable = false;
        else
            isControllable = true;
    }

    private void OnEnable()
    {
        inputMap.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        crouchAction.performed += OnCrouch;
        crouchAction.canceled += OnCrouch;

        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        crouchAction.performed -= OnCrouch;
        crouchAction.canceled -= OnCrouch;

        interactAction.performed -= OnInteract;

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
                interactable.Interact(gameObject);  // parameter = player object for interaction
            }
            
            return true;  // interaction started
        }
        
        return false;
    }
}
