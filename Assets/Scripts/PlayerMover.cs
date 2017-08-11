using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController2D))]
public class PlayerMover : MonoBehaviour
{
	public bool debug;

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float speedWhileAttacking = 4;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public Vector2 dashVelocity = new Vector2(20, 1);
	public float dashDuration = 0.2f;
	public int dashes;
	private int dashesRemaining;

	public Transform[] reversibles;

	private bool facingRight = true;

	private float normalizedHorizontalSpeed = 0;
	private float normalizedVerticalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	private BoxCollider2D _collider;

	public LayerMask wallJumpMask = -1;
	public LayerMask climbableMask = -1;
	public LayerMask canHideMask = -1;

	public bool swordAttack;
	private bool dash;
	private bool moveRight;
	private bool moveLeft;
	private bool jump;
	private bool shortJump;
	private bool changeFacing;
	private float speedMod;
	private bool moveUp;
	private bool moveDown;
	private bool _visible;
	public bool IsVisible { get{ return _visible; } }

	private bool attackingLastFrame;

	private bool isClimbing;
	private bool isDashing;

	public bool lockManualControl;

	private Animator _swooshAniamtor;

	public float lookUpDistance = 1;
	public float lookDownDistance = 2;
	public float lookDelay = 0.2f;

	private SmoothFollow _cameraController;
	private bool lookingDown;
	private bool lookingUp;

	private bool dashReleased;
	private bool jumpReleased;


	/*
	 * Adjusts how close the player must be to individual objects to hide behind them.
	 * The base distance is the sum of half of the player's and object's Renderer.bound.size.x
	 * ie their sprites gotta touch
	 * Used to require the player to become visually obscured.
	 * 
	 * Essentially, it's the overlap required to be hidden. Yeah, definitely the best description so far.
	 */
	public float visibilityPadding; 

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		_swooshAniamtor = transform.Find("SwordAttack").GetComponent<Animator>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		_collider = GetComponent<BoxCollider2D>();

		_cameraController = Camera.main.GetComponent<SmoothFollow>();
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{

	}

	void onTriggerStayEvent( Collider2D col )
	{

	}

	void onTriggerExitEvent( Collider2D col )
	{

	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		if( _controller.isGrounded)
		{
			_velocity.y = 0;
			dashesRemaining = dashes;
			_animator.SetBool("dashing", false);
			GetComponent<DashTrail>().enabled = false;
		}

		if(VisibilityCheck())
			_visible = true;
		else
			_visible = false;
			
		GetInput();

		HandleInput();

		float smoothedMovementFactor;

		// apply horizontal speed smoothing it
		smoothedMovementFactor = _controller.isGrounded || isClimbing? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );


		if(changeFacing && !swordAttack)
		{
			if(_velocity.x > 0)
			{
				foreach(Transform t in reversibles)
					if( t.localScale.x < 0f )
						t.localScale = new Vector3( -t.localScale.x, t.localScale.y, t.localScale.z );
				facingRight = true;
			}
			else if(_velocity.x < 0)
			{
				foreach(Transform t in reversibles)
					if( t.localScale.x > 0f )
						t.localScale = new Vector3( -t.localScale.x, t.localScale.y, t.localScale.z );
				facingRight = false;
			}
		}
		
		
		// apply gravity before moving but not while climbing
		if(isClimbing)
			_velocity.y = Mathf.Lerp( _velocity.y, normalizedVerticalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );
		else
			_velocity.y += gravity * Time.deltaTime;

