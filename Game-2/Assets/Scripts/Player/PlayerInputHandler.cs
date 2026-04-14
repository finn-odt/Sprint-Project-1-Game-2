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

    private void OnGameStateChange(GameState newState)
    {
        // set isControllable=false, when Game Over
        if(newState == GameState.Lose)
            isControllable = false;
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
        
        CollisionDetection();
    }

    public Transform interactionCollider;  // collider for interaction
    public LayerMask interactionLayer;  // to which layer the collision is restricted

    // Collision Detection for Interactable Game Objects nearby
    private void CollisionDetection()
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
            
            return;
        }
    }
}
