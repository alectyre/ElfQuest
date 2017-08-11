using UnityEngine;
using System.Collections;

public class BushHealthScript : HealthScript {

	public string nameOfLeafPool;
	public GameObject dropOnKill;

	private BoxCollider2D _collider;
	private GameObjectPool _leafPool;

	GameObject leafs;
	
	private void Start ()
	{
		_collider = GetComponent<BoxCollider2D>();

		_leafPool = GameObject.Find(nameOfLeafPool).GetComponent<GameObjectPool>();

		if(_leafPool == null)
			Debug.Log("WTF no leafPool found. How will bushes die awesomely?");
	}

	public override void InflictDamage(float value)
	{
		currentHealth -= value;
		
		if(currentHealth <= 0) {
			Kill();
		}
	}
	
	public override void Kill() 
	{ 
		_collider.enabled = false;
		GetComponent<Renderer>().enabled = false;

		if(_leafPool != null)
		{
			leafs = _leafPool.GetObjectFromPool();

			leafs.transform.position = transform.position;
			leafs.transform.Rotate(new Vector3(0,0,Random.Range(0,360)));
			leafs.GetComponent<Animator>().Play("leafs_scattering");

			leafs.SetActive(true);
		}

		if(dropOnKill)
		{
			GameObject dropped = (GameObject)Instantiate(dropOnKill);
			dropped.transform.position = transform.position;
		}

		Invoke("RemoveBush", 1);
	}

	void RemoveBush ()
	{
		if(_leafPool != null)
			_leafPool.ReturnObjectToPool(leafs);

		Destroy(gameObject);
	}
}
