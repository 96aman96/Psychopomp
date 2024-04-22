using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorderSelf : MonoBehaviour
{
    // Start is called before the first frame update
    public void SendToBack()
    {
        Debug.Log("called -----    " + transform.GetSiblingIndex() + "   " + transform.name);

        transform.SetSiblingIndex(0);
        Debug.Log("called " + transform.GetSiblingIndex() + "   " + transform.name);
    }

    public void BringToFront()
    {
        transform.SetSiblingIndex(transform.childCount-1);
    }
}
