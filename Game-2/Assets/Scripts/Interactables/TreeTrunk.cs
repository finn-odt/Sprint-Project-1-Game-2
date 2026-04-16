using TriInspector;
using UnityEngine;

public class TreeTrunk : MonoBehaviour, IInteractable
{
    [SerializeField, LabelText("Rigidbody Component")] private Rigidbody rb;

    public void Interact(GameObject player)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;  // enable physics for pushing
        rb.WakeUp();

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        // TODO: start push animation [but when to stop???]

        Debug.Log("Tree Trunk now physical");
    }
}