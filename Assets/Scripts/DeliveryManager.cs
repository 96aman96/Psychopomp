using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public Transform pickupPoint, DropPoint, RuinCollectible;
    public GameObject stashedItem;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Pickup":
                PickupItem(other.transform);
                break;
            case "Drop Zone":
                DropItem(other.transform);
                break;
        }
    }

    private void PickupItem(Transform item)
    {
        item.SetParent(this.transform);
        Destroy(item.GetComponent<Collider>());
        item.gameObject.SetActive(false);
        stashedItem = item.gameObject;
    }

    private void DropItem(Transform drop)
    {
        DropPoint = drop.transform;
        stashedItem.transform.SetParent(DropPoint);
        stashedItem.gameObject.SetActive(true);
    }
}
