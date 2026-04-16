using TriInspector;
using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    [SerializeField, LabelText("Animator for Gate")] private Animator animator;

    public void Interact(GameObject player)
    {
        // TODO: somehow start animation of player for lock picking

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        // TODO: start animation of gate swinging open

    }
}