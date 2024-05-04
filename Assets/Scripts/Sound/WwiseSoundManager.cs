using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class WwiseSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AK.Wwise.Event playTheme;
    public AK.Wwise.Event pauseTheme;
    public AK.Wwise.Event resumeTheme;
    public AK.Wwise.Event stopTheme;
    public AK.Wwise.Event soundwaveSFX;
    public AK.Wwise.Event random_featherSFX;
    public AK.Wwise.Event WindSoundEvent;
    public AK.Wwise.RTPC windPitchRTPC;
    public AK.Wwise.RTPC windLowPassRTPC;
    public AK.Wwise.RTPC windHighPassRTPC;
    public AK.Wwise.Event itemSFX;
    public AK.Wwise.Event playAmbient;
    
    public AK.Wwise.Event stopWindSoundEvent;
    public AK.Wwise.Event playSplash;
    public AK.Wwise.Event stopSplash;
    public AK.Wwise.Event playSplashFast;
    public AK.Wwise.Event stopSplashFast;
    public AK.Wwise.Event playOpenLetterSFX;
    public AK.Wwise.Event playSwitchLetterSFX;    


    public AK.Wwise.Event playFly;
    public AK.Wwise.Event pauseFly;
    public AK.Wwise.Event resumeFly;

    public AK.Wwise.Event buildingSFX;
    

    private bool isFirstTimeGliding = true;
    
    
    public AK.Wwise.Event ui_start;
    public AK.Wwise.Event ui_quit;

    public void Start()
    {
        playTheme.Post(gameObject);
        PlayAmbient();
    }
    public void MusicStartGliding()
    {
        pauseTheme.Post(gameObject);
        playFly.Post(gameObject);
    }

    public void MusicStopGliding()
    { 
        resumeTheme.Post(gameObject);
        pauseFly.Post(gameObject);
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

    public void PlayWindSound(float pitchValue, float lowPassValue, float highPassValue)
    {
        //you can use this pitch value to set up the pitch of the wind
        //(0, 100)
        windPitchRTPC.SetGlobalValue(pitchValue); 
        windLowPassRTPC.SetGlobalValue(lowPassValue); 
        windHighPassRTPC.SetGlobalValue(highPassValue); 
        WindSoundEvent.Post(gameObject);
    }
    public void StopWindSound()
    {
        stopWindSoundEvent.Post(gameObject);
    }

    public void ItemSound()//item pick up sfx
    {
        itemSFX.Post(gameObject);
    }

    public void PlayAmbient()//background white noise
    {
        playAmbient.Post(gameObject);
    }

    public void PlaySplash()
    {
        playSplash.Post(gameObject);
    }

    public void StopSplash()
    {
        stopSplash.Post(gameObject);
    }

    public void PlaySplashFast()
    {
        playSplashFast.Post(gameObject);
    }

    
    public void StopSplashFast()
    {
        stopSplashFast.Post(gameObject);
    }

    public void playOpenLetter()
    {
        playOpenLetterSFX.Post(gameObject);
    }

    public void playSwitchLetter()
    {
        playSwitchLetterSFX.Post(gameObject);
    }

    public void ResumeFlyMusic()
    {
        resumeFly.Post(gameObject);
    }

    public void PlayBuilding()
    {
        buildingSFX.Post(gameObject);
    }
    
}
