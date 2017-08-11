using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BGMScript : MonoBehaviour {

	//public AudioClipSequence[] sequences;
	public AudioClipSequence initialSequence;
	public bool playOnAwake;

	private AudioClipSequence currentSequence;
	private AudioClipSequence nextSequence;

	private int sequenceIndex;

	public bool willTransition;

	private AudioSource _currentAudioSource;
	private AudioSource _nextClipAudioSource;
	private AudioSource _nextSequenceAudioSource;

	void Awake ()
	{
		_currentAudioSource = GetComponent<AudioSource>();

		currentSequence = initialSequence;
		sequenceIndex = -1;

		if(playOnAwake)
			PlayNext();
	}

	void FixedUpdate ()
	{
		float clipTimeRemaining = _currentAudioSource.clip.length - _currentAudioSource.time;

		if(clipTimeRemaining <= Time.deltaTime * 0.9f)
		{
			if(sequenceIndex == currentSequence.clips.Length-1 &&
			   !currentSequence.loop && nextSequence == null)
			{
				Transition();
			}

			if(willTransition)
			{
				willTransition = false;
				DoTransition();
			}

			PlayNext();
		}
	}

	//Debug pls delete me when done
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.M))
			Transition();
	}

	/// <summary>
	/// Plays the next clip in the current sequence.
	/// </summary>
	private void PlayNext ()
	{
		if(currentSequence == null || currentSequence.clips.Length == 0)
			return;

		sequenceIndex = (sequenceIndex+1)%currentSequence.clips.Length;

		_currentAudioSource.clip = currentSequence.clips[sequenceIndex];
		_currentAudioSource.Play();
	}

	/// <summary>
	/// Transition to current sequence's nextSequence at the end of the current clip.
	/// </summary>
	public void Transition ()
	{
		willTransition = true;
		nextSequence = currentSequence.nextSequence;
	}

	/// <summary>
	/// Transition the argument sequence at the end of the current clip.
	/// </summary>
	public void Transition (AudioClipSequence sequence)
	{
		willTransition = true;
		nextSequence = sequence;
	}

	private void DoTransition ()
	{
		currentSequence = nextSequence;
		sequenceIndex = -1;
		nextSequence = null;
	}
	
	public void StopBGM ()
	{
		_currentAudioSource.Stop();
	}
}
