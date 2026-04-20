using System;
using UnityEngine;
using TriInspector;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private DeviceConnection deviceConnection;

    public DeviceConnection GetConnectionMode()
    {
        return deviceConnection;
    }

    public void SetConnectionMode(DeviceConnection mode)
    {
        deviceConnection = mode;
    }

    public Action ConnectionModeChanged;

    public static GameManager Instance;
    private GameState gameState = GameState.Intro;  // set to first game state
    private GameState lastGameState = GameState.Intro;  // when pausing

    public GameState GetGameState()
    {
        return gameState;
    }
    public Action<GameState> GameStateChanged;

    [Unit("sec")] [SerializeField, LabelText("Time to complete game")] private float gameTimer;
    private float usedTime = 0f;
    public Action<float, float> GameTimerUpdated;

    private float sanity = 1;
    public Action<float> SanityUpdated;
    [SerializeField, LabelText("Sanity Game Over Limit")] private float sanityLimit = 0.05f;
    
    [SerializeField, LabelText("Maximum Player Distance"), Range(5f, 50f)] private float maxPlayerDistance = 20;
    
    public Action<float> CameraTurnAngle;

    public float GetMaximumPlayerDistance()
    {
        return maxPlayerDistance;
    }

    public float GetSanity()
    {
        return sanity;
    }
    private void DecreaseSanity()
    {
        // calculate factor by distance of players
        float dist = Vector3.Distance(playerObject1.transform.position, playerObject2.transform.position);
        float playerDistanceFactor = dist / maxPlayerDistance;  // between 0 and 1

        sanity -= sanityUpdateStep * playerDistanceFactor;
        SanityUpdated?.Invoke(sanity);
    }

    public Action<Transform> InteractionStatusChanged;  // transform of interactable object

    private bool arePlayersHoldingHands;

    [Unit("sec")] [SerializeField, LabelText("Sanity Update Interval")] private float sanityUpdateInterval = 1f;
    [SerializeField, LabelText("Maximum Sanity Update Step Size")] private float sanityUpdateStep = 1f;
    private float sanityUpdateTimer;

    public Action<bool> PlayerHandsConnected;

    private GameObject playerObject1, playerObject2;

    public Action<int, bool> PlayerControllable;  //Player-Index, isControllable

    public void SetPlayersHoldingHands(bool isHoldingHands)
    {
        if (arePlayersHoldingHands == isHoldingHands)
            return;

        // if changed, invoke event
        PlayerHandsConnected?.Invoke(isHoldingHands);

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
        if(gameState == GameState.Playing) {  // TODO: remove Intro
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
            GameOver();
        }
    }

    public void GameOver()
    {
        gameState = GameState.Lose;
        GameStateChanged?.Invoke(gameState);
    }
    public void ChangeStateTo(GameState newState)
    {
        if(newState == GameState.Playing && gameState == GameState.Intro || gameState == GameState.Playing) {
            gameState = newState;
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

    public Action<int, int, bool> SkillCheck;
    [SerializeField] private float timeForSkillCheck;
    public float GetTimeForSkillCheck()
    {
        return timeForSkillCheck;
    }
    public Action SkillCheckFinished;
    [Unit("sec")] [SerializeField, LabelText("Skill Check Failure Time Penalty")] private float skillCheckTimePenalty;
    [SerializeField, LabelText("Skill Check Failure Sanity Penalty"), Range(0f, 1f)] private float skillCheckSanityPenalty;

    public void TriggerSkillCheck(int playerIndex, bool triggeredByNPC)
    {
        int buttonIndex = UnityEngine.Random.Range(1, 4);  // random skill check button
        SkillCheck?.Invoke(playerIndex, buttonIndex, triggeredByNPC);
    }

    public Action TimePenalty, SanityPenalty;
    public void TakeTimeAway()
    {
        TimePenalty?.Invoke();
        usedTime += skillCheckTimePenalty;
    }

    public void TakeSanityAway()
    {
        SanityPenalty?.Invoke();
        sanity -= skillCheckSanityPenalty;
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

    */

    private void Start()
    {
        //CameraSwitcher.Instance.SwitchToGameplay();
        //CameraSwitcher.Instance.SwitchToOnShoulder();

        //CameraSwitcher.Instance.SetCameraTargetAngle(90);

        // get player instances
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(players.Length == 2)
        {
            playerObject1 = players[0];
            playerObject2 = players[1];
        }
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
