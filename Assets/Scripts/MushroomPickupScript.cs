using UnityEngine;
using System.Collections;

public class MushroomPickupScript : MonoBehaviour {

	public TripScript tripper;
	public float duration;

	public SpriteRenderer[] changeColors;

	private ParticleSystem aura;

	void Awake ()
	{
		aura = GetComponentInChildren<ParticleSystem>();
	}
	
	public void OnTriggerEnter2D(Collider2D hit) { OnTriggerStay2D(hit); }
	
	public void OnTriggerStay2D(Collider2D hit) {
		
		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			GetComponent<Collider2D>().enabled = false;
			GetComponent<Renderer>().enabled = false;	

			tripper.StartTripping();

			if(aura)
				aura.emissionRate = 0;

			Invoke("EndTrip", duration);
		}
	}

	public void EndTrip()
	{
		tripper.StopTripping();
	}
}
