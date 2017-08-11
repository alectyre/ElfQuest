using UnityEngine;
using System.Collections;

public class OrcBehaviorScript : MonoBehaviour {

	public GameObject target;
	private Vector3 targetOffset;
	private Vector3 lastKnownLocation;
	private float distanceToTarget;

	public Sprite seekSprite;
	public Sprite alertSprite;
	public Vector3 stateSpriteOffset;
	private SpriteRenderer spriteRenderer;

	public float startSeekDistance;
	public float maximumSeekDistance;
	public float minimumSeekDistance;
	public float seekSpeed;

	public float alertDistance;
	public float alertThreshold;
	public int playingChickenThreshold;
	private int playingChickenCount;
	private float alertTime;

	private bool targetVisibleLastFrame;
	private bool targetVisible;

	public float idleMinDuration;
	public float idleMaxDuration;
	private float idleTimeRemaining;

	public enum PatrolMode
	{
		Loop, Reverse
	};

	public float patrolSpeed;
	public Vector3[] patrolPoints;
	public PatrolMode patrolMode;
	private int patrolPointIncrement = 1;
	private int currentPatrolPointIndex;
	public float minDistanceToPatrolPoint;

	public enum State
	{
		Idle, Patrol, Alert, Engaging, Attacking, KnockBack
	};

	public State currentState;
	private State previousState;

	public bool debug;

	private EnemyControllerScript enemyControllerScript;

	private GameObject behaviorSprite;

	private VisionConeScript sightScript;

	private Animator animator;

	void Awake ()
	{
		previousState = currentState = State.Idle;
		alertTime = 0;

		behaviorSprite = new GameObject();
		spriteRenderer = behaviorSprite.AddComponent<SpriteRenderer>();
		behaviorSprite.transform.name = "BehaviorSprite";
		behaviorSprite.transform.parent = gameObject.transform;
		behaviorSprite.transform.position = stateSpriteOffset + gameObject.transform.position;

		sightScript = GetComponent<VisionConeScript>();

		enemyControllerScript = GetComponent<EnemyControllerScript>();

		animator = GetComponent<Animator>();
	}

	void FixedUpdate ()
	{
		UpdateTargetVisibility();

		UpdateState(Time.fixedDeltaTime);

		HandleState(Time.fixedDeltaTime);

		targetVisibleLastFrame = targetVisible;
	}

	private void UpdateTargetVisibility() {

		targetVisible = false;
		
		if(target != null && sightScript != null)
		{
			if(target.activeSelf)
				targetVisible = sightScript.canSee;
		}
		
		if(targetVisible)
		{
			targetOffset = target.transform.position - transform.position;
		}
		else
		{
			targetOffset = Vector3.one * float.MaxValue;
		}
	}

	/*
	 * Should kill this noise and have all states handle transitions internally
	 */
	private void UpdateState(float deltaTime)
	{
		if(currentState == State.KnockBack)
		{

		}
		else if(currentState != State.Engaging && currentState != State.Attacking)
		{
			if(targetVisible)
			{
				currentState = State.Alert;
			}  	
		}

		else if(currentState == State.Engaging)
		{
			if(!targetVisible)
			{
				currentState = State.Alert;
			}
		}

		else if(currentState != State.Alert && currentState != State.Engaging)
		{

		}
	}

	private void HandleState(float deltaTime)
	{
		switch(currentState)
		{
		case State.Engaging:
			Engage(deltaTime);
			break;
		case State.Alert:
			Alert(deltaTime);
			break;
		case State.Idle:
			Idle(deltaTime);
			break;
		case State.Patrol:
			Patrol(deltaTime);
			break;
		case State.Attacking:
			Attacking(deltaTime);
			break;
		case State.KnockBack:
			KnockBack(deltaTime);
			break;
		default:
			Debug.Log("Y u no have state?");
			break;
		}
	}

	private void Alert(float deltaTime)
	{
		//State initializer
		if(currentState !=  previousState)
		{
			Debug.Log("Alerted...");
			spriteRenderer.sprite = alertSprite;
			previousState = currentState;
			playingChickenCount = 0;
		}

		if(targetVisible)
		{
			//If we're alert and the player ducks out and back into sight, boost alertness
			if(alertTime > 0 && !targetVisibleLastFrame)
			{
				playingChickenCount++;

				if(playingChickenCount >= playingChickenThreshold)
					currentState = State.Engaging;
			}

			alertTime += deltaTime;

			if (alertTime >= alertThreshold)
			{
				alertTime = alertThreshold;

				currentState = State.Engaging;
			}
		}
		else
		{
			alertTime -= deltaTime;

			if(alertTime <= 0)
			{
				alertTime = 0;

				currentState = State.Patrol;
			}
		}

	}

