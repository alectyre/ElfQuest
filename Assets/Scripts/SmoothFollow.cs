using UnityEngine;
using System.Collections;



public class SmoothFollow : MonoBehaviour
{
	public Transform target;
	public float smoothDampTime = 0.2f;
	[HideInInspector]
	public new Transform transform;
	public Vector3 cameraOffset;
	public float minY;
	public float maxY;
	public bool useFixedUpdate = false;
	
	private CharacterController2D _playerController;
	private Vector3 _smoothDampVelocity;
	

	public void Reattach(Transform target)
	{
		this.target = target;
		_playerController = target.GetComponent<CharacterController2D>();
	}

	void Awake()
	{
		transform = gameObject.transform;
		_playerController = target.GetComponent<CharacterController2D>();
	}
	
	
	void LateUpdate()
	{
		if( !useFixedUpdate )
			updateCameraPosition();
	}


	void FixedUpdate()
	{
		if( useFixedUpdate )
			updateCameraPosition();
	}


	void updateCameraPosition()
	{
		if( _playerController == null )
		{
			return;
		}
		
		if( _playerController.velocity.x > 0 )
		{
			Vector3 newPos = target.position - cameraOffset;
			newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
			transform.position = Vector3.SmoothDamp( transform.position, newPos, ref _smoothDampVelocity, smoothDampTime );
		}
		else
		{
			var leftOffset = cameraOffset;
			leftOffset.x *= -1;
			Vector3 newPos = target.position - leftOffset;
			newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
			transform.position = Vector3.SmoothDamp( transform.position, newPos, ref _smoothDampVelocity, smoothDampTime );
		}
	}
	
}
