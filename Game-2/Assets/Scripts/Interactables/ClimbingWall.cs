using TriInspector;
using UnityEngine;

public class ClimbingWall : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        // TODO: start climb animation, wait for end -> teleport behind wall
    }
}