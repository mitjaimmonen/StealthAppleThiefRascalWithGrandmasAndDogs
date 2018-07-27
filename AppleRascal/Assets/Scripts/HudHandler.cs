using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	// Use this for initialization
	public Text applesText;
	public Image applesIcon;
	Vector3 appleIconOriginalScale;
	Vector3 appleTextOriginalScale;
	int appleAmount;
	void Start () {
		GameMaster.Instance.hudHandler = this;
		appleIconOriginalScale = applesIcon.transform.localScale;
		appleTextOriginalScale = applesText.transform.localScale;
	}

	void Update()
	{
		if (appleIconOriginalScale != applesIcon.transform.localScale)
			applesIcon.transform.localScale = Vector3.Lerp(applesIcon.transform.localScale, appleIconOriginalScale, Time.deltaTime * 10f);
		if (appleTextOriginalScale != applesIcon.transform.localScale)
			applesText.transform.localScale = Vector3.Lerp(applesText.transform.localScale, appleTextOriginalScale, Time.deltaTime*10f);
		if (applesText.transform.eulerAngles != Vector3.zero)
			applesText.transform.rotation = Quaternion.Slerp(applesText.transform.rotation, Quaternion.identity, Time.deltaTime*10f);
	}
	
	public void SetApplesAmount(int amount)
	{
		if (amount > appleAmount)
		{
			applesIcon.transform.localScale += new Vector3(0.1f,0.1f,0.1f);
			applesText.transform.localScale += new Vector3(0.1f,0.1f,0.1f);
			var euler = applesText.transform.eulerAngles;
			euler.z -= 0.15f;
			applesText.transform.eulerAngles = euler;
		}
		
		appleAmount = amount;
		applesText.text = appleAmount.ToString();

	}
}
