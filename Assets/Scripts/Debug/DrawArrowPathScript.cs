using UnityEngine;
using System.Collections;

public class DrawArrowPathScript : MonoBehaviour {

	public Vector2 initialPosition;

	public float xVelocity;
	public float yVelocity;
	public float gravity;

	public float xDistanceToDraw;
	public float numberOfLines;

	private float yPerX;
	private float xDistancePerLine;

	// Use this for initialization
	void Start () {
		yPerX = gravity/xVelocity;
		xDistancePerLine = numberOfLines/xDistanceToDraw;
	}
	
	// Update is called once per frame
	void Update () {

		float currentYVelocity = yVelocity;

		for(int i = 0; i < numberOfLines; i++)
		{

		}
	}
}
