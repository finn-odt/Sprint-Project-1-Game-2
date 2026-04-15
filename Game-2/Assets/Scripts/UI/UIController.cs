using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider gameTimerSlider;
    [SerializeField] private Slider sanitySlider;
    [SerializeField] private GameObject interactionIndicator;

    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Canvas winCanvas;

    private void Start()
    {
        if(GameManager.Instance != null) {
            GameManager.Instance.GameTimerUpdated += DisplayGameTimer;
            GameManager.Instance.SanityUpdated += DisplaySanity;

            GameManager.Instance.InteractionStatusChanged += ShowInteractionIndicator;

            GameManager.Instance.GameStateChanged += OnGameStateChange;
        }

        interactionIndicator.SetActive(false);

        // deactivate sanity slider at first
        sanitySlider.gameObject.SetActive(false);

        gameCanvas.gameObject.SetActive(true);
        pauseCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.GameTimerUpdated -= DisplayGameTimer;
        GameManager.Instance.SanityUpdated -= DisplaySanity;
        GameManager.Instance.InteractionStatusChanged -= ShowInteractionIndicator;
            GameManager.Instance.GameStateChanged -= OnGameStateChange;
    }

    private void DisplayGameTimer(float usedTime, float totalTime)
    {
        // first: usedTime, second: totalTime
        float percentage = usedTime / totalTime;

        if(1f - percentage > 0f)
        {
            gameTimerSlider.gameObject.SetActive(true);
            gameTimerSlider.value = 1f - percentage;
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

        Vector3 worldPos = interactable.position + new Vector3(0, 0.5f, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            gameCanvas.worldCamera,
            out Vector2 localPos
        );

        // place indicator over interactable object
        interactionIndicator.transform.localPosition = localPos;
    }

    private void OnGameStateChange(GameState newState)
    {
        if(newState == GameState.Playing)
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
}
