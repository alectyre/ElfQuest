using UnityEngine;
using System.Collections;

public class EnemyControllerScript : MonoBehaviour {

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	public Transform[] reversibles;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;
	
	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	
	private BoxCollider2D _collider;

	private bool moveRight;
	private bool moveLeft;
	private bool shortJump;
	private bool changeFacing;
	private float speedMod;

	public bool lockControl;

	void Awake()
	{
		_controller = GetComponent<CharacterController2D>();
		
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		
		_collider = GetComponent<BoxCollider2D>();
	}

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;
	}
	
	
	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}
	
	
	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}
		
	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		//Stop falling if we're on the ground
		if( _controller.isGrounded )
			_velocity.y = 0;

		if(!lockControl)
			ProcessInput();

		/*
		// we can only jump whilst grounded
		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			if(_controller.isGrounded || canWallJump())
			{
				_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
				//_animator.Play( Animator.StringToHash( "Jump" ) );
			}
		}
		*/
		
		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );
		
		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
		
		_controller.move( _velocity * Time.deltaTime );

	
	}

	private void ProcessInput () {
		
		if( !moveLeft && moveRight )
		{
			//If we're here, we're moving right this frame

			normalizedHorizontalSpeed = 1;
			if(changeFacing)
				foreach(Transform t in reversibles)
					if( t.localScale.x < 0f )
						t.localScale = new Vector3( -t.localScale.x, t.localScale.y, t.localScale.z );
		}
		else if( moveLeft && !moveRight )
		{
			//If we're here, we're moving left this frame

			normalizedHorizontalSpeed = -1;
			if(changeFacing)
				foreach(Transform t in reversibles)
					if( t.localScale.x > 0f )
						t.localScale = new Vector3( -t.localScale.x, t.localScale.y, t.localScale.z );
		}
		else
		{
			//If we're here, we're NOT (at least not left or right) moving this frame

			normalizedHorizontalSpeed = 0;
		}

		if( shortJump ) 
		{
			//If we're in here, we're doing a short jump
			//This is called when the enemy gets hit for the knock-back

			if(_controller.isGrounded)
			{
				_velocity.y = Mathf.Sqrt( 0.5f * jumpHeight * -gravity );
				//_animator.Play( Animator.StringToHash( "Jump" ) );
			}
		}

		//Reset control bools
		moveLeft = false;
		moveRight = false;
		shortJump = false;
	}

	//MoveRight and MoveLeft are called externally to control enemy movement
	//Call each frame for movement
	public void MoveRight (bool changeFacing) { MoveRight(1f,changeFacing); }

	public void MoveRight (float speedMod, bool changeFacing)
	{
		moveRight = true;
		this.changeFacing = changeFacing;
		this.speedMod = speedMod;
	}

	public void MoveLeft (bool changeFacing) { MoveLeft(1f,changeFacing); }

	public void MoveLeft (float speedMod, bool changeFacing)
	{
		moveLeft = true;
		this.changeFacing = changeFacing;
		this.speedMod = speedMod;
	}

	public void MoveAway (Vector3 point, bool changeFacing) { MoveAway(point, 1f, changeFacing); }

	public void MoveAway (Vector3 point, float speedMod, bool changeFacing) {

		if(point.x > gameObject.transform.position.x)
		{
			MoveLeft(speedMod, changeFacing);
		}
		else if(point.x < gameObject.transform.position.x)
		{
			MoveRight(speedMod, changeFacing);
		}
	}

	public void ShortJump () {

		shortJump = true;
	}


}
