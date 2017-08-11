using UnityEngine;
using System.Collections;

public class DashTrailObject : MonoBehaviour
{
	public new SpriteRenderer renderer;
	[HideInInspector] public Color startColor = Color.white;
	[HideInInspector] public Color endColor = Color.white;

	private Vector2 position;
	private float displayTime;
	private float timeDisplayed;


	// Update is called once per frame
	void LateUpdate ()
	{
		transform.position = position;

		timeDisplayed += Time.deltaTime;

		renderer.color = Color.Lerp (startColor, endColor, timeDisplayed / displayTime);

		if (timeDisplayed >= displayTime)
		{
			Debug.Log("Hiding");
			gameObject.SetActive(false);
			//Destroy (gameObject);
		}
	}

	public void Initiate (float displayTime, Sprite sprite, Vector2 position, DashTrail trail)
	{
		gameObject.SetActive(true);

		this.displayTime = displayTime;
		transform.position = this.position = position;
		renderer.sprite = sprite;
		timeDisplayed = 0;
		startColor = trail.startColor;
		endColor = trail.endColor;
		renderer.flipX = trail.leadingSprite.flipX;

		renderer.color = Color.Lerp (startColor, endColor, timeDisplayed / displayTime);
	}
}