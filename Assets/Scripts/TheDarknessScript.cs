using UnityEngine;
using System.Collections;

public class TheDarknessScript : MonoBehaviour {

	void Awake ()
	{
		GetComponent<SpriteRenderer>().enabled = true;
	}
}
