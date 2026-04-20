using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

public class Slide : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformAnchor;
    [SerializeField] private Transform platformTarget;
    [SerializeField] private Transform platformStart;
    [SerializeField] private Transform playerTarget;
    private Collider slideCollider;

    private Transform oldPlayerParent;

    private List<int> travelledPlayers;
    private void Awake()
    {
        slideCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        travelledPlayers = new List<int>();
    }

    public void Interact(GameObject player)
    {
        // stop player movement
        int idx = player.GetComponentInChildren<PlayerInputHandler>().playerIndex;

        if(travelledPlayers.Contains(idx))  // TODO: problem that skillcheck is triggered before
            return;

        GameManager.Instance.PlayerControllable?.Invoke(idx, false);  // set non-controllable

        travelledPlayers.Add(idx);

        // deactivate Collider
        if(slideCollider != null) {
            slideCollider.enabled = false;
        }
        // save old parent for later
        oldPlayerParent = player.transform.parent;
        
        // parent player to platform anchor
        player.transform.SetParent(platformAnchor);
        player.transform.localPosition = new Vector3(-0.000600000028f,0.000180000003f,-0.000440000003f);

        StartCoroutine(MovePlatform(5f, true, player));
    }

    private IEnumerator MovePlatform(float time, bool toTarget, GameObject player)
    {
        yield return null;

        Vector3 dir;  // direction
        if(toTarget)
            dir = platformTarget.position - platformStart.position;
        else
            dir = platformStart.position - platformTarget.position;

        float timeStep = time / 100f;
        Vector3 dirStep = dir / 100f;

        for(int i = 0; i < 100; i++)
        {
            platform.position = platform.position + dirStep;
            yield return new WaitForSeconds(timeStep);
        }

        // set end position
        if(toTarget) {
            platform.position = platformTarget.position;
            Debug.Log("set to target");
        } else {
            platform.position = platformStart.position;
            Debug.Log("set to start");
        }

        if(player != null) {
            // parent player back to old parent
            player.transform.SetParent(oldPlayerParent);  // keep world transform
            player.transform.position = playerTarget.position;

            int idx = player.GetComponentInChildren<PlayerInputHandler>().playerIndex;
            GameManager.Instance.PlayerControllable?.Invoke(idx, true);  // set controllable
        }

        // move back to start position
        if(toTarget)
        {
            StartCoroutine(MovePlatform(4f, false, null));
        }

        // activate collider again
        if(slideCollider != null) {
            slideCollider.enabled = true;
        }
    }
}