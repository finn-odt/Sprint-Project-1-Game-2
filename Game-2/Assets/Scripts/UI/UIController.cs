using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider gameTimerSlider;
    public Slider sanitySlider;

    private void Start()
    {
        if(GameManager.Instance != null) {
            GameManager.Instance.GameTimerUpdated += DisplayGameTimer;
            GameManager.Instance.SanityUpdated += DisplaySanity;
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.GameTimerUpdated -= DisplayGameTimer;
        GameManager.Instance.SanityUpdated -= DisplaySanity;
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
}
