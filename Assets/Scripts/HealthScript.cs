using UnityEngine;
using System.Collections;

public abstract class HealthScript : MonoBehaviour {

	public float currentHealth;

	public abstract void InflictDamage(float value);

	public abstract void Kill();
}
