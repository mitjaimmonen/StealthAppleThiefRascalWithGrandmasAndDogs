using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	// Use this for initialization
	public GameObject hudPanel;
	public Text applesText;
	public Image applesIcon;
	public Text buttonHintText;
	public Image buttonHintImage;
	public Slider leapCooldownSlider;
	Vector3 appleIconOriginalScale;
	Vector3 appleTextOriginalScale;
	int appleAmount;

	public Image controlsTipsPanel;
	public Image moveInstructionPanel;
	public Image crawlInstructionPanel;
	public Image actionInstructionPanel;


	void Start () {
		buttonHintImage.gameObject.SetActive(false);
		buttonHintText.gameObject.SetActive(false);
		
		appleIconOriginalScale = applesIcon.transform.localScale;
		appleTextOriginalScale = applesText.transform.localScale;
		applesText.text = 0.ToString();

	}

	void Update()
	{
			ShowTips();

		if (leapCooldownSlider.gameObject.activeSelf|| GameMaster.Instance.player.DashOnCooldown != 0)
		{
			leapCooldownSlider.gameObject.SetActive(true);
			leapCooldownSlider.value = GameMaster.Instance.player.DashOnCooldown;
			if (GameMaster.Instance.player.DashOnCooldown == 0)
				leapCooldownSlider.gameObject.SetActive(false);
		}

		if (appleIconOriginalScale != applesIcon.transform.localScale)
			applesIcon.transform.localScale = Vector3.Lerp(applesIcon.transform.localScale, appleIconOriginalScale, Time.deltaTime * 10f);
		if (appleTextOriginalScale != applesIcon.transform.localScale)
			applesText.transform.localScale = Vector3.Lerp(applesText.transform.localScale, appleTextOriginalScale, Time.deltaTime*10f);
		if (applesText.transform.eulerAngles != Vector3.zero)
			applesText.transform.rotation = Quaternion.Slerp(applesText.transform.rotation, Quaternion.identity, Time.deltaTime*10f);
	}

	void ShowTips()
	{
		
		if (GameMaster.Instance.player.hasMoved && moveInstructionPanel.gameObject.activeSelf)
		{
			var newPos = moveInstructionPanel.rectTransform.position;
			newPos.x = -210f;
			moveInstructionPanel.rectTransform.position = Vector3.Lerp(moveInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
			if (moveInstructionPanel.rectTransform.position.x < -200f)
				moveInstructionPanel.gameObject.SetActive(false);
		}
		else if (!GameMaster.Instance.player.hasMoved && moveInstructionPanel.rectTransform.position.x < 190f)
		{
			controlsTipsPanel.gameObject.SetActive(true);
			moveInstructionPanel.gameObject.SetActive(true);
			var newPos = moveInstructionPanel.rectTransform.position;
			newPos.x = 190f;
			moveInstructionPanel.rectTransform.position = Vector3.Lerp(moveInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
		}
		if (GameMaster.Instance.player.hasCrawled && crawlInstructionPanel.gameObject.activeSelf)
		{
			var newPos = crawlInstructionPanel.rectTransform.position;
			newPos.x = -210f;
			crawlInstructionPanel.rectTransform.position = Vector3.Lerp(crawlInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
			if (crawlInstructionPanel.rectTransform.position.x < -200f)
				crawlInstructionPanel.gameObject.SetActive(false);
		}
		else if (!GameMaster.Instance.player.hasCrawled && crawlInstructionPanel.rectTransform.position.x < 190f)
		{
			controlsTipsPanel.gameObject.SetActive(true);
			crawlInstructionPanel.gameObject.SetActive(true);
			var newPos = crawlInstructionPanel.rectTransform.position;
			newPos.x = 190f;
			crawlInstructionPanel.rectTransform.position = Vector3.Lerp(crawlInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
		}
		if (GameMaster.Instance.player.hasJumped && actionInstructionPanel.gameObject.activeSelf)
		{
			var newPos = actionInstructionPanel.rectTransform.position;
			newPos.x = -210f;
			actionInstructionPanel.rectTransform.position = Vector3.Lerp(actionInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
			if (actionInstructionPanel.rectTransform.position.x < -200f)
				actionInstructionPanel.gameObject.SetActive(false);
		}
		else if (!GameMaster.Instance.player.hasJumped && actionInstructionPanel.rectTransform.position.x < 190f)
		{
			controlsTipsPanel.gameObject.SetActive(true);
			actionInstructionPanel.gameObject.SetActive(true);
			var newPos = actionInstructionPanel.rectTransform.position;
			newPos.x = 190f;
			actionInstructionPanel.rectTransform.position = Vector3.Lerp(actionInstructionPanel.rectTransform.position, newPos, Time.deltaTime*5f);
		}
		
		if (!moveInstructionPanel.gameObject.activeSelf && !crawlInstructionPanel.gameObject.activeSelf && !actionInstructionPanel.gameObject.activeSelf)
		{
			controlsTipsPanel.gameObject.SetActive(false);
		}
	}

	public void ActivateHud()
	{
		hudPanel.SetActive(true);
	}
	public void DeactivateHud()
	{
		hudPanel.SetActive(false);
	}


	public void SetActionText(bool triggerState, string text)
	{
		if (triggerState)
		{
			buttonHintImage.gameObject.SetActive(true);
			buttonHintText.gameObject.SetActive(true);
			buttonHintText.text = text;
		}
		else
		{
			buttonHintImage.gameObject.SetActive(false);
			buttonHintText.gameObject.SetActive(false);
		}
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
