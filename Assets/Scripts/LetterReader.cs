using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class LetterReader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ReadableLetterPrefab;
    public Transform collectedLetterHolder,topLetter, bottomLetter;
    public GameObject FallBackText;
    public Animator anim;
    [TextArea]
    public string[] LetterContents;
    public int assignmentCounter;
    
    
    [Button("Show Letter")]
    public void ShowLetter()
    {
        
        anim.Play("Show Letter");

    }
    [Button("Hide Letter")]
    public void hideLetter()
    {
        anim.Play("Hide Letter");
    }
    private void Update()
    {
        if (transform.childCount == 0)
        {
            FallBackText.gameObject.SetActive(true);
        }
        else
        {
            FallBackText.gameObject.SetActive(false);
        }
        if (collectedLetterHolder.transform.childCount !=0)
        {
            GetTopAndBottom();
            if (Input.GetKeyDown(KeyCode.A))
            {
                ReadPreviousLetter();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ReadNextLetter();
            }
        }
    }
    [NaughtyAttributes.Button("Add Letter")]
    public void AddLetter()
    {
        float randRot = Random.Range(-10, 10);
        GameObject Collectible = GameObject.Instantiate(ReadableLetterPrefab);
        Collectible.transform.SetParent(this.collectedLetterHolder);
        Collectible.transform.localPosition = Vector3.zero;
        Collectible.transform.localRotation = Quaternion.Euler(0,0,randRot);
        Collectible.transform.SetSiblingIndex(collectedLetterHolder.childCount-1);
        Collectible.name = "Letter " + assignmentCounter;
        Collectible.GetComponent<Letter>().Content = LetterContents[assignmentCounter];
        assignmentCounter++;
    }
    [NaughtyAttributes.Button("Prev Letter")]
    public void ReadPreviousLetter()
    {
        bottomLetter.GetComponent<Animator>().Play("Come To Front");
    }

    public void GetTopAndBottom()
    {
        topLetter = collectedLetterHolder.GetChild(collectedLetterHolder.childCount - 1);
        bottomLetter = collectedLetterHolder.GetChild(0);

    }
    [NaughtyAttributes.Button("Next Letter")]
    public void ReadNextLetter()
    {
        topLetter.GetComponent<Animator>().Play("Go Back");

    }
}
