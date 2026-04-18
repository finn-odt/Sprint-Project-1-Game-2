using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    private bool areHandsConnected = false;

    private VisualEffect handConnectionEffect;

    private MeshRenderer meshRenderer;

    private void Start()
    {
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
        if(areHandsConnected && !handsConnected)
        {
            areHandsConnected = false;
        }
    }
    
    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            
        if (handConnectionEffect == null) {
            handConnectionEffect = GetComponentInChildren<VisualEffect>();
            handConnectionEffect.Stop();
        }
    }

    public void OnHandTriggerEnter(Collider other)
    {
        if(areHandsConnected)
            return;

        areHandsConnected = true;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);

        if(handConnectionEffect)
            handConnectionEffect.Play();
        
        // OPTIONAL: replace with animation of hand holding?
        //oldMaterial = meshRenderer.material;
        //meshRenderer.material = handIndicatorMaterial;
    }

    public void OnHandTriggerExit(Collider other)
    {
        if(!areHandsConnected)
            return;

        areHandsConnected = false;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        //meshRenderer.material = oldMaterial;
    }
}
