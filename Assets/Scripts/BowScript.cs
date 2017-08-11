using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//[RequireComponent (typeof (GameObjectPool))]
public class BowScript : MonoBehaviour {


	//public bool bowHeld;
	public bool fireWhenReady;

	private Animator _animator;

	//public GameObject arrowPrefab;

	public Vector3 arrowOrigin;

	private GameObjectPool arrowPool;
	
	void Awake () {
		_animator = GetComponent<Animator>();

		arrowPool = GetComponent<GameObjectPool>();
	}

	void Update () {
		if(Input.GetMouseButtonDown(1) && !_animator.GetBool("bowDrawn"))
		{
			//Draw bow
			_animator.Play( "alucard_bow" );
			_animator.SetBool("bowHeld", true);
			fireWhenReady = false;
			_animator.SetBool("bowDrawn", true);
		}
		else if(!Input.GetMouseButton(1) && _animator.GetBool("bowHeld"))
		{
			_animator.SetBool("bowHeld", false);
		}

		if(!_animator.GetBool("bowHeld") && fireWhenReady && _animator.GetBool("bowDrawn"))
		{
			fireWhenReady = false;
			_animator.SetBool("bowHeld", false);

			FireArrow();
		}
	}

	public void EndBowAttack() {
		_animator.SetBool("bowDrawn", false);
	}

	public void TriggerBow() {
		fireWhenReady = true;
	}

	void FireArrow()
	{
		GameObject arrowToFire = arrowPool.GetObjectFromPool();

		if(!arrowToFire)
			return;

		arrowToFire.SetActive(true);
		arrowToFire.GetComponent<ProjectileScript>().velocity *= transform.root.localScale.x; //Invert velocity if facing left
		
		arrowToFire.transform.position = new Vector3(transform.position.x + (arrowOrigin.x * transform.root.localScale.x), //Invert origin offset if facing left
		                                               transform.position.y + arrowOrigin.y, 
		                                               transform.position.z + arrowOrigin.z);
		
		arrowToFire.transform.localScale = new Vector3(transform.root.localScale.x * -arrowToFire.transform.localScale.x,
		                                                 arrowToFire.transform.localScale.y,
		                                                 arrowToFire.transform.localScale.z);
	}
}
