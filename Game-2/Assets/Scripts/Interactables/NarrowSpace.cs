using TriInspector;
using UnityEngine;

public class NarrowSpace : MonoBehaviour, IInteractable
{
    [SerializeField, LabelText("Entry Collider")] private BoxCollider entryCollider;

    public void Interact(GameObject player)
    {
        // narrow space is now open to squeeze through
        entryCollider.enabled = false;

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        // TODO: start squeeze animation [when to stop?]
    }
}