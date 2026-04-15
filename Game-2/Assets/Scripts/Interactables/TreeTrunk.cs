using UnityEngine;

public class TreeTrunk : MonoBehaviour, IInteractable
{
    [SerializeField] private Rigidbody rb;

    public void Interact(GameObject player)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;  // enable physics for pushing
        rb.WakeUp();

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");

        Debug.Log("Tree Trunk now physical");
    }
}