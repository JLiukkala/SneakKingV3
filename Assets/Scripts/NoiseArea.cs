using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector
{
    public class NoiseArea : MonoBehaviour
    {
        #region Variables
        public GameObject Owner { get; protected set; }
        EnemyUnit enemy;

        [HideInInspector]
        public static bool heardNoise = false;

        GameObject sofa;
        NavMeshModifier nmm;

        GameObject navMeshGenerator;
        NavMeshGenerator nmg;
        #endregion

        void Start()
        {
            if (Owner == null)
            {
                Owner = GameObject.FindGameObjectWithTag("Enemy");
            }

            enemy = Owner.GetComponent<EnemyUnit>();

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
            if (enemy.roomHasNoiseArea)
            {
                if (enemy.isRoomTwo)
                {
                    nmm.ignoreFromBuild = true;
                    nmg.BuildNavMesh();
                }

                heardNoise = true;
            }
        }
    }
}
