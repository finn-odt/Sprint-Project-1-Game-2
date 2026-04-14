using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private string triggerLayer;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayer))  // if collider other player
            playerController.OnHandTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayer))  // if collider other player
            playerController.OnHandTriggerExit(other);
    }
}