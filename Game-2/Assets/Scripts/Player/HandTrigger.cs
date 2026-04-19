using TriInspector;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    [SerializeField, LabelText("Player Controller Component")] private PlayerController playerController;
    [SerializeField, LabelText("Hand-Hold-Trigger Layer")] private string triggerLayer;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayer))  // if collider other player
            playerController.OnHandTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayer))  // if collider other player
            playerController.OnHandTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayer))  // if collider other player
            playerController.OnHandTriggerExit(other);
    }
}