using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointSystem
{
    public class Waypoint : MonoBehaviour
    {
        public Vector3 Position
        {
            get { return transform.position; }
        }
    }
}

