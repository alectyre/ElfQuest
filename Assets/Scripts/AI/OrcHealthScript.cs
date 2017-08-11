using UnityEngine;
using System.Collections;

public class OrcHealthScript : MonoBehaviour {
	
	public int currentHealth;
	public int maxHealth;

	private OrcBehaviorControllerScript orcBehaviorController;
	
	//private bool isDead = false;

	private void Awake () {
		orcBehaviorController = transform.root.GetComponentInChildren<OrcBehaviorControllerScript>();
	}
	
	public void InflictDamage(int amount) {

		currentHealth -= amount;
		
		if(currentHealth <= 0)
			KillOrc();
		else
			if(orcBehaviorController != null)
				orcBehaviorController.SetState(OrcBehaviorScript.State.KnockBack);
	}
	
	private void KillOrc () {
		gameObject.SetActive(false);
	}
}
