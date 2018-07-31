using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingHandler : MonoBehaviour {

	PostProcessLayer postProcessLayer;
	PostProcessVolume postProcessVolume;
	PostProcessProfile profile;
	float defaultDof;
	DepthOfField dof;

	// Use this for initialization
	void Start () {
		postProcessVolume = GetComponentInChildren<PostProcessVolume>();
		postProcessLayer = GetComponent<PostProcessLayer>();
		profile = postProcessVolume.profile;
		postProcessVolume.profile.TryGetSettings(out dof);
		defaultDof = dof.focusDistance.value;

	}
	public void SetDepthOfField (float dofOffset)
	{
		if (profile)
		{
			dofOffset = Mathf.Clamp((dofOffset+defaultDof),0,20f);
			profile.TryGetSettings(out dof);
			dof.focusDistance.value = dofOffset;
			Debug.Log(dof.focusDistance);

		}
		
	}
}
