using TriInspector;
using UnityEngine;

public class CameraTurnVolume : MonoBehaviour
{
    
    [SerializeField, LabelText("Target Camera Angle"), Range(0f, 360f)] private float targetAngle;
    [SerializeField, LabelText("Player 1 Trigger Layer")] private string triggerLayerPlayer1 = "Player1";
    [SerializeField, LabelText("Player 2 Trigger Layer")] private string triggerLayerPlayer2 = "Player2";
    private bool player1Detected = false;
    private bool player2Detected = false;
    private bool cameraTurned = false;

    private void Update()
    {
        // both players detected and camera not turned yet
        if(player1Detected && player2Detected && !cameraTurned)
        {
            CameraSwitcher.Instance.SetCameraTargetAngle(targetAngle);  // turn camera
            cameraTurned = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayerPlayer1))
            player1Detected = true;
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayerPlayer2))
            player2Detected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayerPlayer1))
            player1Detected = false;
        if(other.gameObject.layer == LayerMask.NameToLayer(triggerLayerPlayer2))
            player2Detected = false;
        cameraTurned = false;
    }
}
