using UnityEngine;
using System.Collections;

public class KillScript : MonoBehaviour {

	public KeyCode killKey;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(killKey))
			Destroy(gameObject);
	}
}
