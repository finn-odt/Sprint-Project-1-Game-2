using System;
using System.Collections;
using TriInspector;
using UnityEngine;

public class GameStateChangeTrigger : MonoBehaviour
{

    [SerializeField] private GameState changeTarget;

    public void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.ChangeStateTo(changeTarget);
    }

}