using UnityEngine;
using System.Collections;

public class WaterScript : MonoBehaviour {

	static GameObject splashPoolObject;
	static private int numberOfSplashes = 3;

	public GameObject splashPrefab;
	public string behindWaterLayer;
	public TripScript tripScript;

	Transform[] splashPool;

	void Awake ()
	{
		if(WaterScript.splashPoolObject == null)
		{
			GameObject newSplashPool = new GameObject();
			newSplashPool.transform.name = "SplashPool";
			WaterScript.splashPoolObject = newSplashPool;

			for(int i = 0; i < WaterScript.numberOfSplashes; i++)
			{
				GameObject newSplash = (GameObject)Instantiate(splashPrefab);
				newSplash.transform.name = "Splash";
				newSplash.transform.parent = newSplashPool.transform;
			}
		}

		splashPool = GameObject.Find("SplashPool").transform.GetComponentsInChildren<Transform>();
	}

	//public void OnTriggerStay2D(Collider2D hit) { OnTriggerEnter2D(hit); }

	public void OnTriggerEnter2D(Collider2D hit) {


		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			target.GetComponent<SpriteRenderer> ().sortingLayerName = behindWaterLayer;
			target.GetComponent<PlayerMover>().dashTrail.drawTrail = false;
			tripScript.tripDashTrail.enabled = false;

			float surfaceHeight = GetComponent<SpriteRenderer>().bounds.size.y/2 + transform.position.y;

			float spreadIncrement = 0.1f;
			float spreadLeft = 0;
			float spreadRight = 0;

			for(int i = 0; i < splashPool.Length; i++)
			{
				float x = hit.gameObject.transform.position.x;

				if( i != 0 ) 
				{
					if( i%2 == 0 ) //On even number, increase rightIncrement
					{
						spreadRight += spreadIncrement;
						x += spreadRight;
					}
					else //On odd number, decrease leftIncrement
					{
						spreadLeft -= spreadIncrement;
						x += spreadLeft;
					}
				}
								
				Vector3 splashPosition = new Vector3(x, surfaceHeight, -2f);

				splashPool[i].position = splashPosition;

				//Debug.Log(leftIncrement + " " + rightIncrement);

				ParticleSystem[] particleSystems = splashPool[i].gameObject.GetComponentsInChildren<ParticleSystem>();

				foreach(ParticleSystem particleSystem in particleSystems)
					particleSystem.Play();
			}

			target.DrownPlayer();
		}
	}
}
