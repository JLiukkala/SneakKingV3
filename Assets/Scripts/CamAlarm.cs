using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class CamAlarm : MonoBehaviour
    {
        public GameObject Owner { get; protected set; }
        EnemyUnit enemy;

        void Start()
        {
            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();
        }

        void OnTriggerEnter(Collider other)
        {
            enemy.SetLastKnownPosition();
            enemy.inCameraView = true;
        }
    }
}