	private void Engage(float deltaTime)
	{
		//State initializer
		if(currentState !=  previousState)
		{
			Debug.Log("Engaging...");
			if(previousState != State.Attacking)
				StartCoroutine(EngageCoroutine());
			previousState = currentState;
		}

		lastKnownLocation = target.transform.position;
	
		if(Mathf.Abs(targetOffset.x) > minimumSeekDistance)
		{
			if(targetOffset.x < 0)
			{
				enemyControllerScript.MoveLeft(true);
			}
			else if(targetOffset.x > 0)
			{
				enemyControllerScript.MoveRight(true);
			}
		}
		else
		{
			currentState = State.Attacking;
		}
	}

	private IEnumerator EngageCoroutine () {

		spriteRenderer.sprite = seekSprite;

		float turnOffSpriteAt = Time.realtimeSinceStartup + 0.5f;

		while(spriteRenderer.sprite != null)
		{
			if(Time.realtimeSinceStartup >= turnOffSpriteAt)
			{
				spriteRenderer.sprite = null;
			}

			yield return null;
		}
	}

	private void Attacking(float deltaTime) {
		//State initializer
		if(currentState !=  previousState)
		{
			Debug.Log("Attacking...");
			previousState = currentState;
		}

		animator.Play("axe_armor_attack");
	}

	private void KnockBack (float deltaTime) {

		if(currentState !=  previousState)
		{
			Debug.Log("Kocked back");
			StartCoroutine(KnockBackCoroutine(previousState));
			previousState = currentState;
		}
	}

	private IEnumerator KnockBackCoroutine (State previousState) {

		float duration = 0.6f;

		animator.Play("axe_armor_idle");

		enemyControllerScript.ShortJump();

		while(duration > 0) {

			enemyControllerScript.MoveAway(target.transform.position, 0.25f, false);

			duration -= Time.deltaTime;

			yield return null;
		}

		//if(previousState == State.Idle || previousState == State.Patrol)
		currentState = State.Engaging;
	}

	private void Idle (float deltaTime) {
	
		//State initializer
		if(currentState !=  previousState)
		{
			Debug.Log("Going idle...");
			spriteRenderer.sprite = null;
			idleTimeRemaining = Random.Range(idleMinDuration, idleMaxDuration);
			previousState = currentState;
		}

		idleTimeRemaining -= deltaTime;

		if(idleTimeRemaining <= 0)
		{
			currentState = State.Patrol;
		}
	}

	private void Patrol(float deltaTime)
	{
		//State initializer
		if(currentState !=  previousState)
		{
			Debug.Log("Starting patrol...");
			spriteRenderer.sprite = null;
			if(previousState == State.Engaging)
				currentPatrolPointIndex = FindIndexOfNearestPoint(patrolPoints);
			previousState = currentState;
		}

		Vector3 patrolPointOffset = patrolPoints[currentPatrolPointIndex] - transform.position;
		float distanceToPatrolPoint = patrolPointOffset.magnitude;

		if(distanceToPatrolPoint <= minDistanceToPatrolPoint)
		{
			currentPatrolPointIndex = GetNextPatrolPointIndex();

			currentState = State.Idle;
		}

		if(patrolPointOffset.x < 0)
		{
			enemyControllerScript.MoveLeft(true);
		}
		else if(patrolPointOffset.x > 0)
		{
			enemyControllerScript.MoveRight(true);
		}
	}

	private int GetNextPatrolPointIndex()
	{
		if(patrolMode == PatrolMode.Loop)
		{
			if(currentPatrolPointIndex == patrolPoints.Length-1)
				return 0;
			else
				return currentPatrolPointIndex + patrolPointIncrement;
		}
		else if(patrolMode == PatrolMode.Reverse)
		{
			if(currentPatrolPointIndex == patrolPoints.Length-1 || currentPatrolPointIndex == 0)
				patrolPointIncrement = -patrolPointIncrement;

			return currentPatrolPointIndex + patrolPointIncrement; 
		}
		else
			return -1;
	}

	private int FindIndexOfNearestPoint(Vector3[] points)
	{
		int nearestPointIndex;
		float nearestDistance;

		if(points.Length > 0)
		{
			nearestPointIndex = 0;
			nearestDistance = Vector3.Distance(transform.position, points[0]);
		}
		else
		{
			return -1;
		}

		for(int i = 1; i < points.Length; i++)
		{
			float distance = Vector3.Distance(transform.position, points[i]);

			if(distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestPointIndex = i;
			}
		}

		return nearestPointIndex;
	}

	void OnGUI()
	{
		if(debug)
		{
			DebugScript.DrawCircle(100, alertDistance, transform.position, Color.yellow);
			DebugScript.DrawCircle(100, startSeekDistance, transform.position, Color.red);

			GUI.Label(new Rect(10,10,200,30), "State: " + currentState.ToString());
			GUI.Label(new Rect(10,30,200,30), "Patrol Point Index: " + currentPatrolPointIndex.ToString());
			GUI.Label(new Rect(10,50,200,30), "Alertness: " + alertTime.ToString("F1") + " ChickenCount: " + playingChickenCount.ToString());

			foreach(Vector3 point in patrolPoints)
			{
				DebugScript.DrawCross(point, 0.5f, Color.green);
			}
		}
	}

}
