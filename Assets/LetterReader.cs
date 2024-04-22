using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterReader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ReadableLetterPrefab;
    public Transform topLetter, bottomLetter;
    public GameObject FallBackText;
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
        if (transform.childCount > 2)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                bottomLetter = transform.GetChild(0);
                ReadPreviousLetter();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                topLetter = transform.GetChild(transform.childCount - 1);
                ReadNextLetter();
            }
        }

       
    }

    public void AddNewLetterToStack()
    {
        GameObject CollectibleUIStackReadable = GameObject.Instantiate(ReadableLetterPrefab);
        
    }
    public void ReadPreviousLetter()
    {
       bottomLetter.GetComponent<Animator>().Play("Come To Front");
    }
    public void ReadNextLetter()
    {
        topLetter.GetComponent<Animator>().Play("Go Back");

    }
}
