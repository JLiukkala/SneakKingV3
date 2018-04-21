using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Invector
{
    public class LightMechanic : MonoBehaviour
    {
        //[HideInInspector]
        public float lightMeter = 3;

        public float maximumLight;

        public RawImage lightDark;

        [HideInInspector]
        public bool isInTheLight = false;

        void Update()
        {
            if (!isInTheLight)
            {
                lightMeter -= Time.deltaTime;
            }

            if (lightMeter <= 0)
            {
                //lightDark.color = new Color(0, 0, 0, 1);
                //Debug.Log("Game Over!");
                GameUI._losingStatement = true;
            }
        }
    }
}
