using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HearingScript : System.Object {

	/*
	 * Use: 
	 * 
	 * 	//In script that wants to react to sounds
	 *	HearingScript.registerListener(new Listener( gameObject, FunctionName )); 
	 *
	 *	//In script that wants to create sounds
	 *	HearingScript.createNoise( positionOfSound, 1f );
	 *
	 *
	 * Possible additions/changse:
	 * 	-Have walls dampen sound range (simplistic model, just linecast between
	 * 	 listeners in range and shrink radius if something is hit)?
	 * 	
	 *  -I should set up a Globals ScriptableObject and it should contain the
	 * 	 radius for "quiet" and "loud" sounds (unless that gets changed)
	 * 
	 *	-Sound types or seperate listener pools?
	 */

	public struct Listener
	{
		public delegate void HearingDelegate();

		public GameObject gameObject;
		public HearingDelegate didHearSound;
		public float soundRangeModifier;

		public Listener(GameObject self, HearingDelegate didHearSound)
		{
			this.gameObject = self;
			this.didHearSound = didHearSound;
			this.soundRangeModifier = 1;
		}

		public Listener(GameObject self, HearingDelegate didHearSound, float soundRangeModifier)
		{
			this.gameObject = self;
			this.didHearSound = didHearSound;
			this.soundRangeModifier = soundRangeModifier;
		}
	}

	private static List<Listener> registeredListeners = new List<Listener>();
	public static bool debug;

	public static void registerListener(Listener listener)
	{
		registeredListeners.Add(listener);
	}

	public static void unregisterListener(Listener listener)
	{
		registeredListeners.Remove(listener);
	}

	public static void createNoise(Vector3 location, float radius)
	{
		if(debug)
		{
			DebugScript.DrawCircle(100, radius, location, Color.cyan, 0.5f);
		}

		float sqrRadius = radius * radius;

		foreach(Listener listener in registeredListeners)
		{
			Vector3 soundOffset = location - listener.gameObject.transform.position;

			//Do easy, arithmatic only check to see if listener could possibly be close enough to hear
			if( Mathf.Abs(soundOffset.x) > radius || Mathf.Abs(soundOffset.y) > radius )
				continue;

			float sqrDistance = soundOffset.sqrMagnitude;

			if(sqrDistance <= sqrRadius * listener.soundRangeModifier)
			{
				listener.didHearSound();
			}
		}
	}

}
