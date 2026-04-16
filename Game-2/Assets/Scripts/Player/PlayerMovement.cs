using TriInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, LabelText("Character Controller Component")] CharacterController controller;
    private Vector2 moveDir = new Vector2(0, 0);
    private bool isCrouching = true;  // set false, if Game Over

    [SerializeField, LabelText("Movement Speed")] private float speed;
    [SerializeField, LabelText("Push Power")] private float pushPower;
    //[SerializeField] private float rotationSpeed;

    [SerializeField, LabelText("Other Player Object")] private GameObject otherPlayer;
    [SerializeField, LabelText("Maximum Player Distance"), Range(0f, 50f)] private float maxPlayerDistance = 20;
    [SerializeField, LabelText("Maximum Player Distance (hand-held)"), Range(0f, 20f)] private float maxPlayerDistanceHoldingHands = 1.8f;
    private float maxDistanceBetweenPlayers;


    // GRAVITY
    [SerializeField, LabelText("Gravity")] private float gravity = -20f;
    private float yVelocity;

    private void Start()
    {
        maxDistanceBetweenPlayers = maxPlayerDistance;
        if (GameManager.Instance != null) {
            GameManager.Instance.PlayerHandsConnected += OnHandConnectionChange;
        }
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.PlayerHandsConnected -= OnHandConnectionChange;
        }
    }

    private void OnHandConnectionChange(bool handsConnected)
    {
        if(handsConnected)
            maxDistanceBetweenPlayers = maxPlayerDistanceHoldingHands;  // shorter distance while holding hands
        else
            maxDistanceBetweenPlayers = maxPlayerDistance;  // long distance in normal play
    }

    public void SetMovement(Vector2 dir)
    {
        moveDir = dir;
    }
    public void SetCrouching(bool crouchValue)
    {
        isCrouching = crouchValue;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;  // get rigidBody of colliding object

        if (rb == null || rb.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        // Is Object a TreeTrunk?
        if (hit.collider.GetComponent<TreeTrunk>() == null)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.AddForceAtPosition(
            pushDir * pushPower,
            hit.point,
            ForceMode.Impulse
        );
    }

    void Update()
    {
        // ------- ANIMATOR -------
        /*
        if(isControllable && moveDir != Vector2.zero) 
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
        */
        
        // ------- CHECK DISTANCE TO OTHER PLAYER -------
        Vector3 otherPos = otherPlayer.transform.position;
        Vector3 pos = transform.position;

        // ------- MOVEMENT -------        
        Vector3 dir = new Vector3(moveDir.x, 0, moveDir.y);  // remove height movement
        dir.Normalize();  // normalize for better speed handling

        Vector3 move = dir * speed * Time.deltaTime;  // calculate movement

        // set small downwards force when grounded and moving down
        if(controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;  // increase velocity by gravity per frame

        move.y = yVelocity;

        // make move only, when players are in the distance interval
        if(Vector3.Distance(otherPos, pos+move) < maxDistanceBetweenPlayers)
            controller.Move(move);  // move with character controller (animator)
        // TODO: else { call for other player }
    }
}
