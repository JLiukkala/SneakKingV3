using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;


public class VignetteController : MonoBehaviour {

    public PostProcessingProfile _sceneProfile;

    //[Range(0, 1)]
    public static float _vignetteVolume;

    [FMODUnity.EventRef]
    public string HeartBeatEvent;

    FMOD.Studio.EventInstance _heatbeatEvent;


    private FMOD.Studio.PLAYBACK_STATE musicPlaybackState;

    // Use this for initialization
    void Start() {
        _vignetteVolume = 0;
        _heatbeatEvent = FMODUnity.RuntimeManager.CreateInstance(HeartBeatEvent);
        _heatbeatEvent.getPlaybackState(out musicPlaybackState);
    }


    private void Update()
    {
        
        var vignette = _sceneProfile.vignette.settings;
        //vignette.intensity= _vignetteVolume;
        _vignetteVolume += 0.025f*Time.deltaTime;
        vignette.intensity = _vignetteVolume;
        _sceneProfile.vignette.settings = vignette;

        _heatbeatEvent.getPlaybackState(out musicPlaybackState);
        if (_vignetteVolume > 0.2f)
        {
            Debug.Log(_vignetteVolume);

            //_heatbeatEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            


            if (musicPlaybackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                
                _heatbeatEvent.start();
            }
            

        }
        else if (_vignetteVolume < 0.25f)
        {
                //_heatbeatEvent.release();
                //Debug.Log("lol");

        }
        Debug.Log(musicPlaybackState);
    }
}

    



