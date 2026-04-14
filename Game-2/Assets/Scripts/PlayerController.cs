using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{

    InputActionMap inputMap;

    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    bool isControllable = true;  // is set false, when Game Over

    public void Awake()
    {
        inputMap = InputSystem.actions.FindActionMap("Player");
        inputMap.Enable();  // enable this map (to use it)
        InputSystem.actions["Move"].performed += OnMove;
        InputSystem.actions["Move"].canceled += OnMove;
        InputSystem.actions["PickUp"].performed += OnPickUp;
        InputSystem.actions["Drop"].performed += OnDrop;
    }
    void OnDestroy()
    {
        if (InputSystem.actions["Move"] != null)
        {
            InputSystem.actions["Move"].performed -= OnMove;
            InputSystem.actions["Move"].canceled  -= OnMove;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();  // move direction
    }

    /*
    public void (InputAction.CallbackContext context)
    {
        pickUp = context.ReadValueAsButton();  // pick up something
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        // remove mask child
        if(equipped.childCount > 0)
        {
            Transform mask = equipped.GetChild(0);
            mask.GetComponent<MaskItem>().DropMask();
            maskItem = null;
        }
    }
    */

    // Update is called once per frame
    void Update()
    {
        // ------- ANIMATOR -------
        if(isControllable && moveDir != Vector2.zero) 
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
        
        // ------- SOUNDS -------
        if(moveDir != Vector2.zero && walkSoundWaitTime > walkSoundClip.length)
        {
            AudioSource.PlayClipAtPoint(walkSoundClip, transform.position, 0.5f);  // at 50% volume
            walkSoundWaitTime = 0;
        }
        walkSoundWaitTime += Time.deltaTime;
        

        // ------- MOVEMENT -------

        if (!isControllable)
            return;
        
        // rotate if moving and not aligned with camera forward direction
        if(moveDir != Vector2.zero && !EqualRotation(mainCamera.transform.rotation, transform.rotation))
        {
            Quaternion q1 = transform.rotation; // rotation of player
            q1.x = q1.z = 0;
            Quaternion q2 = mainCamera.transform.rotation; // rotation of camera
            q2.x = q2.z = 0;
            transform.rotation = Quaternion.Lerp(q1, q2, rotationSpeed * Time.deltaTime);  // interpolate player rotation to camera rotation
        }

        Vector3 dir = mainCamera.transform.rotation * new Vector3(moveDir.x, 0, moveDir.y);  // multiply camera rotation with moving direction
        dir.y = 0;  // remove height movement
        dir.Normalize();  // normalize for better speed handling

        Vector3 move = dir * speed * Time.deltaTime;  // calculate movement
        controller.Move(move);  // move with character controller
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = 0;
        transform.position = pos;
    }

    bool EqualRotation(Quaternion a, Quaternion b, float maxAngleDeg = 0.1f)
    {
        return Quaternion.Angle(a, b) < maxAngleDeg;
    }

    private void GameOver()
    {
        isControllable = false;
        GameManager.Instance.GameOver();
    }
}
