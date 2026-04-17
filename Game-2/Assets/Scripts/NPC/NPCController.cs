using System;
using TriInspector;
using Unity.VisualScripting;
using UnityEngine;

public abstract class NPCController : MonoBehaviour
{

    [SerializeField, LabelText("Movement Speed")] protected float speed = 4;

    abstract public void OnPlayerTriggerEnter(Collider other);
}
