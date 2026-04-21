using UnityEngine;
using Unity.Cinemachine;
using TriInspector;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using System.Linq;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance { get; private set; }

    [SerializeField, LabelText("Normal Gameplay Camera")] private CinemachineCamera gameplayCam;
    [SerializeField, LabelText("Player Target Group")] private Transform targetGroup;
    private float targetYaw, currentYaw = 0;  // for rotation of camera by rotating the targetGroup [absolute values]
    private float turnSpeed;

    private List<GameObject> deactivatedObjects;  // due to visibility problems

    private void Start()
    {
        deactivatedObjects = new List<GameObject>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if(targetYaw != currentYaw)
            UpdateYaw();

        ClearViewBlockingObjects();
    }

    public void OnDrawGizmos()
    {
        List<CinemachineTargetGroup.Target> players = targetGroup.GetComponent<CinemachineTargetGroup>().Targets;
        if(players.Count != 2)
            return;

        Vector3 p1Pos = players[0].Object.position;
        Vector3 p2Pos = players[1].Object.position;

        Gizmos.color = new Color(255, 0, 0);
        Gizmos.DrawLine(transform.position, p1Pos + new Vector3(0,1f,0));
    }

    private void ClearViewBlockingObjects()
    {
        List<CinemachineTargetGroup.Target> players = targetGroup.GetComponent<CinemachineTargetGroup>().Targets;
        if(players.Count != 2)
            return;

        Vector3 p1Pos = players[0].Object.position + new Vector3(0, 1f, 0);
        Vector3 p2Pos = players[1].Object.position + new Vector3(0, 0.8f, 0);

        Vector3 direction = p1Pos - transform.position;
        float distance = Vector3.Distance(p1Pos, transform.position);
        direction.Normalize();
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, direction, distance - 2f);

        List<GameObject> tempList = new List<GameObject>();
        for(int i = 0; i < hits.Length; i++)
        {
            GameObject obj = hits[i].collider.gameObject;
            Renderer r = obj.GetComponentInChildren<Renderer>();
            if(r != null) {
                r.enabled = false;  // hide object between players and camera
                tempList.Add(obj);
            }
        }
        Debug.Log($"All Hits tempList.Count={tempList.Count}");
        Debug.Log($"Old hits.Count={deactivatedObjects.Count}");
        // show objects that are not intercepting anymore
        for(int i = deactivatedObjects.Count - 1; i >= 0; i--)
        {
            if(!tempList.Contains(deactivatedObjects[i]))
            {
                Debug.Log($"Removed {deactivatedObjects[i].gameObject.name} from deactivated list");
                // was not hit this time -> set active again
                Renderer r = deactivatedObjects[i].GetComponentInChildren<Renderer>();
                r.enabled = true;  // show object again
                deactivatedObjects.Remove(deactivatedObjects[i]);
            } else
            {
                Debug.Log($"Removed {deactivatedObjects[i].gameObject.name} from temp list");
                // is already in list of deactivated objects
                tempList.Remove(deactivatedObjects[i]);
            }
        }
        Debug.Log($"Cleaned old hits.Count={deactivatedObjects.Count}");
        Debug.Log($"Cleaned tempList.Count={tempList.Count}");
        // add new intercepting objects into list
        deactivatedObjects.AddRange(tempList);
    }

    public void SetCameraTargetAngle(float yawAngle, float rotationSpeed)
    {
        targetYaw = yawAngle;
        turnSpeed = rotationSpeed;  // set rotation speed
        GameManager.Instance?.CameraTurnAngle?.Invoke(yawAngle);
    }

    private void UpdateYaw()
    {
        currentYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, turnSpeed * Time.deltaTime);
        targetGroup.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    /*public void SwitchToGameplay()
    {
        gameplayCam.Priority = 20;
    }

    public void SwitchToOnShoulder()
    {
        gameplayCam.Priority = 10;
    }

    public void SwitchToHighlight()
    {
        gameplayCam.Priority = 10;
    }*/
}

