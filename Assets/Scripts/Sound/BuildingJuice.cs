using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class BuildingJuice : MonoBehaviour
{
    private WwiseSoundManager wwiseSoundManager;

    private void Start()
    {
        wwiseSoundManager = GameObject.FindObjectOfType<WwiseSoundManager>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        
        if(other.CompareTag("Player"))
        {
            wwiseSoundManager.PlayBuilding();
        }
    }
    
}
