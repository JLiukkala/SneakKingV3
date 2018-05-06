using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector
{
    public class NavMeshGenerator : MonoBehaviour
    {
        public NavMeshSurface surface;

        void Start ()
        {
            BuildNavMesh();
        }

        public void BuildNavMesh ()
        {
            surface.BuildNavMesh();
        }
    }
}
