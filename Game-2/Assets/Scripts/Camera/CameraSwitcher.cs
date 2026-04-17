using UnityEngine;
using Unity.Cinemachine;
using TriInspector;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance { get; private set; }

    [SerializeField, LabelText("Normal Gameplay Camera")] private CinemachineCamera gameplayCam;
    [SerializeField, LabelText("Player Target Group")] private Transform targetGroup;
    private float targetYaw, currentYaw = 0;  // for rotation of camera by rotating the targetGroup [absolute values]
    private float turnSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if(targetYaw != currentYaw)
            UpdateYaw();
    }

    public void SetCameraTargetAngle(float yawAngle, float rotationSpeed)
    {
        targetYaw = yawAngle;
        turnSpeed = rotationSpeed;  // set rotation speed
        GameManager.Instance?.CameraTurnAngle?.Invoke(yawAngle);
    }

    private void UpdateYaw()
    {
        currentYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, turnSpeed * Time.deltaTime);
        targetGroup.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    /*public void SwitchToGameplay()
    {
        gameplayCam.Priority = 20;
    }

    public void SwitchToOnShoulder()
    {
        gameplayCam.Priority = 10;
    }

    public void SwitchToHighlight()
    {
        gameplayCam.Priority = 10;
    }*/
}

