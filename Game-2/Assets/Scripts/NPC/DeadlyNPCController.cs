using System;
using TriInspector;
using UnityEngine;

public class DeadlyNPCController : NPCController
{

    [SerializeField, LabelText("Movement-Target")] private Transform target;
    private bool wasTriggered = false;

    // Update is called once per frame
    void Update()
    {
        if(!wasTriggered || target == null)
            return;

        Vector3 currPos = transform.position;
        Vector3 dir = target.position - currPos;  // direction from NPC to target
        dir.Normalize();

        // move NPC towards target
        transform.position = currPos + dir * speed * Time.deltaTime;
    }

    override public void OnPlayerTriggerEnter(Collider other)
    {
        // target set from outside
        wasTriggered = true;
    }
}
