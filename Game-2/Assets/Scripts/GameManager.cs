using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

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
