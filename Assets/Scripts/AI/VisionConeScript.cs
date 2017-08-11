using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisionConeScript: MonoBehaviour {

	public GameObject target;
	public Vector3[] sightPoints; //Relative to target center
	public LayerMask blocksSight;

	public float sightRange;
	private float sqrSightRange;
	public float fieldOfVision;
	public Vector3 sightDirection;
	public Vector3 sightOffset;

	public float updateFrequency;

	public bool debug;

	private Vector3 currentSightDirection;
	private Vector3 currentSightOrigin;

	private List<GameObject> detectables;

	public bool canSee{ get{ return SightCheck(target); } }

	private float[] angles;
	private Vector3[] offsets;
	private bool[] visibility;

	private float prevXScale;
	//private float prevYScale;

	void Awake ()
	{
		offsets = new Vector3[sightPoints.Length];
		angles = new float[sightPoints.Length];
		visibility = new bool[sightPoints.Length];

		sqrSightRange = sightRange * sightRange;

		prevXScale = transform.localScale.x;
		//prevYScale = transform.localScale.y;
	}

	void Update ()
	{
		//SightCheck(target);
	}

	int debugIndex = 0;

	void OnGUI ()
	{
		if(debug)
		{

			if(GUI.Button(new Rect(8f, 6f, 22f, 22f), debugIndex.ToString()))
			{
				debugIndex++;
				if(debugIndex >= offsets.Length)
					debugIndex = 0;
			}

			GUI.Label(new Rect(10f,28f,600f,400f), 
			          "Can see? " + visibility[debugIndex].ToString()
			          + "\n" + "Angle: " + angles[debugIndex].ToString()
			          + "\n" + "Offset: " + offsets[debugIndex].ToString()
			          + "\n" + "currentSightDirection: " + currentSightDirection.ToString()
			          );
		}
	}

	void UpdateSight()
	{
		// This is gross but adequate. A general solution to flipping the sprite and alerting
		// all interested parties, that didn't involve massive reference exchange, would be lovely.
		if(transform.localScale.x < 0 && prevXScale > 0)
		{
			sightDirection.x = -sightDirection.x;
			sightOffset.x = -sightOffset.x;
		}

		if(transform.localScale.x > 0 && prevXScale < 0)
		{
			sightDirection.x = -sightDirection.x;
			sightOffset.x = -sightOffset.x;
		}

		prevXScale = transform.localScale.x;

		/* Let's hope it never comes to this.
		if(transform.localScale.y < 0 && preyYScale > 0)
			sightDirection.y = -sightDirection.y;
		
		if(transform.localScale.y > 0 && prevYScale < 0)
			sightDirection.y = -sightDirection.y;
		
		prevYScale = transform.localScale.y;
		*/

		currentSightDirection = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z) *  sightDirection;
		currentSightOrigin = transform.position + Quaternion.Euler(0,0,transform.rotation.eulerAngles.z) * sightOffset;

		for(int i = 0; i < sightPoints.Length; i++)
		{
			offsets[i] = sightPoints[i] + target.transform.position - currentSightOrigin;
			
			angles[i] = Vector3.Angle(currentSightDirection, offsets[i]);
		}
	}

	public bool SightCheck(GameObject target)
	{
		if(target == null || target.activeSelf == false)
		{
			Debug.Log("Taget dead, can't see");
			return false;
		}

		bool result = false;

		UpdateSight();

		if(debug)
			DrawSightDebug();

		for(int i = 0; i < sightPoints.Length; i++)
		{
			//Check if within vision range, within field of vision, and unblocked
			if(	offsets[i].sqrMagnitude > sqrSightRange ||
			   	angles[i] > fieldOfVision/2 || 
			   	Physics2D.Raycast(currentSightOrigin, offsets[i], offsets[i].magnitude, blocksSight).collider )
			{
				visibility[i] = false;
			}
			else
			{
				result = true;
				visibility[i] = true;
			}
		}

		return result;
	}

	
	void DrawSightDebug()
	{
		//Calculate the currentSightDirection as a rotation on a unit circle
		float rotation = transform.rotation.eulerAngles.z + Mathf.Atan2(currentSightDirection.y, currentSightDirection.x) * 180/Mathf.PI;
		
		//Draw the two straight edges of the cone
		float x1 = Mathf.Cos((rotation-fieldOfVision/2)/180*Mathf.PI);
		float y1 = Mathf.Sin((rotation-fieldOfVision/2)/180*Mathf.PI);
		Vector3 d1 = new Vector3(x1, y1, 0);;
		
		Debug.DrawRay(currentSightOrigin, d1 * sightRange, Color.green);
		
		float x2 = Mathf.Cos((rotation+fieldOfVision/2)/180*Mathf.PI);
		float y2 = Mathf.Sin((rotation+fieldOfVision/2)/180*Mathf.PI);
		Vector3 d2 = new Vector3(x2, y2, 0);;
		
		Debug.DrawRay(currentSightOrigin, d2* sightRange, Color.green);
		
		//Draw the arc of the cone
		DebugScript.DrawArc(100, sightRange, fieldOfVision, rotation-fieldOfVision/2, currentSightOrigin, Color.green);
		
		for(int i = 0; i < sightPoints.Length; i++)
		{
			if(angles[i] < fieldOfVision/2 && offsets[i].sqrMagnitude <= sqrSightRange)
			{
				RaycastHit2D hit= Physics2D.Raycast(currentSightOrigin, offsets[i], sightRange, blocksSight);
				
				//Comparing sqrMagnitude is faster, so just get the sqrt of the smaller sqrMagnitude. Half the sqrts!
				float sightRayLength = Mathf.Sqrt(Mathf.Min(offsets[i].sqrMagnitude, ((Vector3)hit.point - transform.position - sightOffset).sqrMagnitude));
				
				Debug.DrawRay(currentSightOrigin, offsets[i].normalized * sightRayLength, Color.red);
			}
		}
	}


}