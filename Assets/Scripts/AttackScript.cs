using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Instead of having the attack scripts call damage directly, have them generate
 * an object or struct that represents the AttackHit which gets passed to an
 * AttackResolver object to handle. This should give us a way to handle parries
 * nicely aaand hopefully not fuck up anything else.
 */

public class AttackScript : MonoBehaviour {

	public int damageValue;

	public int pierce = 1;
	private int piercesRemaining;
	private List<GameObject> hitsThisAttack;

	public bool destroyOnHit;


	void Awake()
	{
		hitsThisAttack = new List<GameObject>();

	}

	public void RefreshAttack ()
	{	
			piercesRemaining = pierce;
			hitsThisAttack.Clear();
	}

	public void OnTriggerEnter2D(Collider2D hit) { OnTriggerStay2D(hit); }

	public void OnTriggerStay2D(Collider2D hit) {

		if(piercesRemaining <= 0) {
			return;	
		}

		HealthScript target = hit.gameObject.GetComponent<HealthScript>();

		if(target && !hitsThisAttack.Contains(hit.gameObject))
		{
			target.InflictDamage(damageValue);
			hitsThisAttack.Add(hit.gameObject);
			piercesRemaining -= 1;

			if(destroyOnHit)
				gameObject.SetActive(false);
		}
	}
}
