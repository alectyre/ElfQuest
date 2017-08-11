using UnityEngine;
using System.Collections;

public class TriggerEnablerScripter : MonoBehaviour {

	public GameObject[] objectsToEnable;

	void Awake ()
	{
		OnTriggerEnter2D(null);
	}

	void OnTriggerEnter2D(Collider2D hit) {

		PlayerHealthScript target = null;

		if(hit != null)
			target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			foreach(GameObject obj in objectsToEnable)
			{
				obj.SetActive(true);
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D hit) {

		
		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			foreach(GameObject obj in objectsToEnable)
			{
				obj.SetActive(false);
			}
		}
	}
}
