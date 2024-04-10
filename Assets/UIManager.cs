using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StartPanel, PausePanel,LetterPanel, InGamePanel;
    public bool isPaused;
    public GameObject DeliveryText;

    private void OnEnable()
    {
        Audio_Distance_Controller.letterDelivered += LetterDeliveredToTrain;
    }

    private void OnDisable()
    {
        Audio_Distance_Controller.letterDelivered -= LetterDeliveredToTrain;
    }

    private void LetterDeliveredToTrain()
    {
        DeliveryText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused && Input.anyKeyDown)
        {
            ResumeGame();
        }
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Hello 2");
            PauseGame();
        }

 
    }

    public void StartGame()
    {
        StartPanel.gameObject.SetActive(false);
        LetterPanel.gameObject.SetActive(true);
        LetterPanel.GetComponent<LetterManager>().ShowLetter();
    }
    public void PauseGame()
    {
        isPaused = true;
        PausePanel.gameObject.SetActive(true);
    }
    public void ResumeGame()
    {
        PausePanel.gameObject.SetActive(false);
        isPaused = false;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
