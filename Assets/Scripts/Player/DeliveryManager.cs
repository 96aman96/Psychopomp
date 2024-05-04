using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public Transform pickupPoint;
    public LetterReader _letterReader;
    public GameObject LetterReaderPanel;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Pickup":
                _letterReader.ShowLetter();
                _letterReader.AddLetter();
                Destroy(other.gameObject);
                break;

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
