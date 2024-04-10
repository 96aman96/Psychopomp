using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    public bool canMusic, canFX;
    public UIManager _UIManager;
    public AudioManager _AudioManager;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }


}
