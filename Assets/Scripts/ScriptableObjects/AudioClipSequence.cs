using UnityEngine;
using System.Collections;

public class AudioClipSequence : ScriptableObject {

	public AudioClip[] clips;

	public AudioClipSequence nextSequence;
	public bool loop;
}
