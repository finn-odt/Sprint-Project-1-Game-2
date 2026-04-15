using UnityEngine;

public class Food : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        Destroy(gameObject);  // delete this object (food is eaten)
    }
}