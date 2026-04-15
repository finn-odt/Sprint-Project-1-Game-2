using System;
using UnityEngine;
using TriInspector;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private GameState gameState = GameState.Intro;  // set to first game state
    private GameState lastGameState = GameState.Intro;  // when pausing

    public GameState GetGameState()
    {
        return gameState;
    }
    public Action<GameState> GameStateChanged;

    [Unit("sec")] [SerializeField] private float gameTimer;
    private float usedTime = 0f;
    public Action<float, float> GameTimerUpdated;

    private float sanity = 1;
    public Action<float> SanityUpdated;
    [SerializeField] private float sanityLimit = 0.05f;

    public float GetSanity()
    {
        return sanity;
    }
    private void DecreaseSanity()
    {
        sanity -= sanityUpdateStep;
        SanityUpdated?.Invoke(sanity);
    }

    public Action<Transform> InteractionStatusChanged;  // transform of interactable object

    private bool arePlayersHoldingHands;

    [Unit("sec")]
    [SerializeField] private float sanityUpdateInterval = 1f;
    [SerializeField] private float sanityUpdateStep = 1f;
    private float sanityUpdateTimer;

    public void SetPlayersHoldingHands(bool isHoldingHands)
    {
        if (arePlayersHoldingHands == isHoldingHands)
            return;

        arePlayersHoldingHands = isHoldingHands;
        sanityUpdateTimer = sanityUpdateInterval;  // set timer to interval, to execute instantly
    }

    private void Update()
    {
        // don't update timer, when paused/game over/won
        if(gameState == GameState.Intro || gameState == GameState.Playing) {
            usedTime += Time.deltaTime;
            GameTimerUpdated?.Invoke(usedTime, gameTimer);
        }

        // don't update timer, when paused/game over/won/intro-level
        if(gameState == GameState.Playing) {
            // if players are not holding hands, do sanity meter updates
            if(!arePlayersHoldingHands) {
                sanityUpdateTimer += Time.deltaTime;

                if (sanityUpdateTimer >= sanityUpdateInterval)  // check if given time interval has passed
                {
                    sanityUpdateTimer = 0f;
                    DecreaseSanity();  //decrease sanity by given step
                }
            }
        }

        // Check Loosing Condition
        if(sanity < sanityLimit || usedTime > gameTimer)
        {
            gameState = GameState.Lose;
            GameStateChanged?.Invoke(gameState);
        }
    }

    public void RestartGame()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueGame()
    {
        gameState = lastGameState;
        GameStateChanged?.Invoke(gameState);
    }

    public void PauseGame()
    {
        lastGameState = gameState;
        gameState = GameState.Pause;
        GameStateChanged?.Invoke(gameState);
    }







    //public Action<float> someValue;
    //public Action somethingHappens;
    /*

    // SET ACTION EVENT
    somethingHappens?.Invoke();

    // SUBSCRIBE
    private void OnEnable()
    {
        // add subscription for game manager action
        GameManager.Instance.PlayerGetsChased += OnPlayerGetsChased;
    }

    private void OnDisable()
    {
        // remove subscription for game manager action
        GameManager.Instance.PlayerGetsChased -= OnPlayerGetsChased;
    }

    private void OnPlayerGetsChased()
    {
        Debug.Log("Player is being chased!");
    }

    */

    private void Start()
    {
        CameraSwitcher.Instance.SwitchToGameplay();
        //CameraSwitcher.Instance.SwitchToOnShoulder();
    }

    private void Awake()
    {
        if (!Instance)
            Instance = this;  // create Instance, if not set
        else if (Instance && Instance != this)
            Destroy(this.gameObject);  // destroy all other/new GameManager-Instances
        
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
}
