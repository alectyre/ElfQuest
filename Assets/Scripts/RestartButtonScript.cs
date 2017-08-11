using UnityEngine;
using System.Collections;

public class RestartButtonScript : MonoBehaviour {

	public GameObject player;

	void OnGUI () {

		if(Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		if(!player.activeSelf)
		{
			float width = 100f;
			float height = 30f;


			//Screen.width/2 - width/2, Screen.height/2 - height/2, width, height

			if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 - height/2, width, height), "Restart"))
			{
				Application.LoadLevel("VerticalSlice");
			}
		}
	}
}
