using System.Collections;
using TriInspector;
using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    private Animator gateAnimator;
    [SerializeField, LabelText("Door Collider")] private BoxCollider doorCollider;

    private void Start()
    {
        gateAnimator = GetComponentInChildren<Animator>();
    }

    public void Interact(GameObject player)
    {
        // TODO: somehow start animation of player for lock picking


        // start animation of gate swinging open
        if(gateAnimator != null) {
            gateAnimator.SetTrigger("openGate");
            StartCoroutine(WaitForIdleOpen());
        }

        // remove Layer for clearing interactability
        gameObject.layer = LayerMask.NameToLayer("UsedInteractable");
    }

    private IEnumerator WaitForIdleOpen()
    {
        // wait one frame
        yield return null;

        while (gateAnimator != null)
        {
            AnimatorStateInfo state = gateAnimator.GetCurrentAnimatorStateInfo(0);

            if (!gateAnimator.IsInTransition(0) && state.IsName("idle_open"))
            {
                // gate is now fully open -> deactivate collider to go through
                doorCollider.enabled = false;
                yield break;
            }

            yield return null;
        }
    }
}