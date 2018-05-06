using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using Invector.CharacterController;


public class VignetteController : MonoBehaviour {

    public PostProcessingProfile _sceneProfile;

    public static float _vignetteVolume;


    public Animator _fader;
    public Transform _spawnPoint;

    private bool fadeOut = false;

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


    IEnumerator GotCaught()
    {
        
        GameObject _player = vThirdPersonCamera.instance.target.gameObject;
        _player.GetComponent<vThirdPersonMotor>().lockMovement = true;
        _fader.SetBool("Fade", true);
        fadeOut = true;
        yield return new WaitForSeconds(1.25f);
        if (_spawnPoint != null)
        {
            _player.GetComponent<Animator>().SetBool("GotCaught", true);
            
        }
        yield return new WaitForSeconds(1f);
        _player.transform.position = _spawnPoint.position;
        _vignetteVolume = 0;
        _player.GetComponent<Animator>().SetBool("GotCaught", false);
        _fader.SetBool("Fade", false);
        fadeOut = false;

        yield return new WaitForSeconds(3.5f);
        _player.GetComponent<vThirdPersonMotor>().lockMovement = false;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space)) { _vignetteVolume = 1.4f; }
        var vignette = _sceneProfile.vignette.settings;
        _vignetteVolume += 0.025f*Time.deltaTime;
        vignette.intensity = _vignetteVolume;
        _sceneProfile.vignette.settings = vignette;

        _heatbeatEvent.getPlaybackState(out musicPlaybackState);

        if (_vignetteVolume > 1.5f)
        {

            if (!fadeOut) { 
            StartCoroutine(GotCaught());
            }
        }
        else if (_vignetteVolume > 0.2f)
        {
            //Debug.Log(_vignetteVolume);

            //_heatbeatEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            


            if (musicPlaybackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                
                _heatbeatEvent.start();
            }
            

        }
        else if (_vignetteVolume < 0.25f)
        {
                _heatbeatEvent.release();
        } 


        //Debug.Log(musicPlaybackState);
    }
}

    



