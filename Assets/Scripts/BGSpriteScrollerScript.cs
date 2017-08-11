using UnityEngine;
using System.Collections;

public class BGSpriteScrollerScript : MonoBehaviour
{
	public GameObject BGSpriteOne;
	public GameObject BGSpriteTwo;

	private Transform transformOne;
	private Transform transformTwo;

	public int tilesPerObject;
	private float tileSize;

	private Transform cameraTransform;

	public float updateFrequency;
	private float timeUntilNextUpdate;

	private bool isOnTheRight;
		
	void Start ()
	{
		timeUntilNextUpdate = updateFrequency;

		transformOne = BGSpriteOne.transform;
		transformTwo = BGSpriteTwo.transform;
			
		tileSize = BGSpriteOne.GetComponent<SpriteRenderer>().bounds.size.x;

		cameraTransform = Camera.main.transform;
	}
	
	void Update ()
	{
		if(timeUntilNextUpdate > 0)
		{
			timeUntilNextUpdate -= Time.deltaTime;
		}
		else
		{
			timeUntilNextUpdate = updateFrequency;

			Transform closerToCamera;
			Transform furtherFromCamera;

			//Find which BGSprite is closer to the camera
			if(Mathf.Abs(cameraTransform.position.x - transformOne.position.x) < Mathf.Abs(cameraTransform.position.x - transformTwo.position.x))
			{
				closerToCamera = transformOne;
				furtherFromCamera = transformTwo;
			}
			else
			{
				closerToCamera = transformTwo;
				furtherFromCamera = transformOne;
			}

			//Check which side of the closer BGSprite the camera is on and move the further BGSprite to that side
			if(cameraTransform.position.x - closerToCamera.position.x > 0)//On the right
			{
				float x = tileSize;
				furtherFromCamera.position = new Vector3(closerToCamera.position.x + x,
				                                         closerToCamera.position.y,
				                                         closerToCamera.position.z);
				isOnTheRight = true;
			}
			else //On the left
			{
				float x = tileSize;
				furtherFromCamera.position = new Vector3(closerToCamera.position.x - x,
				                                         closerToCamera.position.y,
				                                         closerToCamera.position.z);
				isOnTheRight = false;
			}

			/*
			Debug.Log("Closer: " + closerToCamera.name + ", Further: " + furtherFromCamera.name
			          + "\nIs on the " + (isOnTheRight?"right":"left"));
			          */
		}
	}
}