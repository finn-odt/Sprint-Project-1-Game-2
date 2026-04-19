using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    private bool areHandsConnected = false;

    [SerializeField, LabelText("VFX-Effect for Hand-Holding-Start")] private VisualEffect handConnectionEffect;
    [SerializeField, LabelText("VFX-Effect for Hand-Holding-End")] private VisualEffect handDivisionEffect;
    private static readonly int VFXpos1ID = Shader.PropertyToID("pos1");
    private static readonly int VFXpos2ID = Shader.PropertyToID("pos2");
    private static readonly int VFXisControlledID = Shader.PropertyToID("isControlled");

    private MeshRenderer meshRenderer;

    private void Start()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.PlayerHandsConnected += OnHandConnectionChange;
        }
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.PlayerHandsConnected -= OnHandConnectionChange;
        }
    }

    private void OnHandConnectionChange(bool handsConnected)
    {
        if(areHandsConnected && !handsConnected)
        {
            areHandsConnected = false;
        }
    }
    
    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void OnHandTriggerEnter(Collider other)
    {
        if(areHandsConnected)
            return;

        areHandsConnected = true;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);

        PlaceVFXBetweenPlayers();

        if(handDivisionEffect != null)
            handDivisionEffect.Stop();

        // play, when effect is set and not playing
        if(handConnectionEffect != null && !handConnectionEffect.HasAnySystemAwake())
            handConnectionEffect.Play();
        
        // OPTIONAL: replace with animation of hand holding?
    }
    public void OnHandTriggerStay(Collider other)
    {
        if(handConnectionEffect != null && handConnectionEffect.HasAnySystemAwake())
            PlaceVFXBetweenPlayers();
    }

    private void PlaceVFXBetweenPlayers()
    {
        // calculate placement centered between players
        Vector3 p2Pos = GetComponent<PlayerMovement>().otherPlayer.transform.position;
        Vector3 dir = transform.position - p2Pos;  // from P2 to P1
        dir.y = 0; // no vertical movement
        Vector3 vfxPos = p2Pos + 0.5f * dir;
        vfxPos.y = handConnectionEffect.transform.position.y;  // constant y-coordinate
        
        if(handConnectionEffect != null)
            handConnectionEffect.transform.position = vfxPos;
    }

    public void OnHandTriggerExit(Collider other)
    {
        if(!areHandsConnected)
            return;

        areHandsConnected = false;
        GameManager.Instance.SetPlayersHoldingHands(areHandsConnected);
        
        if(handConnectionEffect != null)
            handConnectionEffect.Stop();

        PlaceVFXAnchorsBetweenPlayers(true);  // place and play
    }

    private void PlaceVFXAnchorsBetweenPlayers(bool play)
    {
        if(handDivisionEffect == null || handDivisionEffect.GetBool(VFXisControlledID))
            return;
            
        handDivisionEffect.SetBool(VFXisControlledID, true);  // lock object for control

        // calculate placement centered between players
        Vector3 p2Pos = GetComponent<PlayerMovement>().otherPlayer.transform.position + new Vector3(0, 1f, 0);
        Vector3 p1Pos = transform.position + new Vector3(0, 1f, 0);

        handDivisionEffect.SetVector3(VFXpos1ID, p1Pos);
        handDivisionEffect.SetVector3(VFXpos2ID, p2Pos);

        if(play)
            handDivisionEffect.Play();

        handDivisionEffect.SetBool(VFXisControlledID, false);
    }
}
