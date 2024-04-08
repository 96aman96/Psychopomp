using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using NaughtyAttributes.Editor;
public class LetterManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Stamps;
    public TextMeshProUGUI content;
    public Animator anim;
    void Start()
    {
        
    }

    void randomStamp()
    {
        for (int i = 0; i < Stamps.childCount; i++)
        {
            Stamps.GetChild(i).gameObject.SetActive(false);
        }
        Stamps.GetChild(Random.Range(0,Stamps.transform.childCount)).gameObject.SetActive(true);
    }
    [Button("Show Letter")]
    public void ShowLetter()
    {
        randomStamp();
        anim.Play("Show Letter");

    }
    [Button("Hide Letter")]
    public void hideLetter()
    {
        anim.Play("Hide Letter");
    }
}
