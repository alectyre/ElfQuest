using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	public float velocity;
	public float gravity;
	public float lifeSpan;

	public LayerMask collidesWith;

	private BoxCollider2D _collider;
	//private LayerMask

	void Awake () {
		_collider = GetComponent<BoxCollider2D>();

		if(lifeSpan <= 0)
			Debug.Log("Projectile lifespan is zero");
	}

	void OnEnable () {
		Invoke("Destroy", lifeSpan);
	}

	void OnDisable () {
		CancelInvoke();
	}

	void FixedUpdate() {

		//Apply forward velocity
		transform.position = transform.position + (transform.right * velocity);

		//Apply gravity
		transform.position = transform.position + (-transform.up * gravity);


		if(CheckForCollision())
		{
				Destroy();
		}
	}

	void Destroy() {
		gameObject.SetActive(false);
	}



	bool CheckForCollision() {
		float originDistance = _collider.size.x/2 - 0.2f;

		float rayCastLength = velocity;

		Vector3 origin = transform.position + (transform.right * originDistance * -transform.localScale.x);
		Vector3 ray = transform.right * rayCastLength  * -transform.localScale.x;

		Debug.DrawRay(origin, ray, Color.red, 0.0001f);

		RaycastHit2D hit = Physics2D.Raycast(origin, transform.right * -transform.localScale.x, rayCastLength, collidesWith.value);

		if(hit) //Hit something in collidesWith
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
