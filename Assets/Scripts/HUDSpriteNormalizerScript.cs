using UnityEngine;
using System.Collections;

public class HUDSpriteNormalizerScript : MonoBehaviour {


	public float originalAspect = 16.0f/9.0f;
	public bool adjustScale;

	private Vector3 normalizedPosition;
	private Vector2 normalizedScale;

	void Awake () {
		normalizedPosition = transform.position;
		normalizedPosition.x = normalizedPosition.x / originalAspect;

		normalizedScale = transform.localScale / originalAspect;
	}
	
	void Update () {
		Vector3 adjustedPosition = normalizedPosition;
		adjustedPosition.x *= Camera.main.aspect;
		transform.position = adjustedPosition;

		if(adjustScale)
			transform.localScale = normalizedScale * Camera.main.aspect;
	}
}
