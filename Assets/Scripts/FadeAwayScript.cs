using UnityEngine;
using System.Collections;

public class FadeAwayScript : MonoBehaviour {

	public float fadeDelay;
	public float fadeSpeed;

	void Awake ()
	{
		StartCoroutine(FadeAwayCoroutine());
	}

	IEnumerator FadeAwayCoroutine ()
	{
		float delayRemaining = fadeDelay;

		while(delayRemaining > 0)
		{
			delayRemaining -=  Time.deltaTime;
			yield return null;
		}

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		Color newColor = spriteRenderer.color;

		while(newColor.a > 0)
		{
			newColor.a -= fadeSpeed * Time.deltaTime;
			spriteRenderer.color = newColor;

			yield return null;
		}
	}
}
