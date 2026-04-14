using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance { get; private set; }

    [SerializeField] private CinemachineCamera gameplayCam;
    [SerializeField] private CinemachineCamera onShoulderCam;
    [SerializeField] private CinemachineCamera highlightCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SwitchToGameplay()
    {
        gameplayCam.Priority = 20;
        onShoulderCam.Priority = 10;
        //highlightCam.Priority = 10;
    }

    public void SwitchToOnShoulder()
    {
        gameplayCam.Priority = 10;
        onShoulderCam.Priority = 20;
        //highlightCam.Priority = 10;
    }

    public void SwitchToHighlight()
    {
        gameplayCam.Priority = 10;
        onShoulderCam.Priority = 10;
        //highlightCam.Priority = 20;
    }
}

