using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
//			DontDestroyOnLoad(gameObject);
		}
		else
			DestroyImmediate(this.gameObject);
	}

	void LateUpdate () 
	{
		transform.position = Camera.main.transform.position;
	}
}
