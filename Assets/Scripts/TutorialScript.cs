using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

	public GameObject HUDAnchor;
	public bool showTutorial;
	public Vector2 position;
	public float originalAspect = 16.0f/9.0f;

	private float _normalizedX;
	private float _normalizedY;

	private bool seen;

	void Awake ()
	{
		_normalizedX = position.x / originalAspect;
		_normalizedY = position.y / originalAspect;
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			showTutorial = !showTutorial;

			if(showTutorial == true)
				seen = true;
		}
	}

	void OnGUI () 
	{
		float adjustedX = _normalizedX * Camera.main.aspect;
		float adjustedY = _normalizedY * Camera.main.aspect;
		
		if(HUDAnchor)
		{
			adjustedX += HUDAnchor.transform.position.x;
			adjustedY += HUDAnchor.transform.position.y;
		}

		if(showTutorial)
		{
			GUI.Label(new Rect(adjustedX,adjustedY,300,600), 
			          "Left A, Right D, Up W, Down S\n" +
			          "Attack K, Dash L, Jump Space or ;\n"+
			          "Press Escape to close");
		}
		else if(!seen)
		{
			GUI.Label(new Rect(adjustedX,adjustedY,300,600), 
			   	      "Press Escape to for Keyboard inputs");
		}
	}
}
