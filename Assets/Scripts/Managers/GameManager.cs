using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#region ENUMS

enum TargetType
{
    PickupTrain,
    DropTrain,
    TargetRuin
}

#endregion
public class GameManager : MonoBehaviour
{
    
    public KinematicCharacterController player;
    public RandomLetterSpawner letterMaker;
    public UIManager _UIManager;
    public bool isPaused;
    public delegate void GameStateHandler();
    [SerializeField]
    public UnityEvent onGameStart;
    [SerializeField] 
    public UnityEvent  onGamePause;
    [SerializeField]
    public UnityEvent  OnGameResume;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
    }
    
    public void StartGame()
    {
        onGameStart.Invoke();
    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     if (isPaused)
        //     {
        //         isPaused = false;
        //         OnGameResume.Invoke();
        //     }
        //     else
        //     {
        //         isPaused = true;
        //         onGamePause.Invoke();
        //     }
        // }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
