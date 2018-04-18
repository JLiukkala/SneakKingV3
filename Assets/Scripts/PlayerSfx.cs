using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSfx : MonoBehaviour {


    [FMODUnity.EventRef]
    public string FootstepEvent;
    
    // Use this for initialization
    public float m_Wood;
    public float m_Grass;


    void Awake () {
       

        //Defaults
        m_Grass = 0.0f;
        m_Wood = 1.0f;

    }
	

    void Step()
    {
        FMOD.Studio.EventInstance _stepEvent = FMODUnity.RuntimeManager.CreateInstance(FootstepEvent);
        SetParameter(_stepEvent, "Wood", m_Wood);
        //SetParameter(_stepEvent, "Grass", m_Grass);


        if (FootstepEvent != null)
        {
            
            _stepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            

            _stepEvent.start();
            _stepEvent.release();
        }
    }

    void SetParameter(FMOD.Studio.EventInstance _stepEvent, string name, float value)
    {
        FMOD.Studio.ParameterInstance parameter;
        _stepEvent.getParameter(name, out parameter);
        parameter.setValue(value);
    }

}

