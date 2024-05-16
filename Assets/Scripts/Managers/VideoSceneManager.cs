using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoSceneManager : MonoBehaviour{
    void Update(){
        if(Input.anyKey){
            AkSoundEngine.StopAll();
            SceneManager.LoadScene("29.4 Main");
        }
    }
}
