using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StartPanel, PausePanel,LetterPanel, InGamePanel;
    public GameObject DeliveryText;
    private GameManager gm;
    public UnityEvent PauseGame, ResumeGame;
    public bool isPausedGame;
    private void LetterDeliveredToTrain()
    {
        if(InGamePanel!=null)
        {
        InGamePanel.gameObject.SetActive(true);
        DeliveryText.gameObject.SetActive(true);
        Destroy(DeliveryText,4);
        Destroy(InGamePanel,4);
        }

    }
 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPausedGame)
            {
                isPausedGame = false;
                ResumeGame.Invoke();
            }
            else
            {
                isPausedGame = true;
                PauseGame.Invoke();
            }
        }
    }
}
