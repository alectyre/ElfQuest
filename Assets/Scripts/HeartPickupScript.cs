using UnityEngine;
using System.Collections;

public class HeartPickupScript : MonoBehaviour {

	public float healthValue;
	
	public void OnTriggerEnter2D(Collider2D hit) { OnTriggerStay2D(hit); }
	
	public void OnTriggerStay2D(Collider2D hit) {
		
		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target && !target.HasMaxHealth)
		{
			target.Heal(healthValue);

			Destroy(gameObject);
		}
	}

	public void OnTriggerExit2D(Collider2D hit) {
		
		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target && !target.HasMaxHealth)
		{
			target.Heal(healthValue);
			
			Destroy(gameObject);
		}
	}
}
