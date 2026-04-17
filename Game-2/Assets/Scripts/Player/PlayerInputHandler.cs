using System.Collections;
using System.Linq;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{

    private Animator animator;

    [SerializeField, LabelText("Player Movement Script Component")] private PlayerMovement playerMovement;

    [SerializeField, LabelText("Player Input Action Asset")] private InputActionAsset actions;
    [SerializeField, LabelText("Action Map Name")] private string actionMapName = "Player";
    [SerializeField, LabelText("Player-Index"), Range(1, 2)] private int _playerIndex = 0;
    [SerializeField, LabelText("Allowed Interaction Tags")] private string[] allowedInteractionTags;

    private InputActionMap inputMap;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction skillCheck1Action;
    private InputAction skillCheck2Action;
    private InputAction skillCheck3Action;

    private int skillCheck1Count = 0, skillCheck2Count = 0, skillCheck3Count = 0;
    private int skillButtonIndexToPress;
    private bool skillCheckTriggeredByNPC = false;

    private bool isControllable = true;  // set false, if Game Over
    private bool skillCheckActive = false;  // set true, when skill check event is triggered

    private void Start()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.GameStateChanged += OnGameStateChange;

            GameManager.Instance.SkillCheck += OnSkillCheck;
        }
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        inputMap = actions.FindActionMap(actionMapName, true);
        moveAction = inputMap.FindAction("Move", true);
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
            Debug.Log("Necessary-Button-Index: " + skillButtonIndexToPress);
            // were multiple or the wrong buttons pressed -> failure
            if(CountPressedSkillButtons() > 1 || (CountPressedSkillButtons() > 0 && !CheckForSkillButtonPress()))
            {
                if(skillCheckTriggeredByNPC)
                    GameManager.Instance.TakeSanityAway();
                else
                    GameManager.Instance.TakeTimeAway();

                DeactivateSkillCheck();
            } else if(CheckForSkillButtonPress()) {  // was correct button pressed?
                DeactivateSkillCheck();
            }
        }
    }

    private void DeactivateSkillCheck()
    {
        GameManager.Instance.SkillCheckFinished?.Invoke();  // trigger end of skill check

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

    private void OnSkillCheck(int playerIndex, int buttonIndex, bool triggeredByNPC)
    {
        if(playerIndex != _playerIndex)
            return;  // ignore, if Skill Check is for other player

        isControllable = false;
        skillCheckActive = true;
        skillButtonIndexToPress = buttonIndex;
        skillCheckTriggeredByNPC = triggeredByNPC;
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

        interactAction.performed += OnInteract;

        skillCheck1Action.performed += OnSkillCheck1Input;
        skillCheck2Action.performed += OnSkillCheck2Input;
        skillCheck3Action.performed += OnSkillCheck3Input;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        interactAction.performed -= OnInteract;

        skillCheck1Action.performed -= OnSkillCheck1Input;
        skillCheck2Action.performed -= OnSkillCheck2Input;
        skillCheck3Action.performed -= OnSkillCheck3Input;

        inputMap.Disable();

        if (GameManager.Instance != null) {
            GameManager.Instance.GameStateChanged -= OnGameStateChange;

            GameManager.Instance.SkillCheck -= OnSkillCheck;
        }
    }

    public void OnMove(CallbackContext context)
    {
        if(!isControllable) {
            playerMovement.SetMovement(Vector2.zero);
        } else {
            Vector2 dir = context.ReadValue<Vector2>();
            playerMovement.SetMovement(dir);

            // set animator-parameters
            if(dir.magnitude > 0)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);
        }
    }

    public void OnInteract(CallbackContext context)
    {
        if(!isControllable)
            return;

        StartInteraction();  // returns true, if interaction was started
    }

    [SerializeField, LabelText("Interaction-Check Collider")] private Transform interactionCollider;  // collider for interaction
    [SerializeField, LabelText("Interaction-Collider Layermask")] private LayerMask interactionLayer;  // to which layer the collision is restricted

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
            GameManager.Instance?.InteractionStatusChanged?.Invoke(null);

        lastHitColliderCount = hitColliders.Length;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            // check if collider-object is in allowed interaction objects
            if(System.Array.IndexOf(allowedInteractionTags, hitColliders[i].gameObject.tag) == -1)
                continue;

            // execute interaction
            IInteractable interactable = hitColliders[i].GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                // set transform of interactable for UI Indicator
                GameManager.Instance?.InteractionStatusChanged?.Invoke(hitColliders[i].transform);
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
                string hitTag = hitColliders[i].gameObject.tag;
                // check if collider-object is in allowed interaction objects
                if(System.Array.IndexOf(allowedInteractionTags, hitTag) == -1)
                    continue;

                if(hitTag != "Wall" && hitTag != "Gate") {  // because animation starts to play instantly
                    // Trigger skill check for this player
                    int playerID = int.Parse(LayerMask.LayerToName(gameObject.layer).Replace("Player", ""));
                    GameManager.Instance.TriggerSkillCheck(playerID, false);
                }

                // Set Animator parameters
                switch(hitTag)
                {
                    case "Tree":
                        animator.SetBool("Push", true);
                        StartCoroutine(DeactivateAnimation(1.5f, "Push"));  // set false after 3s
                        break;
                    case "Gate":
                        animator.SetTrigger("lockpick");
                        break;
                    case "Wall":
                        animator.SetTrigger("Climb");
                        StartCoroutine(TeleportBehindWall(hitColliders[i].gameObject));  // check for end of animation
                        break;
                    case "Narrow":
                        animator.SetBool("squeeze", true);
                        StartCoroutine(DeactivateAnimation(6f, "squeeze"));  // set false after 6s
                        break;
                }
                
                interactable.Interact(gameObject);  // parameter = player object for interaction
                return true;  // interaction started
            }
        }
        
        return false;
    }

    private IEnumerator DeactivateAnimation(float delay, string animtorParameter)
    {
        yield return new WaitForSeconds(delay);

        animator.SetBool(animtorParameter, false);
    }

    private IEnumerator TeleportBehindWall(GameObject walls)
    {
        yield return new WaitForSeconds(0.3f); // wait for 0.3s

        while (animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (!animator.IsInTransition(0) && !state.IsName("climb"))
            {
                // climb animation is finished -> teleport behind wall
                Vector3 dir = walls.transform.position - transform.position;  // from player to wall
                dir.y = 0f;  // remove vertical movement
                dir.Normalize();

                transform.position += dir * 2f;
                yield break;
            }

            yield return null;
        }
    }
}
