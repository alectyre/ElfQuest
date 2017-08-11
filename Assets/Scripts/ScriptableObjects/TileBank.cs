using UnityEngine;
using System.Collections;

public class TileBank : ScriptableObject {

	public GameObject[] tiles;

	[HideInInspector]
	public bool show = false;
}
