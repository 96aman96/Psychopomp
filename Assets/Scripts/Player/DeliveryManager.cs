using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    public Transform pickupPoint;
    public LetterReader _letterReader;
    public GameObject LetterReaderPanel;
    public GameObject LetterDeliveryHandlerUI;
    public Image FillMeter;
    public bool DeliveredLetters;
    public float timer, fillVal;
    public RandomLetterSpawner _letterSpawner;
    public GameObject DeliveryText;
    public ParticleSystem pickupParticle;
    private WwiseSoundManager wwiseSoundManager;

    private void Start()
    {
        LetterDeliveryHandlerUI.gameObject.SetActive(false);
        DeliveredLetters = false;
        wwiseSoundManager = GameObject.FindObjectOfType<WwiseSoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Pickup":
                _letterReader.ShowLetter();
                _letterReader.AddLetter();
                pickupParticle.Play();
                wwiseSoundManager.CollectingBottles();
                _letterSpawner.SpawnOneInstance();
                DeliveredLetters = false;
                Destroy(other.gameObject);
                break;
            case  "Delivery Booster":
                if (_letterReader.collectedLetterHolder.transform.childCount > 0)
                {
                    LetterDeliveryHandlerUI.SetActive(true);
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Delivery Booster")
        {
            LetterDeliveryHandlerUI.gameObject.SetActive(false);
            FillMeter.fillAmount = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Delivery Booster")
        {
            if (_letterReader.collectedLetterHolder.transform.childCount > 0 && DeliveredLetters ==false)
            {
                FillMeter.fillAmount += Time.deltaTime/3;
                fillVal = FillMeter.fillAmount;
                if (FillMeter.fillAmount == 1)
                {
                    DeliveredLetters = true;
                }
            }

            if (DeliveredLetters == true)
            {
                DeliveredLetters = false;
                _letterReader.ClearAllLetters();
                DeliveryText.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _letterReader.showStackView();
        }
    }
}
