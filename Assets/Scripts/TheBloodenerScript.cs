using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheBloodenerScript : MonoBehaviour {

	public int poolSize;
	public GameObject bloodPrefab;

	private List<GameObject> bloodPool;

	public bool isSingletonInstance;

	private static TheBloodenerScript singletonInstance;

	void Start () {
		bloodPool = new List<GameObject>();

		GameObject bloodPoolObject = new GameObject();
		bloodPoolObject.transform.name = "BloodPool";

		for(int i = 0; i < poolSize; i++)
		{
			GameObject newBlood = (GameObject)Instantiate(bloodPrefab);

			newBlood.transform.name = bloodPrefab.transform.name;
			newBlood.transform.parent = bloodPoolObject.transform;

			bloodPool.Add(newBlood);
		}

		if(isSingletonInstance)
			TheBloodenerScript.singletonInstance = this;
	}

	static public void MakeBlood (Vector3 position)
	{
		if(singletonInstance != null)
			TheBloodenerScript.singletonInstance.MakeSomeBlood(position);
		else
			Debug.Log("Static MakeBlood called, but no Bloodener set to instance!");
	}

	public void MakeSomeBlood (Vector3 position)
	{
		ParticleSystem[] particleSystems = null;
		GameObject blood = null;
		position.z = bloodPrefab.transform.position.z;

		for(int i = 0; i < bloodPool.Count; i++)
		{
			blood = bloodPool[i];

			particleSystems = blood.GetComponentsInChildren<ParticleSystem>();

			bool isPlaying = false;

			foreach(ParticleSystem particleSystem in particleSystems)
			{
				if(particleSystem.isPlaying)
					isPlaying = true;
			}

			if(!isPlaying)
			{
				break;
			}

			blood = null;
		}
		
		if(blood != null)
		{			
			blood.transform.position = position;

			foreach(ParticleSystem particleSystem in particleSystems)
			{
				particleSystem.Play();
			}
		}
	}
}
