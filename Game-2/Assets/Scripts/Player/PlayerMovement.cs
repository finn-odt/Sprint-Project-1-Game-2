using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    private Vector2 moveDir;
    private bool isCrouching = true;  // set false, if Game Over

    [SerializeField] private float speed;
    //[SerializeField] private float rotationSpeed;

    public void SetMovement(Vector2 dir)
    {
        moveDir = dir;
    }
    public void SetCrouching(bool crouchValue)
    {
        isCrouching = crouchValue;
    }

    void FixedUpdate()
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
        //transform.position = transform.position + move;
        controller.Move(move);  // move with character controller (animator)
    }
}
