using TriInspector;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField, LabelText("Death Trigger Layer")] private string[] triggerLayers;

    private bool wasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(wasTriggered)  // no multi-triggering
            return;

        // instant game over
        GameManager.Instance.GameOver();
    }
}