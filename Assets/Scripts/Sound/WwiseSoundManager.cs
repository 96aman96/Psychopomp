using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class WwiseSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AK.Wwise.Event playTheme;
    public AK.Wwise.Event pauseTheme;
    public AK.Wwise.Event resumeTheme;
    public AK.Wwise.Event stopTheme;
    public AK.Wwise.Event playFlight;
    public AK.Wwise.Event pauseFlight;
    public AK.Wwise.Event resumeFlight;
    private bool isFirstTimeGliding = true;

    public void Start()
    {
        playTheme.Post(gameObject);
    }
    public void MusicStartGliding()
    {
        pauseTheme.Post(gameObject);
        if(isFirstTimeGliding)
        {
            playFlight.Post(gameObject);
        }
        else
        {
            resumeFlight.Post(gameObject);
        }
    }

    public void MusicStopGliding()
    { 
        resumeTheme.Post(gameObject);
        pauseFlight.Post(gameObject);
    }

}
