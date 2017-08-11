using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour {

	private Animator _animator;
	private PlayerMover player;
	private bool attackReleased;

	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator>();
		player = transform.root.GetComponent<PlayerMover>();
	}
	
	// Update is called once per frame
	void Update () {
		if( !attackReleased && Input.GetAxis("Attack") <= 0)
		{
			attackReleased = true;
		}

		if( Input.GetAxis("Attack") > 0 && attackReleased && !player.lockManualControl )
		{
			_animator.Play ("weapon_slash");
			attackReleased = false;
		}
	}
}
