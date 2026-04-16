using System;
using TriInspector;
using UnityEngine;

public class DeadlyNPCController : NPCController
{

    private Animator animator;

    [SerializeField, LabelText("Movement-Target")] private Transform target;
    private bool wasTriggered = false;
    private bool deactivated = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!wasTriggered || target == null || deactivated)
            return;

        Vector3 currPos = transform.position;
        Vector3 dir = target.position - currPos;  // direction from NPC to target
        dir.Normalize();

        // move NPC towards target
        transform.LookAt(target.position);
        transform.position = currPos + dir * speed * Time.deltaTime;

        if(Vector3.Distance(currPos, target.position) < 0.1)
        {
           deactivated = true;
           animator.SetBool("isWalking", false); 
        }
    }

    override public void OnPlayerTriggerEnter(Collider other)
    {
        // target set from outside
        wasTriggered = true;

        animator.SetBool("isWalking", true); 
    }
}
