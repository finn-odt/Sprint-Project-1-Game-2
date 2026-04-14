using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private bool areHandsConnected = false;
    private Material oldMaterial;
    [SerializeField] private Material handIndicatorMaterial;

    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void OnHandTriggerEnter(Collider other)
    {
        areHandsConnected = true;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        oldMaterial = meshRenderer.material;
        meshRenderer.material = handIndicatorMaterial;
    }

    public void OnHandTriggerExit(Collider other)
    {
        areHandsConnected = false;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        meshRenderer.material = oldMaterial;
    }
}
