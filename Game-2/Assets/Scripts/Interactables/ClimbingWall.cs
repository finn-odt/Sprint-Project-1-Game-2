using TriInspector;
using UnityEngine;

public class ClimbingWall : MonoBehaviour, IInteractable
{
    [SerializeField, LabelText("Direction in local space")] private Vector3 localDirection = Vector3.forward;

    [SerializeField, Header("Visualization")] private float gizmoLength = 2f;
    [SerializeField] private Color gizmoColor = Color.magenta;
    public void Interact(GameObject player)
    {
        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");
    }

    public Vector3 WorldDirection
    {
        get
        {
            if (localDirection == Vector3.zero)
                return Vector3.zero;

            return transform.TransformDirection(localDirection.normalized);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (localDirection == Vector3.zero)
            return;

        Gizmos.color = gizmoColor;

        Vector3 start = transform.position;
        start.y += 1f;
        Vector3 end = start + WorldDirection * gizmoLength;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.08f);
    }
}