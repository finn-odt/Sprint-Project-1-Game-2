using TriInspector;
using UnityEngine;

public class SkillcheckNPCController : NPCController
{
    private Animator animator;
    private Transform target;
    private bool wasTriggered = false;
    [SerializeField, LabelText("Skill Check Trigger Layer")] private string[] triggerLayers;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(!wasTriggered || target == null)
            return;

        Vector3 currPos = transform.position;
        Vector3 dir = target.position - currPos;  // direction from NPC to target (aka Player)
        dir.Normalize();

        // move NPC towards target (aka player)
        transform.LookAt(target.position);
        transform.position = currPos + dir * speed * Time.deltaTime;
    }

    override public void OnPlayerTriggerEnter(Collider other)
    {        
        target = other.gameObject.transform;  // set player as target
        wasTriggered = true;

        animator.SetTrigger("isRunning"); 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);

        // check if collided object has one of the triggerLayers
        foreach(string layer in triggerLayers) {
            if(other.gameObject.layer == LayerMask.NameToLayer(layer)) {  // if collider other player
                int playerID = int.Parse(layer.Replace("Player", ""));
                GameManager.Instance.TriggerSkillCheck(playerID, true);
                Destroy(gameObject);
                return;
            }
        }
    }
}
