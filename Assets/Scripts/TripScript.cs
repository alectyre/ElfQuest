using UnityEngine;
using System.Collections;

public class TripScript : MonoBehaviour {

	public bool tripping { get { return _tripping; } }
	private bool _tripping;

	public GameObject player;

	public Vector2 tripDashVelocity = new Vector2(22f, 1f);
	public DashTrail tripDashTrail;
	public float tripRunSpeed = 5f;
	public float tripSpeedWhileAttacking = 5f;
	public float tripJumpHeight = 1.5f;

	public float globalFrequency = 1f;
	public float redVelocity;
	public float greenVelocity;
	public float blueVelocity;

	private float red;
	private float green;
	private float blue;

	private Color originalColor;

	public BGMScript bgm;
	public float tripFrequency;
	public float tripAcceleration;

	public AudioClipSequence tripAudioSequence;
	public AudioClipSequence comedownAudioSequence;

	public ParticleSystem aura;
	public float auraRate;
	public float auraAcceleration;

	void Start () {
		red = Camera.main.backgroundColor.r;
		green = Camera.main.backgroundColor.g;
		blue = Camera.main.backgroundColor.b;

		originalColor = new Color();
		originalColor.r = red;
		originalColor.g = green;
		originalColor.b = blue;
	}

	public void StartTripping()
	{
		_tripping = true;
		StartCoroutine(StartTrippingCoroutine());
	}

	public void StopTripping()
	{
		_tripping = false;
	}

	IEnumerator StartTrippingCoroutine()
	{
		bgm.Transition(tripAudioSequence);

		float fudgeValue = 0.98f;

		PlayerMover mover = player.GetComponent<PlayerMover>();
		mover.dashVelocity = tripDashVelocity;
		mover.runSpeed = tripRunSpeed;
		mover.speedWhileAttacking = tripSpeedWhileAttacking;
		mover.jumpHeight = tripJumpHeight;
		mover.dashTrail.drawTrail = false;
		tripDashTrail.drawTrail = true;

		while(_tripping)
		{
			if(globalFrequency <= tripFrequency * fudgeValue)
			{
				globalFrequency = Mathf.Lerp(globalFrequency, tripFrequency, tripAcceleration * Time.deltaTime);
			}

			if(aura.emissionRate <= auraRate * fudgeValue)
			{
				aura.emissionRate = Mathf.Lerp(aura.emissionRate, auraRate, auraAcceleration * Time.deltaTime);
			}

			Color color = Camera.main.backgroundColor;
			
			red += globalFrequency * redVelocity * Time.deltaTime;
			green += globalFrequency * greenVelocity * Time.deltaTime;
			blue += globalFrequency * blueVelocity * Time.deltaTime;
			
			color.r = Mathf.Sin(red);
			color.g = Mathf.Sin(green);
			color.b = Mathf.Sin(blue);

			Camera.main.backgroundColor = color;

			yield return null;
		}

		bgm.Transition(comedownAudioSequence);

		float frequencyFudge = globalFrequency - globalFrequency * fudgeValue;
		float emissionRateFudge = aura.emissionRate * fudgeValue;

		int colorShiftLimit = 500;
		int colorShiftCount = 0;

		bool frequencyDone = false;
		bool colorShiftDone = false;
		bool emissionrateDone = false;

		while(!(frequencyDone && colorShiftDone && emissionrateDone))
		{
			if(globalFrequency > frequencyFudge)
			{
				globalFrequency = Mathf.Lerp(globalFrequency, 0, tripAcceleration * Time.deltaTime);

				if(globalFrequency <= frequencyFudge)
					frequencyDone = true;
			}

			if(colorShiftCount < colorShiftLimit)
			{
				Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, originalColor, tripAcceleration * Time.deltaTime);

				colorShiftCount++;

				if(colorShiftCount >= colorShiftLimit)
					colorShiftDone = true;
			}

			if(aura.emissionRate > 0)
			{
				aura.emissionRate -= Mathf.Lerp(aura.emissionRate, 0, auraAcceleration * Time.deltaTime);

				if(aura.emissionRate < 0)
					aura.emissionRate = 0;

				if(aura.emissionRate <= 0)
				{
					emissionrateDone = true;

					mover.dashVelocity = new Vector2(22, 1);
					mover.runSpeed = 3;
					mover.speedWhileAttacking = 2;
					mover.jumpHeight = 1.1f;
					mover.dashTrail.drawTrail = true;
					tripDashTrail.drawTrail = false;
				}
			}

			yield return null;
		}
	}
}
