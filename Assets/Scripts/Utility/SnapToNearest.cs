using UnityEngine;

[ExecuteInEditMode]
public class SnapToNearest : MonoBehaviour {

	public float positionIncrement;
	public Vector3 positionOffset;

	public float rotationIncrement;
	public Vector3 rotationOffset;

	public float scaleIncrement;
	public Vector3 scaleOffset;


	public bool lockPosition;
	private bool positionLockedLastFrame;
	private Vector3 savedPosition;

	public bool lockRotation;
	private bool rotationLockedLastFrame;
	private Quaternion savedRotation;

	public bool lockScale;
	private bool scaleLockedLastFrame;
	private Vector3 savedScale;

	public bool enableInPlaymode;


	void Update () 
	{

		if(lockPosition)
		{
			//If we were NOT locked last frame, save current value
			if(!positionLockedLastFrame)
			{
				savedPosition = transform.position;
				positionLockedLastFrame = true;
			}
			//If we were locked last frame, keep reverting to saved value
			else
				transform.position = savedPosition;
		}
		else
		{
			positionLockedLastFrame = false;

			//Not locked, so snap values to nearest increment each frame
			if(positionIncrement != 0)
			{
				Vector3 p = transform.position;

				/*
				 * This I only sort of understand, because did it with intuition not brain :3
				 * 
				 * Rounding the absolute value keeps rounding behavior universal
				 * between positive and negative values. 
				 * 
				 * Subracting the offset and then adding
				 * it back seems to keep objects centered on the mouse while moving them while
				 * still offsetting appropriately.
				 * 
				 * I think I should be able to move the absolute value thing into Round() itself
				 * and that might not be completely inappropriate?
				 */
				transform.position = new Vector3(SignOf(p.x)*Round(Mathf.Abs(p.x - positionOffset.x), positionIncrement) + positionOffset.x,
			                                 	 SignOf(p.y)*Round(Mathf.Abs(p.y - positionOffset.y), positionIncrement) + positionOffset.y,
			                                 	 SignOf(p.z)*Round(Mathf.Abs(p.z - positionOffset.z), positionIncrement) + positionOffset.z);
			}
		}

		if(lockRotation)
		{
			if(!rotationLockedLastFrame)
			{
				savedRotation = transform.rotation;
				rotationLockedLastFrame = true;
			}
			transform.rotation = savedRotation;
		}
		else
		{
			rotationLockedLastFrame = false;

			if(rotationIncrement != 0)
			{
				Vector3 r = transform.rotation.eulerAngles;

				transform.rotation = Quaternion.Euler(new Vector3(SignOf(r.x)*Round(Mathf.Abs(r.x - rotationOffset.x), rotationIncrement) + rotationOffset.x,
				                                 				  SignOf(r.y)*Round(Mathf.Abs(r.y - rotationOffset.y), rotationIncrement) + rotationOffset.y,
			                                 	 				  SignOf(r.z)*Round(Mathf.Abs(r.z - rotationOffset.z), rotationIncrement) + rotationOffset.z));
			}
		}

		if(lockScale)
		{
			if(!scaleLockedLastFrame)
			{
				savedScale = transform.localScale;
				scaleLockedLastFrame = true;
			}
			else
				transform.localScale = savedScale;
		}
		else
		{
			scaleLockedLastFrame = false;

			if(scaleIncrement != 0)
			{
				Vector3 s = transform.localScale;
				
				transform.localScale = new Vector3(SignOf(s.x)*Round(Mathf.Abs(s.x - scaleOffset.x), scaleIncrement) + scaleOffset.x,
				                                   SignOf(s.y)*Round(Mathf.Abs(s.y - scaleOffset.y), scaleIncrement) + scaleOffset.y,
				                                   SignOf(s.z)*Round(Mathf.Abs(s.z - scaleOffset.z), scaleIncrement) + scaleOffset.z);
			}
		}

		//Destroy in Playmode if !enableInPlaymode
		//Don't worry, it comes back when you exit Playmode
		//Do it at the end of the first Update so Instantiated
		//objects snap once before the script disappears
		if (Application.isPlaying && !enableInPlaymode)
		{
			Destroy(this);
			return;
		}

	}

	//Returns -1 or 1 based on the sign of the input
	private int SignOf(float input)
	{
		return input>=0?1:-1;
	}

	//Returns input rounded to the nearest incremement
	private float Round(float input, float increment)
	{
		float snappedValue;
		
		snappedValue = increment * Mathf.Round((input / increment));
		
		return snappedValue;
	}
}
