using UnityEngine;
using System.Collections;

public class HUDHintSpriteScript : MonoBehaviour {
	
	public float blinkSpeed;
	public float minimumAlpha;

	public float pulseSpeed;
	public Vector2 minimumSize;

	private SpriteRenderer _spriteRenderer ;
	private Vector2 _originalSize;

	void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_originalSize = new Vector2(transform.localScale.x, transform.localScale.y);

		gameObject.SetActive(false);
	}

	void Update () {
		{
			Color newColor = _spriteRenderer.color;
			newColor.a = Mathf.PingPong(Time.time * blinkSpeed, 1.0f-minimumAlpha) + minimumAlpha;
			_spriteRenderer.color = newColor;

			float newWidth = Mathf.PingPong(Time.time * pulseSpeed, _originalSize.x - minimumSize.x) + minimumSize.x;
			float newHeight = Mathf.PingPong(Time.time * pulseSpeed, _originalSize.y - minimumSize.y) + minimumSize.y;

			transform.localScale = new Vector3(newWidth,newHeight,transform.localScale.z);
		}
	}

	void OnEnable ()
	{
		//Set to minAlpha and minSize OnEnable for a consitent starting size and alpha
		Color newColor = _spriteRenderer.color;
		newColor.a = minimumAlpha;
		_spriteRenderer.color = newColor;

		transform.localScale = new Vector3(minimumSize.x,minimumSize.y,transform.localScale.z);
	}
}
