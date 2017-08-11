using UnityEngine;

public class PlayerRespawnerScript : MonoBehaviour {

	public float respawnDelay;
	public GameObject player;
	public string playerLayer;
	public int maxBufferSize;
	public float updateFrequency;
	public float respawnInvincibility;
	public bool requireGrounded;

	public bool debug;

	private Vector3[] locationBuffer;

	private int currentIndex = 0;
	private float timeUntilNextUpdate;

	private CharacterController2D _characterController;

	void Start ()
	{
		locationBuffer = new Vector3[maxBufferSize];
		ClearBuffer();
		timeUntilNextUpdate = updateFrequency;

		_characterController = player.GetComponent<CharacterController2D>();
	}

	void FixedUpdate () 
	{
		if(timeUntilNextUpdate > 0)
		{
			timeUntilNextUpdate -= Time.deltaTime;
		}
		else
		{

			if(!player)
			{
				Debug.Log("PlayerRespawner: No player set.");
				return;
			}

			if(!player.activeSelf)
				Invoke("RespawnPlayer", respawnDelay);
			else
			{
				if(!(requireGrounded && !_characterController.isGrounded)) //This blew my mind
				{
					currentIndex++;

					locationBuffer[currentIndex%maxBufferSize] = player.transform.position;
				}
			}

			timeUntilNextUpdate = updateFrequency;
		}

		if(debug)
			DebugScript.DrawCross(locationBuffer[(currentIndex+1)%maxBufferSize], 0.5f, Color.green);
	}


	void RespawnPlayer ()
	{
		player.GetComponent<SpriteRenderer> ().sortingLayerName = playerLayer;
		player.transform.position = locationBuffer[(currentIndex+1)%maxBufferSize];
		ClearBuffer();
		player.GetComponent<PlayerHealthScript>().Invincible(2.0f);
		player.GetComponent<PlayerHealthScript>().FullHeal();
		player.GetComponent<Collider2D>().enabled = true;
		player.SetActive(true);
				Debug.Log ("respawning");
	}

	void ClearBuffer ()
	{
		Vector3 pos = player.transform.position;

		for(int i = 0; i < locationBuffer.Length; i++)
			locationBuffer[i] = pos;
	}
}
