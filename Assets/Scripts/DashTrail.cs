using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Enable Sprite Batching for better performance.
/// In the texture inport settings give the sprite a packing tag
/// Edit > Project Settings > Editor > Sprite Packer Mode to 'Always Enabled'
/// </summary>
public class DashTrail : MonoBehaviour
{
	public SpriteRenderer leadingSprite;
	public float spawnInterval;
	public float trailTime;
	public GameObject trailObject;
	public Color startColor = Color.white;
	public Color endColor = Color.white;
	public bool recolorExisting;
	[SerializeField] private bool _drawTrail = true;

	public bool drawTrail 
	{
		set
		{
			if(value != _drawTrail)
				spawnTimer = spawnInterval;
			_drawTrail = value;
		}

		get { return _drawTrail; }
	}

	private float spawnTimer;

	private List<GameObject> trailObjectPool;

	// Use this for initialization
	void Start ()
	{
		trailObjectPool = new List<GameObject> ();

		int trailSegments = Mathf.FloorToInt(trailTime / spawnInterval);

		for (int i = 0; i < trailSegments; i++)
		{
			GameObject trail = GameObject.Instantiate (trailObject);
			trail.transform.SetParent (transform);
			trailObjectPool.Add(trail);
		}
	}

	public void OnEnable()
	{
		spawnTimer = spawnInterval;
	}

	// Update is called once per frame
	void Update ()
	{
		if(_drawTrail)
		{
			spawnTimer += Time.deltaTime;

			//TODO probably should spawn by distance traveled?
			if(spawnTimer >= spawnInterval)
			{
				GameObject trail = GetPooledTrailObject();

				DashTrailObject trailObject = trail.GetComponent<DashTrailObject>();

				trailObject.Initiate(trailTime, leadingSprite.sprite, transform.position, this);

				spawnTimer = 0;

				trailObject.GetComponent<SpriteRenderer>().sortingOrder = -1;

				for(int i = 0; i < trailObjectPool.Count; i++)
				{
					SpriteRenderer spriteRenderer = trailObjectPool[i].GetComponent<SpriteRenderer>();
					spriteRenderer.sortingOrder -= 1;
				}
			}

			if(recolorExisting)
			{
				for(int i = 0; i < trailObjectPool.Count; i++)
				{
					DashTrailObject trailObject = trailObjectPool[i].GetComponent<DashTrailObject>();

					trailObject.startColor = startColor;
					trailObject.endColor = endColor;
				}
			}
		}
	}

	private GameObject GetPooledTrailObject()
	{
		GameObject trail = null;

		for(int i = 0; i < trailObjectPool.Count; i++)
		{
			if(!trailObjectPool[i].activeSelf)
			{
				trail = trailObjectPool[i];
				break;
			}
		}

		if(trail == null)
		{
			trail = GameObject.Instantiate (trailObject);
			trail.transform.SetParent (transform);
			trailObjectPool.Add(trail);
		}

		return trail;
	}
}