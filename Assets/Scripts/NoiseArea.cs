using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector
{
    public class NoiseArea : MonoBehaviour
    {
        public GameObject Owner { get; protected set; }
        EnemyUnit enemy;
        //AudioSource audio;

        GameObject sofa;
        NavMeshModifier nmm;

        GameObject navMeshGenerator;
        NavMeshGenerator nmg;

        void Start()
        {
            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();

            //if (enemy.hasNoiseArea)
            //{
            //    audio = GetComponent<AudioSource>();
            //}

            if (enemy.isRoomTwo)
            {
                sofa = GameObject.Find("Sofa");
                navMeshGenerator = GameObject.Find("NavMeshGenerator");

                nmm = sofa.GetComponent<NavMeshModifier>();
                nmg = navMeshGenerator.GetComponent<NavMeshGenerator>();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (enemy.hasNoiseArea)
            {
                if (enemy.isRoomTwo)
                {
                    nmm.ignoreFromBuild = true;
                    nmg.BuildNavMesh();
                }

                enemy.heardNoise = true;
                //audio.Play();
                //audio.Play(44100);
            }
        }
    }
}
