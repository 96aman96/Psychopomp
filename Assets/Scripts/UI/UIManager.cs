using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines.Primitives;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StartPanel, PausePanel, LetterPanel, InGamePanel;
    public GameObject DeliveryText;
    private GameManager gm;
    public UnityEvent PauseGame, ResumeGame;
    
    public LetterReader letterUI;

    public bool isPausedGame = false;
    private bool isBagOpen = false;

    void Start(){
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart(){
        yield return new WaitForSeconds(0.05f);
        Pause();
    }

    private void LetterDeliveredToTrain(){
        if(InGamePanel!=null)
        {
        InGamePanel.gameObject.SetActive(true);
        DeliveryText.gameObject.SetActive(true);
        Destroy(DeliveryText,4);
        Destroy(InGamePanel,4);
        }

    }
 
    // Update is called once per frame
    void Update(){
        if (Input.GetButtonDown("Pause")){
            Pause();
        }

        if(Input.GetButtonDown("Inventory")){
            Inventory();
        }

        if(letterUI.SingleLetterShown && (Input.GetButtonDown("UI Accept") || Input.GetButtonDown("Inventory") || Input.GetButtonDown("Pause"))){
            DismissLetter();
        }
    }

    private void Pause(){
        if(isBagOpen || letterUI.SingleLetterShown) return;
        
        if (isPausedGame){
            isPausedGame = false;
            ResumeGame.Invoke();
            PausePanel.SetActive(false);

            // Time.timeScale = 1;
        } else {
            isPausedGame = true;
            PauseGame.Invoke();
            PausePanel.SetActive(true);

            // Time.timeScale = 0;
        }
    }

    private void Inventory(){
        if(isPausedGame || letterUI.SingleLetterShown) return;

        if(isBagOpen){
            isBagOpen = false;
            ResumeGame.Invoke();
            letterUI.CloseBag();

            // Time.timeScale = 1;
            
        } else {
            isBagOpen = true;
            PauseGame.Invoke();
            letterUI.OpenBag();

            // Time.timeScale = 0;
        }
    }

    public void InvokePause(){
        PauseGame.Invoke();
    }

    public void InvokeUnpause(){
        ResumeGame.Invoke();
    }

    private void DismissLetter(){
        letterUI.DismissLetter();
    }
}
