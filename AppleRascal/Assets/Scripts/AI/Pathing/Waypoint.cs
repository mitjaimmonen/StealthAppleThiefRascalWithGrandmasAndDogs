using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointSystem
{
    public class Waypoint : MonoBehaviour
    {
        public bool overrideSentry;
        public float sentryModeDuration;
        public float sentryModeLookAngle;

        public Vector3 Position
        {
            get { return transform.position; }
        }
    }
}

