using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTree : MonoBehaviour {

	public GameObject applePrefab;
	public Transform[] appleSpawns = new Transform[5];
	public int maxApplesInTree;
	public int minApplesInTree;
	public float shakeInterval;
	public int minAmountPerShake;
	public int maxAmountPerShake;

	List<Apple> apples = new List<Apple>();
	float lastShakeTime;
	int applesLeft;
	Player player;
	GameObject parent;
	void Start()
	{
		parent = new GameObject();
		parent.name = "Apples Parent";
		parent.transform.parent = this.transform;
		int amount = Random.Range(minApplesInTree,maxApplesInTree+1);
		for (int i = 0; i < amount ; i++)
		{
			apples.Add(Instantiate(applePrefab, transform.position, transform.rotation).GetComponent<Apple>());
			apples[i].gameObject.SetActive(false);
			apples[i].transform.parent = parent.transform;
		}
		applesLeft = apples.Count;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.collectingHandler.AllowShake = true;
			player.collectingHandler.appleTree = this;
			//Allow shakeshake
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			//Disallow shakeshake
			player = other.GetComponent<Player>();
			player.collectingHandler.AllowShake = false;
		}
	}



	public void ShakeTree()
	{
		if (applesLeft > 0 && lastShakeTime + shakeInterval < Time.time)
		{
			lastShakeTime = Time.time;
			player.soundSource.NewTreeShakeSound();
			
			int rand = Random.Range(minAmountPerShake, maxAmountPerShake+1);
			rand = Mathf.Min(applesLeft, rand);

			for (int i = 0; i < rand; i++)
			{
				Vector3 randOffset = new Vector3(Random.Range(-0.5f,0.5f) , Random.Range(-0.5f,0.5f) , Random.Range(-0.5f,0.5f));
				Vector3 spawnPos = appleSpawns[Random.Range(0, appleSpawns.Length)].transform.position + randOffset;
				apples[applesLeft-1].gameObject.SetActive(true);
				apples[applesLeft-1].transform.position = spawnPos;
				apples[applesLeft-1].player = player;
				applesLeft--;
			}

			if (applesLeft <= 0)
				ShowEmptyTreeVisual();
		}
		
	}

	void ShowEmptyTreeVisual()
	{
		Debug.Log("Empty");
	}
	
}
