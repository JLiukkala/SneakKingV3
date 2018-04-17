using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class NoiseArea : MonoBehaviour
    {
        public GameObject Owner { get; protected set; }
        EnemyUnit enemy;
        AudioSource audio;

        void Start()
        {
            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            if (enemy.hasNoiseArea)
            {
                audio = GetComponent<AudioSource>();
            }
        }

        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (enemy.hasNoiseArea)
            {
                enemy.heardNoise = true;
                audio.Play();
                audio.Play(44100);
            }
        }
    }
}
