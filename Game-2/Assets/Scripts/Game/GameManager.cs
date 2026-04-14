using System;
using UnityEngine;
using TriInspector;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    GameState gameState = GameState.Park;  // set to first game state
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
        usedTime += Time.deltaTime;
        GameTimerUpdated?.Invoke(usedTime, gameTimer);

        // if players are not holding hands, do sanity meter updates
        if(!arePlayersHoldingHands) {
            sanityUpdateTimer += Time.deltaTime;

            if (sanityUpdateTimer >= sanityUpdateInterval)  // check if given time interval has passed
            {
                sanityUpdateTimer = 0f;
                DecreaseSanity();  //decrease sanity by given step
            }
        }

        // Check Loosing Condition
        if(sanity < sanityLimit || usedTime > gameTimer)
        {
            gameState = GameState.Lose;
            GameStateChanged?.Invoke(GameState.Lose);
        }
    }







    public Action<float> someValue;
    public Action somethingHappens;
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
