using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseHandler : MonoBehaviour
{
    public int IdleTimeSetting = 5;
    public float IdleTime;
    public float idleInSeconds;
    public GameObject IdleAnimator;
    void Awake()
    {
        idleInSeconds = IdleTimeSetting * 60;
    }
        
    private void Update()
    {
        if (IdleTime >= idleInSeconds-4)
        {
            IdleAnimator.gameObject.SetActive(true);
        }
        else
        {
            IdleCheck();
        }
        if(Input.anyKey)
        {
            IdleTime = 0;
        }
        if(Input.GetKeyDown(KeyCode.Backslash))
        {
            IdleAnimator.gameObject.SetActive(!IdleAnimator.gameObject.activeSelf);
        }
        
    }
        
    public void IdleCheck(){
        IdleTime += Time.deltaTime;
    }
}
