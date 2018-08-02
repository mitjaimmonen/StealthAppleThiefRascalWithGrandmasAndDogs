using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LerpPosition : MonoBehaviour {

    private Vector3 relativePosition;
    private Transform target;
    private Transform tr;
    public float speed;
    private NavMeshAgent navMeshAgent;

	void Start () {
        tr = transform;
        relativePosition = tr.localPosition;
        target = tr.parent;
        navMeshAgent = target.GetComponent<NavMeshAgent>();
        tr.parent = null;

	}
	

	void LateUpdate () {
        speed = navMeshAgent.speed + 1;
        Vector3 destPos = target.TransformPoint(relativePosition);
        tr.position = Vector3.Lerp(tr.position, destPos, speed * Time.deltaTime);

	}
}
