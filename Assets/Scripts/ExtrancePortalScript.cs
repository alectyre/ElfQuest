using UnityEngine;
using System.Collections;

public class ExtrancePortalScript : MonoBehaviour {

	public string sceneToLoad;
	public float loadDelay;

	private bool playerHere;
	private GameObject player;

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
			playerHere = true;
			player = hit.gameObject;
		}
	}
	
	void OnTriggerExit2D(Collider2D hit) {
		
		
		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			playerHere = false;
		}
	}

	void Update ()
	{
		if(playerHere && Input.GetAxis("Attack") > 0)
		{
			if(player)
				player.GetComponent<PlayerMover>().lockManualControl = true;

			Invoke("LoadNextLevel", loadDelay);
		}
	}

	void LoadNextLevel ()
	{
		Application.LoadLevel(sceneToLoad);
	}
}
