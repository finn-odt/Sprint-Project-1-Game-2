using UnityEngine;

public class Food : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Destroy(gameObject);  // delete this object (food is eaten)
    }
}