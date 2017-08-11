using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameObjectPool : MonoBehaviour {
	[SerializeField]
	private GameObject prefab = null;
	[SerializeField]
	private int initialSize = 0;
	[SerializeField]
	private string containerName = "";
	[SerializeField]
	private bool growAsNeeded = true;
	
	private List<GameObject> pool;
	private GameObject containerObject;

	public string PooledObjectName 
	{
		get { return prefab.transform.name; }
	}


	public void Awake () {
		
		pool = new List<GameObject>();
		
		containerObject = new GameObject();
		
		containerObject.transform.name = containerName == ""?(prefab.transform.name + "Pool"):containerName;

		for(int i = 0; i < initialSize; i++)
		{
			GrowPool();
		}
	}

	public GameObject GetObjectFromPool() {

		for(int i = 0; i < pool.Count; i++)
		{
			if(!pool[i].activeInHierarchy)
			{
				pool[i].SetActive(true);
				return pool[i];
			}
		}

		if(growAsNeeded)
			return GrowPool();
		else
			return null;
	}

	public void ReturnObjectToPool(GameObject gameObject) {

		if(gameObject.transform.name != prefab.transform.name)
			Debug.Log("Nonmatching object returned to pool!");

		gameObject.SetActive(false);
		gameObject.transform.parent = containerObject.transform;
	}

	public void GrowPoolBy(int count) {
		
		for(int i = 0; i < count; i++)
			GrowPool();
	}

	private GameObject GrowPool() {
		if(prefab != null)
		{
			GameObject newObject = (GameObject)GameObject.Instantiate(prefab);
			newObject.transform.name = prefab.transform.name;
			newObject.transform.parent = containerObject.transform;
			newObject.SetActive(false);
			pool.Add(newObject);

			return newObject;
		}
		else
			return null;
	}

	public void DestroyPool () {

		for(int i = 0; i < pool.Count; i++)
		{
			Destroy(pool[i]);
		}

		Destroy(containerObject);

		pool.Clear();
	}

	private void OnDestroy() {
		DestroyPool();
	}
}

/*
 * 
 * Inspired from the code below
 * 
 * Also contains a cool example of serialized member class.

[AddComponentMenu("Gameplay/ObjectPool")]
public class GameObjectPool : MonoBehaviour {
	
	public static GameObjectPool instance { get; private set; }
	
	#region member
	/// <summary>
	/// Member class for a prefab entered into the object pool
	/// </summary>
	[Serializable]
	public class ObjectPoolEntry {
		/// <summary>
		/// the object to pre instantiate
		/// </summary>
		[SerializeField]
		public GameObject Prefab;
		
		/// <summary>
		/// quantity of object to pre-instantiate
		/// </summary>
		[SerializeField]
		public int InitialCount;
		
		[HideInInspector]
		public GameObject[] pool;
		
		[HideInInspector]
		public int objectsInPool = 0;
	}
	#endregion
	
	/// <summary>
	/// The object prefabs which the pool can handle
	/// by The amount of objects of each type to buffer.
	/// </summary>
	public ObjectPoolEntry[] Entries;
	
	/// <summary>
	/// The pooled objects currently available.
	/// Indexed by the index of the objectPrefabs
	/// </summary>
	
	/// <summary>
	/// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
	/// </summary>
	protected GameObject ContainerObject;

	// Use this for initialization
	void Start()
	{
		if(!instance)
		{
			instance = this;
			instance.ContainerObject = new GameObject("GameObjectPool");
		}
		
		//Loop through the object prefabs and make a new list for each one.
		//We do this because the pool can only support prefabs set to it in the editor,
		//so we can assume the lists of pooled objects are in the same order as object prefabs in the array
		
		for (int i = 0; i < Entries.Length; i++)
		{
			//Check instance.Entries for other entries with the same Prefab
			for (int u = 0; u < instance.Entries.Length; u++)
			{
				if (instance.Entries[u].Prefab.name == Entries[i].Prefab.name)
				{
					int newPoolSize = instance.Entries[u].pool.Length + Entries[i].InitialCount;
					Array.Resize(ref instance.Entries[u].pool, newPoolSize);
				}
			}

			ObjectPoolEntry objectPrefab = Entries[i];
			
			//create the repository
			if(instance == this)
				objectPrefab.pool = new GameObject[objectPrefab.InitialCount];
			
			//fill it                      
			for (int n = 0; n < objectPrefab.InitialCount; n++)
			{
				GameObject newObj = (GameObject)Instantiate(objectPrefab.Prefab);
				newObj.name = objectPrefab.Prefab.name;
				GameObjectPool.PoolObject(newObj);
			}

			foreach(ObjectPoolEntry entry in instance.Entries)
			{
				Debug.Log(entry.Prefab.transform.name + " has " + entry.objectsInPool + " objectsInPool");
				Debug.Log(entry.Prefab.transform.name + " has " + entry.pool.Length + " pool.Length");
			}
		}
	}
	
	
	
	/// <summary>
	/// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
	/// then null will be returned.
	/// </summary>
	/// <returns>
	/// The object for type.
	/// </returns>
	/// <param name='objectType'>
	/// Object type.
	/// </param>
	/// <param name='onlyPooled'>
	/// If true, it will only return an object if there is one currently pooled.
	/// </param>
	public static GameObject GetObjectForType(string objectType, bool onlyPooled)
	{
		
		for (int i = 0; i < instance.Entries.Length; i++)
		{
			GameObject prefab = instance.Entries[i].Prefab;
			
			if (prefab.name != objectType)
				continue;
			
			if (instance.Entries[i].objectsInPool > 0)
			{
				
				GameObject pooledObject = instance.Entries[i].pool[--instance.Entries[i].objectsInPool];
				pooledObject.transform.parent = null;
				pooledObject.SetActive(true);
				
				return pooledObject;
			} else if (!onlyPooled)
			{
				GameObject obj = (GameObject)Instantiate(instance.Entries[i].Prefab);
				obj.name = obj.name;
				return obj;
			}
		}
		
		//If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
		return null;
	}
	
	/// <summary>
	/// Pools the object specified.  Will not be pooled if there is no prefab of that type.
	/// </summary>
	/// <param name='obj'>
	/// Object to be pooled.
	/// </param>
	public static void PoolObject(GameObject obj)
	{
		
		for (int i = 0; i < instance.Entries.Length; i++)
		{
			if (instance.Entries[i].Prefab.name != obj.name)
				continue;

			Debug.Log(instance.ContainerObject.transform);

			obj.SetActive(false);
			obj.transform.parent = instance.ContainerObject.transform;

			if (obj.rigidbody != null) {
				obj.rigidbody.velocity = Vector3.zero;
			}
			
			instance.Entries[i].pool[instance.Entries[i].objectsInPool++] = obj;
			return;
		}
		Destroy(obj);
	}
}
*/