using UnityEngine;
using System.Collections;

public class OrcAxeAttackScript : MonoBehaviour {

	public int damage;

	public void OnTriggerEnter2D (Collider2D hit) {

		PlayerHealthScript player = hit.gameObject.GetComponent<PlayerHealthScript>();

		if(player != null)
			player.InflictDamage(gameObject, damage);
	}
}
