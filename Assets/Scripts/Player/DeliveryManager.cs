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
        Debug.Log("HERE 11111");
        switch (other.tag)
        {
            case "Pickup":
                Debug.Log("HERE 2");
                PickupItem(other.transform);
                break;
            case "Drop Zone":
                
                break;
        }
    }

    private void PickupItem(Transform item)
    {
        Destroy(item.gameObject);
        _letterReader.AddLetter();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Nfdvve");
            LetterReaderPanel.gameObject.SetActive(!LetterReaderPanel.gameObject.activeSelf);
        }
    }
}
