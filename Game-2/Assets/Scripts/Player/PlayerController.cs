using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private bool areHandsConnected = false;
    public Material oldMaterial;
    [SerializeField] private Material handIndicatorMaterial;

    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void OnHandTriggerEnter(Collider other)
    {
        if(areHandsConnected)
            return;

        areHandsConnected = true;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        oldMaterial = meshRenderer.material;
        meshRenderer.material = handIndicatorMaterial;
    }

    public void OnHandTriggerExit(Collider other)
    {
        if(!areHandsConnected)
            return;

        areHandsConnected = false;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        meshRenderer.material = oldMaterial;
    }
}
