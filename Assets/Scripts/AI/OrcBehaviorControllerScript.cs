using UnityEngine;
using System.Collections;

public class OrcBehaviorControllerScript : MonoBehaviour {

	private OrcBehaviorScript orcScript;

	void Start () {
		orcScript = transform.root.GetComponentInChildren<OrcBehaviorScript>();
	}

	public void SetState(OrcBehaviorScript.State state)
	{
		if(orcScript == null)
		{
			Debug.Log("OrcBehaviorScript not found");
			return;
		}

		orcScript.currentState = state;
	}

}
