using UnityEngine;
using System.Collections;

public class SnakeBehaviorScript : MonoBehaviour {

	public float pathingDistance;
	public bool pathRight;
	public float speed;
	public float chaseSpeed;
	public float visionRange;
	public LayerMask visionLayers;
	public Sprite engageSprite;
	public Sprite lostSightSprite;
	public float behaiorSpriteDuration;
	private float behaviorSpriteDurationRemaining;

	private Animator _animator;

	private bool playerSpotted;
	private bool playerSpottedLastFrame;
	private SpriteRenderer behaviorSpriteRenderer;

	private float distanceRemaining = 0;

	private bool facingRight = true;

	void Awake () {
		_animator = GetComponent<Animator>();
		_animator.SetBool("crawling", true);

		Transform behaviorSpriteObject = gameObject.transform.Find("BehaviorSprite");

		if(behaviorSpriteObject != null)
			behaviorSpriteRenderer = behaviorSpriteObject.gameObject.GetComponent<SpriteRenderer>();

	}

	void FixedUpdate () 
	{
		if(chaseSpeed != 0)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, facingRight?transform.right:-transform.right, visionRange, visionLayers);

			if(hit)
			{
				//if(hit.transform.gameObject.GetComponent<PlayerMover>().IsVisible)
					playerSpotted = true;
			}
		}

		if(playerSpotted && !playerSpottedLastFrame)
		{

			behaviorSpriteDurationRemaining = behaiorSpriteDuration;
			behaviorSpriteRenderer.sprite = engageSprite;

			_animator.speed = _animator.speed * chaseSpeed / speed;

		}
		else if(!playerSpotted && playerSpottedLastFrame)
		{
			behaviorSpriteDurationRemaining = behaiorSpriteDuration;

			if(behaviorSpriteRenderer != null)
				behaviorSpriteRenderer.sprite = lostSightSprite;

			_animator.speed = _animator.speed * speed / chaseSpeed;
		}
		else if(behaviorSpriteDurationRemaining > 0)
		{
			behaviorSpriteDurationRemaining -= Time.deltaTime;
		}
		else
		{
			if(behaviorSpriteRenderer != null)
				behaviorSpriteRenderer.sprite = null;
		}

		float toMove = (chaseSpeed!=0&&playerSpotted)?chaseSpeed * Time.deltaTime:speed * Time.deltaTime;

		if(distanceRemaining <= toMove)
		{
			toMove = distanceRemaining;
		}
	
		transform.position = new Vector3(transform.position.x + (facingRight?toMove:-toMove),
		                                 transform.position.y, 
		                                 transform.position.z);

		distanceRemaining -= toMove;

		if(distanceRemaining <= 0)
		{
			distanceRemaining = pathingDistance;
			facingRight = !facingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);			                               
		}

		playerSpottedLastFrame = playerSpotted;
		playerSpotted = false;
	}

	private IEnumerator BehaviorSpriteCoroutine (Sprite sprite, float duration) {
		
		behaviorSpriteRenderer.sprite = sprite;
		
		float turnOffSpriteAt = Time.realtimeSinceStartup + duration;
		
		while(behaviorSpriteRenderer.sprite != null)
		{
			if(Time.realtimeSinceStartup >= turnOffSpriteAt)
			{
				behaviorSpriteRenderer.sprite = null;
			}
			
			yield return null;
		}
	}
}
