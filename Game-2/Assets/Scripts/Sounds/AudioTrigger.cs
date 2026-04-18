using UnityEngine;
using System;

public class AudioTrigger : MonoBehaviour
{
    public event Action<Collider> TriggerEntered;
    public event Action<Collider> TriggerExited;
    public event Action<Collider> TriggerStays;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEntered?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExited?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStays?.Invoke(other);
    }
}