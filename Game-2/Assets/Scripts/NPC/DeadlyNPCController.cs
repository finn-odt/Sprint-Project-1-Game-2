using System;
using System.Collections;
using TriInspector;
using UnityEngine;

public class DeadlyNPCController : NPCController
{

    private Animator animator;

    [SerializeField, LabelText("Movement-Target")] private Transform target;

    [Header("Patrol Mode")]
    [SerializeField, LabelText("Patrol between targets:")] private bool patrolMode;
    [SerializeField, LabelText("Movement-Targets (cyclic, in order)")] private Transform[] targets;
    private int currTargetIndex = 0;

    private bool wasTriggered = false;
    private bool deactivated = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if(patrolMode && targets.Length > 0)
        {
            target = targets[currTargetIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!wasTriggered || deactivated || (target == null && targets.Length == 0))
            return;

        animator.SetBool("isWalking", true); 
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
            if(patrolMode) {
                StartCoroutine(SetNewTarget(0.5f));
            }
        }
    }

    private IEnumerator SetNewTarget(float delay)
    {
        yield return new WaitForSeconds(delay);

        currTargetIndex = (currTargetIndex + 1) % targets.Length;  // cyclic rotation
        target = targets[currTargetIndex];
        deactivated = false; 
    }

    override public void OnPlayerTriggerEnter(Collider other)
    {
        if(wasTriggered)
            return;

        // target set from outside
        wasTriggered = true;
    }
}
