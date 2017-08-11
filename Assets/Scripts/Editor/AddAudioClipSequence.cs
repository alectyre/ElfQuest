using UnityEngine;
using UnityEditor;

public class AddAudioClipSequence
{
	[MenuItem("Assets/Create/AudioClipSequence")]
	private static void NewMenuOption()
	{
		ScriptableObjectUtility.CreateAsset<AudioClipSequence>();
	}
}