		_controller.move( _velocity * Time.deltaTime );


		
		//Reset control bools
		attackingLastFrame = swordAttack;
		moveLeft = false;
		moveRight = false;
		jump = false;
		shortJump = false;
		moveUp = false;
		moveDown = false;
		dash = false;
	}

	void OnGUI () {

		if(debug)
		{
			GUI.Label(	new Rect(10, 0, 140, 400), 
				        "canWallJump " + CanWallJump.ToString() + "\n" +
				        "isGrounded " + _controller.isGrounded.ToString()	);
		}
	}

	private void GetInput ()
	{
		if(lockManualControl)
			return;

		string currentAnimation = "";
		AnimationClip aniamtionClip = null;

		if(_swooshAniamtor.GetCurrentAnimatorClipInfo(0).Length != 0)
		{
			aniamtionClip = _swooshAniamtor.GetCurrentAnimatorClipInfo(0)[0].clip;
		}

		if(aniamtionClip)
		{
			currentAnimation = aniamtionClip.name;

			if(aniamtionClip.name == "swoosh")
				swordAttack = true;
			else
				swordAttack = false;
		}
		else
			swordAttack = false;

		if( !dashReleased && Input.GetAxis("Dash") <= 0 )
		{
			dashReleased = true;
		}
		if( Input.GetAxis("Dash") > 0 && dashReleased)
		{
			dash = true;
			dashReleased = false;
		}


		if( Input.GetAxis("Horizontal") > 0 )
		{
			MoveRight(true);
		}
		else if( Input.GetAxis("Horizontal") < 0 )
		{
			MoveLeft(true);
		}

		float minimumVerticalMovement = 0.5f;

		if( Input.GetAxis("Vertical") > minimumVerticalMovement )
		{
			MoveUp();
		}
		else if( Input.GetAxis("Vertical") < -minimumVerticalMovement  )
		{
			MoveDown();
		}

		if( !jumpReleased &&  Input.GetAxis("Jump") <= 0)
		{
			jumpReleased = true;
		}
		if(Input.GetAxis("Jump") > 0 && jumpReleased)
		{
			jump = true;
			jumpReleased = false;
		}
	}
	
	private void HandleInput () 
	{

		///CLIMB A THING
		if( moveUp && !isClimbing )
		{
			if(CanClimbUp)
			{
				isClimbing = true;
				lookingUp = false;
				lookingDown = false;
			}

		}
		else if( moveDown && !isClimbing )
		{
			if(CanClimbDown)
			{
				isClimbing = true;
				_controller.OneWayPlayforms(false, 0.01f);
			}
			
		}
		else if( !CanClimbDown && (_controller.isGrounded || !CanClimbUp) )
		{
			isClimbing = false;
		}
		else if( moveDown )
		{
			if(CanClimbDown)
				_controller.OneWayPlayforms(false, 0.01f);
		}


		///ANIMATION ATTACK
		if(!isClimbing && swordAttack && !attackingLastFrame){
			_animator.Play("player_slash");
		}

		///DASH
		if( dash )
		{
			if( dashesRemaining <= 0 )
				return;

			dashesRemaining--;

			Vector2 newDashVelocity = dashVelocity;

			if(moveLeft)
				newDashVelocity.x = -newDashVelocity.x;
			else if(moveRight)
				newDashVelocity.x = newDashVelocity.x;
			else
				newDashVelocity.x = facingRight?newDashVelocity.x:-newDashVelocity.x;


			_controller.move(new Vector3(0,0.1f,0));

			Dash(newDashVelocity, dashDuration);
		}

		///MOVE LEFT OR RIGHT
		if( moveRight )
		{
			normalizedHorizontalSpeed = swordAttack?speedWhileAttacking / runSpeed  * speedMod:1 * speedMod;
			_animator.SetBool("walking", true);

		}
		else if( moveLeft )
		{
			normalizedHorizontalSpeed = swordAttack?-speedWhileAttacking / runSpeed * speedMod:-1 * speedMod;
			_animator.SetBool("walking", true);
		}
		else
		{
			_animator.SetBool("walking", false);
			normalizedHorizontalSpeed = 0;
		}

		///LOOK DOWN
		if( moveDown && _controller.isGrounded && !lookingDown && !isClimbing)
		{
			lookingDown = true;
			StartCoroutine(LookCoroutine(lookDownDistance));
		}
		if( moveDown )
		{
			//lookingDown = true;
		}
		else
		{
			lookingDown = false;
		}


		///LOOK UP
		if( moveUp && _controller.isGrounded && !lookingUp && !isClimbing)
		{
			lookingUp = true;
			StartCoroutine(LookCoroutine(-lookUpDistance));
		}
		if( moveUp )
		{
			//lookingUp = true;
		}
		else
		{
			lookingUp = false;
		}

		///WE ARE CLIMBING, WHY IS THIS HERE?
		if(isClimbing)
		{
			_animator.SetBool("climbing", true);
			_animator.SetBool("walking", false);

			if( moveUp )
				normalizedVerticalSpeed = swordAttack?0.75f:1;
			else if( moveDown )
				normalizedVerticalSpeed = swordAttack?-0.75f:-1;
			else
				normalizedVerticalSpeed = 0;

			if(moveLeft || moveRight || moveUp || moveDown)
				_animator.speed = 1;
			else
				_animator.speed = 0;
		}
		else
		{
			_animator.SetBool("climbing", false);
			_animator.speed = 1;
		}

		///SHORT JUMP AkA OWFUCK
		if( shortJump ) 
		{
			if(_controller.isGrounded)
			{
				_velocity.y = Mathf.Sqrt( 0.5f * jumpHeight * -gravity );
				//_animator.Play( Animator.StringToHash( "Jump" ) );
			}
		}

		///BIG BOY JUMP AND MOVE DOWN THROUGH ONE-WAY PLATFORMS
		if( jump )
		{
			if(_controller.isGrounded) //|| canWallJump())
			{
				if( moveDown )
				{
					_controller.OneWayPlayforms(false, 0.1f);
				}
				else
				{
					_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
				}
			}
		}
	}

	bool CanWallJump {
		get {
			float spacer = 0.01f;
			float width = _collider.size.x;
			float leftBound = transform.position.x - (width/2) - spacer;
			float rightBound = transform.position.x + (width/2) + spacer;

			float[] heights = {	transform.position.y + (_collider.size.y/2) + _collider.offset.y, 
								transform.position.y + (_collider.size.y/5) + _collider.offset.y,
								transform.position.y - (_collider.size.y/5) + _collider.offset.y,
								transform.position.y - (_collider.size.y/2) + _collider.offset.y + 0.001f };

			float distance = 0.03f;

			foreach(float height in heights) {

				Debug.DrawRay(new Vector2(leftBound, height),  distance* -Vector2.right, Color.red, 0.001f);
				Debug.DrawRay(new Vector2(rightBound, height),  distance* Vector2.right, Color.red, 0.001f);

				RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(leftBound, height), -Vector2.right, distance, wallJumpMask.value);

				RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(rightBound, height), Vector2.right, distance, wallJumpMask.value);

				if(leftHit.collider || rightHit.collider)
					return true;
			}

			return false;
		}
	}

	bool CanClimbUp
	{
		get {
			float width = 0.3f; //A lot of the width is in the collider of climbable tiles

			Vector2 rightSide = transform.position;
			rightSide.x = rightSide.x + width/2;
			rightSide.y = rightSide.y + _collider.offset.y;

			Vector2 leftSide = transform.position;
			leftSide.x = leftSide.x - width/2;
			leftSide.y = leftSide.y + _collider.offset.y;
			
			RaycastHit2D hit = Physics2D.Linecast(rightSide, leftSide, climbableMask);
			Debug.DrawLine(rightSide, leftSide, Color.cyan, 0.001f);

			if(hit)
				return true;
			/*
			leftSide.y = leftSide.y + _collider.size.y/2;
			rightSide.y = rightSide.y + _collider.size.y/2;

			hit = Physics2D.Linecast(rightSide, leftSide, climbableMask);
			Debug.DrawLine(rightSide, leftSide, Color.cyan, 0.001f);

			if(hit)
				return true;
			*/
			return false;
		}
	}

	bool CanClimbDown 
	{
		get{
			float width = 0.01f; //A lot of the width is in the collider of climbable tiles
			float distanceBelowCollider = 0.05f;
			
			Vector2 rightSide = transform.position;
			rightSide.x = rightSide.x + width/2;
			rightSide.y = rightSide.y - _collider.size.y/2 + _collider.offset.y - distanceBelowCollider;

			Vector2 leftSide = transform.position;
			leftSide.x = leftSide.x - width/2;
			leftSide.y = leftSide.y - _collider.size.y/2 + _collider.offset.y - distanceBelowCollider;
						
			RaycastHit2D hit = Physics2D.Linecast(rightSide, leftSide, climbableMask);
			Debug.DrawLine(rightSide, leftSide, Color.cyan, 0.001f);
			
			if(hit)
				return true;
			
			return false;
		}
	}

	void LockManualControl() //For AnimationEvent calling
	{ 
		lockManualControl = true;
	}

	void UnlockManualControl() //For AnimationEvent calling
	{ 
		lockManualControl = false;
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

	public void MoveUp() {
		moveUp = true;
	}

	public void MoveDown() {
		moveDown = true;
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

	public void DamageKnockBack(GameObject source) 
	{
		lockManualControl = true;
		StartCoroutine(DamageKnockBackCoroutine(source, 0.5f));
	}

	IEnumerator DamageKnockBackCoroutine(GameObject source, float duration)
	{
		shortJump = true;
		_animator.Play("player_hit");
		Vector3 sourcePosition = source.transform.position;

		while(duration > 0)
		{
			MoveAway(sourcePosition, false);

			duration -= Time.deltaTime;

			yield return null;
		}

		lockManualControl = false;
	}

	public void Dash(Vector2 velocity, float duration)
	{
		StartCoroutine(DashCoroutine(velocity, duration));
	}

	IEnumerator DashCoroutine(Vector2 velocity, float duration)
	{
		//lockManualControl = true;
		GetComponent<DashTrail>().enabled = true;
		_animator.SetBool("dashing", true);
		isDashing = true;

		_controller.velocity = Vector2.zero; //Kill velocity so our dash velocity is the only thing moving us 

		while(duration > 0)
		{
			_velocity.x = velocity.x;
			_velocity.y = velocity.y;

			duration -= Time.deltaTime;

			yield return null;
		}
						
		isDashing = false;
		lockManualControl = false;
	}

	IEnumerator LookCoroutine(float yOffset)
	{		
		float delayRemaining = lookDelay;

		while(delayRemaining > 0)
		{
			delayRemaining -= Time.deltaTime;
			yield return null;
		}

		float originalOffset = _cameraController.cameraOffset.y;
		
		_cameraController.cameraOffset.y += yOffset;

		while(lookingDown || lookingUp)
		{
			yield return null;
		}

		_cameraController.cameraOffset.y = originalOffset;
	}


	public bool VisibilityCheck()
	{
		/* 
		 * checkWidth will determine how for left and right to scan for things
		 * to hide behind.
		 * 
		 * Otherwise use visibilityPadding to adjust how close we need to be
		 * (center to center) with an object to count as hidden, with the base
		 * value being determined by the width of each object's sprite.
		 * 
		 * * If we're being covered by multiple things, get free invisibility.
		 * This overcomes the normal padding on checks for coverage, which will
		 * usually cause the player to need to be be much more than halfway 
		 * behind an object to count as being hidden.
		 * 
		 * The goal here is to sync up what looks to the player like they're
		 * hidden with the code's interpretation.
		 */

		float checkDistance = 0.5f;
		
		Vector2 rightSide = transform.position;
		rightSide.x = rightSide.x + checkDistance;
		rightSide.y = rightSide.y + _collider.offset.y;
		
		Vector2 leftSide = transform.position;
		leftSide.x = leftSide.x - checkDistance;
		leftSide.y = leftSide.y + _collider.offset.y;

		//Look for things to hide behind
		RaycastHit2D[] hits = Physics2D.RaycastAll(rightSide, Vector3.left, rightSide.x - leftSide.x, canHideMask);
		Debug.DrawLine(rightSide, leftSide, Color.yellow, 0.001f);

		if(hits.Length != 0)
		{
			foreach(RaycastHit2D hit in hits)
			{
				//Need something visible to hide behind
				Renderer hitSpriteRenderer = hit.transform.gameObject.GetComponent<Renderer>();

				if(hitSpriteRenderer)
				{
					float padding = hits.Length>1?0:visibilityPadding;

					float maxDistance = GetComponent<Renderer>().bounds.size.x/2 + hitSpriteRenderer.bounds.size.x/2 + padding;

					float xOffset = Mathf.Abs(transform.position.x - hit.transform.position.x);

					if(xOffset <= maxDistance)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}
