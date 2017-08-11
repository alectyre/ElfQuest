using UnityEngine;
using System.Collections;

public class HearingTesterScript : MonoBehaviour {

	public bool createSound;
	public bool debug;
	public Camera cameraToUse;
	private Plane plane = new Plane(Vector3.forward, Vector3.zero);

	void Awake () {
		HearingScript.debug = debug;

		HearingScript.registerListener(new HearingScript.Listener(gameObject, DidHearSound));
	}

	private void DidHearSound() {
		Debug.Log(transform.name + " heard that.");
	}

	void Update () {
		if(!createSound)
			return;

		//Update _targetPosition to mouse position on _plane
		if(Input.GetMouseButtonDown(0)) {

			if(!cameraToUse){
				Debug.Log("SoundTesterScript: cameraToUse not set!");
				return;
			}

			Ray ray = cameraToUse.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			float hitdist = 0.0f;
			
			Vector3 targetPosition;
			
			if(plane.Raycast(ray, out hitdist)) {
				targetPosition = ray.GetPoint(hitdist);

				HearingScript.createNoise(targetPosition, 4);
			}
		}
	}
}
