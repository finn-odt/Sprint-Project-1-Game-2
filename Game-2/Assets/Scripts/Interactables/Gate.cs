using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{

    public void Interact(GameObject player)
    {
        // TODO: somehow start animation of player for lock picking

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        // TODO: start animation of gate swinging open

    }
}