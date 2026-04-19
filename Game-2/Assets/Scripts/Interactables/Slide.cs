using System.Collections;
using TriInspector;
using UnityEngine;

public class Slide : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform platformBoden;
    [SerializeField] private Transform platformWinde;
    [SerializeField] private Transform platformTarget;
    [SerializeField] private Transform platformStart;

    private Transform oldPlayerParent;

    public void Interact(GameObject player)
    {
        oldPlayerParent = player.transform.parent;
        player.transform.SetParent(platformBoden);
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
            platform.position = platformStart.position + i * dirStep;
            yield return new WaitForSeconds(timeStep);
        }

        if(toTarget)
            platform.position = platformTarget.position;
        else
            platform.position = platformStart.position;

        // parent player back to old parent
        player.transform.SetParent(oldPlayerParent);
    }
}