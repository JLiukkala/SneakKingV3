using UnityEngine;
using System.Collections;

namespace Invector.WaypointSystem
{
	public class Waypoint : MonoBehaviour
	{
		public Vector3 Position
		{
			get { return transform.position; }
		}
	}
}
