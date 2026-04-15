using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    private Vector2 moveDir;
    private bool isCrouching = true;  // set false, if Game Over

    [SerializeField] private float speed;
    [SerializeField] private float pushPower;
    //[SerializeField] private float rotationSpeed;


    // GRAVITY
    [SerializeField] private float gravity = -20f;
    private float yVelocity;


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
        

        // ------- MOVEMENT -------        
        Vector3 dir = new Vector3(moveDir.x, 0, moveDir.y);  // remove height movement
        dir.Normalize();  // normalize for better speed handling

        Vector3 move = dir * speed * Time.deltaTime;  // calculate movement

        // set small downwards force when grounded and moving down
        if(controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;  // increase velocity by gravity per frame

        move.y = yVelocity;

        //transform.position = transform.position + move;
        controller.Move(move);  // move with character controller (animator)
    }
}
