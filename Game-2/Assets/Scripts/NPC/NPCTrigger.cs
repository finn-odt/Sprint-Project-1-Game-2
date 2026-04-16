using TriInspector;
using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField, LabelText("NPC Controller Component")] private NPCController npcController;
    [SerializeField, LabelText("Movement Trigger Layer")] private string[] triggerLayers;

    private bool wasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(wasTriggered)  // no multi-triggering
            return;

        // check if collided object has one of the triggerLayers
        foreach(string layer in triggerLayers) {
            if(other.gameObject.layer == LayerMask.NameToLayer(layer)) {  // if collider other player
                npcController.OnPlayerTriggerEnter(other);
                wasTriggered = true;
                return;
            }
        }
    }
}