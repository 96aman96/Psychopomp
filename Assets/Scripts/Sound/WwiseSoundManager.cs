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
    public AK.Wwise.Event soundwaveSFX;
    public AK.Wwise.Event random_featherSFX;
    public AK.Wwise.Event WindSoundEvent;
    public AK.Wwise.Event stopWindSoundEvent;

    private bool isFirstTimeGliding = true;
    
    public AK.Wwise.Event ui_start;
    public AK.Wwise.Event ui_quit;

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

    public void UI_Sound_Start()
    { 
        ui_start.Post(gameObject);
    }
    
    public void UI_Sound_Quit()
    { 
        ui_quit.Post(gameObject);
    }

    public void PlaySoundWave()
    {
        soundwaveSFX.Post(gameObject);
    }

    public void PlayRandomFeather()
    {
        random_featherSFX.Post(gameObject);
    }

    public void PlayWindSound()
    {
        WindSoundEvent.Post(gameObject);
    }
    public void StopWindSound()
    {
        stopWindSoundEvent.Post(gameObject);
    }
}
