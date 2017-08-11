using UnityEngine;
using System.Collections;

public class PlayerHealthScript : MonoBehaviour {

	public float currentHealth;
	public float maxHealth;
	public HeartMeterScript heartMeter;
	public float invincibleTime;

	public bool HasMaxHealth {
		get{ return currentHealth >= maxHealth; }
	}

	private PlayerMover _playerMover;

	private bool _invincible;
	private float _originalZ;

	/*
	 * Should make a damage class to contain relevent information like
	 * source, amount, type, and whatever else becomes useful
	 */

	void Awake ()
	{
		_playerMover = GetComponent<PlayerMover>();
		_originalZ = gameObject.transform.position.z;
	}

	void Start () 
	{
		for(int i = 0; i < currentHealth; i++)
		{
			heartMeter.AddHalfHeart();
		}
	}

	public void Heal(float amount) 
	{
		float damage = maxHealth - currentHealth;

		for(int i = 0; i < damage && i < amount; i++)
		{
			currentHealth++;
			heartMeter.AddHalfHeart();
		}
	}

	public void FullHeal()
	{
		Heal (maxHealth - currentHealth);
	}

	public void Invincible(bool isInvincible) 
	{
		_invincible = isInvincible;
	}

	public void Invincible(float duration)
	{
		_invincible = true;

		Invoke("InvincibleOff", duration);
	}

	private void InvincibleOff ()
	{
		_invincible = false;
	}

	public void InflictDamage(GameObject source, float amount) 
	{
		if(_invincible)
			return;

		currentHealth -= amount;

		for(int i = 0; i < amount; i++)
		{
			heartMeter.RemoveHalfHeart();
		}

		Vector3 bloodPos = transform.position;
		TheBloodenerScript.MakeBlood(bloodPos);

		if(currentHealth <= 0)
		{
			Kill();
		}
		else
		{
			_playerMover.DamageKnockBack(source);
			Invincible(invincibleTime);
		}

	}

	public void Kill () 
	{
		Vector3 bloodPos = transform.position;
		TheBloodenerScript.MakeBlood(bloodPos);
		TheBloodenerScript.MakeBlood(bloodPos);
		TheBloodenerScript.MakeBlood(bloodPos);

		currentHealth = 0;
		heartMeter.RemoveAllHearts();
		DeactivatePlayer();
	}

	public void DrownPlayer ()
	{
		currentHealth = 0;
		heartMeter.RemoveAllHearts();

		_playerMover.lockManualControl = true;

		//This is just moving the player sprite behind the water (and probably everything else)
		transform.position = new Vector3(transform.position.x, transform.position.y, 8);

		GetComponent<Collider2D>().enabled = false;
		Invoke("DeactivatePlayer",0.5f) ;
	}

	void OnEnable () 
	{ 
		Vector3 pos = gameObject.transform.position;
		pos.z = _originalZ;
		gameObject.transform.position = pos; 
	}

	private void DeactivatePlayer() 
	{ 
		gameObject.SetActive(false);
	}
}
