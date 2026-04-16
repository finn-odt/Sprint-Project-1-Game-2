using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TriInspector;
using TMPro;
using System;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField, LabelText("Timer UI-Text")] private GameObject timerTextObject;
    private TextMeshProUGUI timerText;
    [SerializeField, LabelText("Sanity UI-Slider")] private Slider sanitySlider;
    [SerializeField, LabelText("Interaction Indicator")] private GameObject interactionIndicator;
    [SerializeField, LabelText("Skill Check Indicator")] private GameObject skillCheckIndicator;

    [SerializeField, LabelText("Gameplay Canvas")] private Canvas gameCanvas;
    [SerializeField, LabelText("Pause Menu Canvas")] private Canvas pauseCanvas;
    [SerializeField, LabelText("Game Over Canvas")] private Canvas gameOverCanvas;
    [SerializeField, LabelText("Win Canvas")] private Canvas winCanvas;
    [SerializeField, LabelText("Connection Mode Tags")] private string[] connectionModeTags;

    private void Start()
    {
        if(GameManager.Instance != null) {
            GameManager.Instance.GameTimerUpdated += DisplayGameTimer;
            GameManager.Instance.SanityUpdated += DisplaySanity;

            GameManager.Instance.InteractionStatusChanged += ShowInteractionIndicator;

            GameManager.Instance.GameStateChanged += OnGameStateChange;

            GameManager.Instance.ConnectionModeChanged += OnConnectionModeChange;

            GameManager.Instance.SkillCheck += OnSkillCheckEnter;
            GameManager.Instance.SkillCheckFinished += OnSkillCheckExit;

            GameManager.Instance.TimePenalty += OnTimePenalty;
        }

        timerText = timerTextObject.GetComponent<TextMeshProUGUI>();

        // deactivate indicators
        interactionIndicator.SetActive(false);
        skillCheckIndicator.SetActive(false);

        // deactivate sanity slider at first
        sanitySlider.gameObject.SetActive(false);

        gameCanvas.gameObject.SetActive(true);
        pauseCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);

        OnConnectionModeChange();
    }

    private void OnConnectionModeChange()
    {
        // activate only the necessary UI elements (regarding Keyboard, Gamepad, Mixed)
        string visibleConnectionModeTag = GameManager.Instance.GetConnectionMode().ToString();
        
        // get all transforms (also inactive ones)
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach(string tag in connectionModeTags)
        {
            // filter for correct tag
            var objs = allTransforms
                .Where(t => t.CompareTag(tag))
                .Select(t => t.gameObject)
                .ToArray();

            foreach(GameObject obj in objs)
            {
                obj.SetActive(tag == visibleConnectionModeTag);
            }
        }    
    }

    private void OnDisable()
    {
        GameManager.Instance.GameTimerUpdated -= DisplayGameTimer;
        GameManager.Instance.SanityUpdated -= DisplaySanity;
        GameManager.Instance.InteractionStatusChanged -= ShowInteractionIndicator;
        GameManager.Instance.GameStateChanged -= OnGameStateChange;
        GameManager.Instance.SkillCheck -= OnSkillCheckEnter;
        GameManager.Instance.SkillCheckFinished -= OnSkillCheckExit;
        GameManager.Instance.TimePenalty -= OnTimePenalty;
    }

    private void DisplayGameTimer(float usedTime, float totalTime)
    {
        float timeLeft = totalTime - usedTime;
        string formatted = TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");

        if(timerText != null) {
            timerText.text = formatted;

            // change timer color, when only 10 seconds are left
            if(timeLeft <= 10) {                 
                timerText.color = new Color(1f, 0.4f, 0);
            }
        }
    }

    private void DisplaySanity(float sanity)
    {
        // sanity is already in percent (1 = 100%)

        if(sanity > 0f)
        {
            sanitySlider.gameObject.SetActive(true);
            sanitySlider.value = sanity;
        }
    }

    private void ShowInteractionIndicator(Transform interactable)
    {
        if(interactable == null) {
            interactionIndicator.SetActive(false);  // hide indicator
            return;
        }
        interactionIndicator.SetActive(true);

        // place indicator over interactable object
        interactionIndicator.transform.localPosition = GetScreenCoordinatesOfPlayer(interactable.position, new Vector3(0, 0, 0));
    }

    private void OnGameStateChange(GameState newState)
    {
        if(newState == GameState.Intro)  // TODO: change back to Playing
        {
            // activate sanity slider after intro
            sanitySlider.gameObject.SetActive(false);
        }

        switch(newState)
        {
            case GameState.Pause:  // PAUSED
                gameCanvas.gameObject.SetActive(false);
                gameOverCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                break;
            case GameState.End:  // WON
                gameCanvas.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(false);
                gameOverCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(true);
                break;
            case GameState.Lose:  // GAME OVER
                gameCanvas.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(false);
                gameOverCanvas.gameObject.SetActive(true);
                break;
            default:
                pauseCanvas.gameObject.SetActive(false);
                gameOverCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(false);
                gameCanvas.gameObject.SetActive(true);
                break;
        }
    }

    private void OnSkillCheckEnter(int playerIndex, int buttonIndex, bool triggeredByNPC)
    {
        // get player instance
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int playerLayer = LayerMask.NameToLayer("Player" + playerIndex);

        GameObject wantedPlayer = players.FirstOrDefault(player => player.layer == playerLayer);

        // place indicator over player
        skillCheckIndicator.SetActive(true);
        skillCheckIndicator.transform.localPosition = GetScreenCoordinatesOfPlayer(wantedPlayer.transform.position, new Vector3(0, 1f, 0));

        // show just the button that is needed
        int i = 1;
        // 6 elements in total (first 3 -> Player1, second 3 -> Player2)
        int modifiedButtonIndex = ((playerIndex - 1) * 3) + buttonIndex;
        foreach (Transform child in skillCheckIndicator.transform)
        {
            child.gameObject.SetActive(i == modifiedButtonIndex);
            i++;
        }
    }

    private void OnSkillCheckExit()
    {
        skillCheckIndicator.SetActive(false);
    }

    private Vector2 GetScreenCoordinatesOfPlayer(Vector3 position, Vector3 offset)
    {
        Vector3 worldPos = position + new Vector3(0, 0.5f, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            gameCanvas.worldCamera,
            out Vector2 localPos
        );

        return localPos;
    }

    private void OnTimePenalty()
    {
        timerText.color = new Color(1f, 0, 0);
        StartCoroutine(RecolorTimerText(2f));  // delay in seconds
    }
    private IEnumerator RecolorTimerText(float delay)
    {
        yield return new WaitForSeconds(delay);
        timerText.color = new Color(1f, 1f, 1f);
    }
}
