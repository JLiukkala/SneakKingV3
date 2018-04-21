using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Invector
{
    public class RaiseLight : MonoBehaviour
    {
        GameObject lightMechanicObject;
        LightMechanic lightMechanicScript;

        void Start()
        {
            lightMechanicObject = GameObject.Find("LightMechanic");

            lightMechanicScript = lightMechanicObject.GetComponent<LightMechanic>();
        }

        void OnTriggerEnter(Collider other)
        {
            lightMechanicScript.isInTheLight = true;
        }

        void OnTriggerStay(Collider other)
        {
            if (lightMechanicScript.lightMeter < lightMechanicScript.maximumLight && lightMechanicScript.isInTheLight)
            {
                lightMechanicScript.lightMeter += Time.deltaTime;
            }

            if (lightMechanicScript.lightMeter >= lightMechanicScript.maximumLight && lightMechanicScript.isInTheLight)
            {
                lightMechanicScript.lightMeter = lightMechanicScript.maximumLight;
            }
        }

        void OnTriggerExit(Collider other)
        {
            lightMechanicScript.isInTheLight = false;
        }
    }
}
