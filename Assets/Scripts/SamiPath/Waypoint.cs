using UnityEngine;
using System.Collections;

namespace Invector.WaypointSystem
{
    // The waypoint system of the game utilises Sami's systems from the game created during the Game Programming 2 course.
    public class Waypoint : MonoBehaviour
	{
		public Vector3 Position
		{
			get { return transform.position; }
		}
	}
}
