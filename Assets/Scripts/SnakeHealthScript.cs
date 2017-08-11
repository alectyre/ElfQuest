using UnityEngine;
using System.Collections;

public class SnakeHealthScript : HealthScript {

	private BoxCollider2D _collider;
	
	private void Awake ()
	{
		_collider = GetComponent<BoxCollider2D>();
	}

	public override void InflictDamage(float value)
	{
		currentHealth -= value;

		Vector3 bloodPos = transform.position;
		bloodPos.y -= 0.3f;
		TheBloodenerScript.MakeBlood(bloodPos);

		if(currentHealth <= 0) {

			Kill();
		}
	}
	
	public override void Kill() 
	{ 
		_collider.enabled = false;

		Vector3 bloodPos = transform.position;
		bloodPos.y -= 0.3f;
		TheBloodenerScript.MakeBlood(bloodPos);
		TheBloodenerScript.MakeBlood(bloodPos);
		TheBloodenerScript.MakeBlood(bloodPos);

		Destroy(gameObject);
	}
}
