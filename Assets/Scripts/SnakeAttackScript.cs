using UnityEngine;
using System.Collections;

public class SnakeAttackScript : MonoBehaviour {

	public float damageValue;

	public void OnTriggerEnter2D(Collider2D hit) { OnTriggerStay2D(hit); }

	public void OnTriggerStay2D(Collider2D hit) {

		PlayerHealthScript target = hit.gameObject.GetComponent<PlayerHealthScript>();
		
		if(target)
		{
			target.InflictDamage(gameObject, damageValue);
		}
	}
}
