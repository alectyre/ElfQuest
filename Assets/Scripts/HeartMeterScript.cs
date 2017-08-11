using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HeartMeterScript : MonoBehaviour {

	[SerializeField]
	private GameObject heartPrefab = null;
	[SerializeField]
	private Sprite fullHeartSprite = null;
	[SerializeField]
	private Sprite halfHeartSprite = null;
	[SerializeField]
	private Vector2 spaceBetweenHearts = Vector2.zero;
	[SerializeField]
	private int pulseThresholdInHalves = 0;

	private List<GameObject> heartPool;

	private Animator _animator;

	private int heartHalves;


	void Awake () {
		Destroy(transform.Find("Heart").gameObject);
		
		heartPool = new List<GameObject>();
		
		_animator = GetComponent<Animator>();
	}

	private GameObject LastActiveHeart
	{
		get
		{  
			GameObject lastActiveHeart = null;
			
			for(int i = heartPool.Count-1; i >= 0; i--)
			{
				if(heartPool[i].activeSelf)
				{
					lastActiveHeart = heartPool[i];
					break;
				}
			}

			return lastActiveHeart;
		}
	}

	private GameObject FirstInactiveHeart
	{
		get
		{
			GameObject firstActiveHeart = null;
			
			for(int i = 0; i < heartPool.Count; i++)
			{
				if(!heartPool[i].activeSelf)
				{
					firstActiveHeart = heartPool[i];
					break;
				}
			}

			return firstActiveHeart;
		}
	}
	
	private GameObject CreateNewHeart ()
	{
		GameObject newHeart = (GameObject)Instantiate(heartPrefab);
		newHeart.transform.name = "Heart" + heartPool.Count;
		newHeart.transform.parent = transform;
		
		Vector3 newHeartPos = Vector3.zero;
		
		newHeartPos.x = (spaceBetweenHearts.x + heartPrefab.transform.localScale.x) * heartPool.Count;
		
		newHeartPos.z = heartPrefab.transform.position.z;
		
		newHeart.transform.localPosition = newHeartPos;
		
		heartPool.Add(newHeart);
		
		return newHeart;
	}

	public void AddHalfHeart ()
	{
		GameObject heart = LastActiveHeart;

		if(heart == null || heart.GetComponent<SpriteRenderer>().sprite == fullHeartSprite)
		{
			heart = FirstInactiveHeart;

			if(heart == null)
			{
				heart = CreateNewHeart();
				heart.GetComponent<SpriteRenderer>().sprite = halfHeartSprite;
			}
			else
			{
				heart.SetActive(true);
				heart.GetComponent<SpriteRenderer>().sprite = halfHeartSprite;
			}
		}
		else
		{
			heart.GetComponent<SpriteRenderer>().sprite = fullHeartSprite;
		}

		heartHalves++;

		if(heartHalves<=pulseThresholdInHalves)
		{
			_animator.SetBool("pulse", true);
		}
		else
		{
			_animator.SetBool("pulse", false);
		}
	}

	public void RemoveHalfHeart ()
	{
		GameObject heart = LastActiveHeart;
		
		if(heart != null)
		{
			if(heart.GetComponent<SpriteRenderer>().sprite == fullHeartSprite)
			{
				heart.GetComponent<SpriteRenderer>().sprite = halfHeartSprite;
			}
			else
			{
				heart.SetActive(false);
			}
		}

		heartHalves--;

		if(heartHalves<0)
			heartHalves = 0;

		if(heartHalves<=pulseThresholdInHalves)
		{
			_animator.SetBool("pulse", true);
		}
		else
		{
			_animator.SetBool("pulse", false);
		}
	}

	public void AddHeart ()
	{
		AddHalfHeart();
		AddHalfHeart();
	}
	
	public void RemoveHeart ()
	{
		RemoveHalfHeart();
		RemoveHalfHeart();
	}

	public void RemoveAllHearts()
	{
		for(int i = 0; i < heartPool.Count; i++)
		{
			heartPool[i].SetActive(false);
		}
	}
}
