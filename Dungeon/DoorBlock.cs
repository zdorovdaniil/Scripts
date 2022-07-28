using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlock : MonoBehaviour
{
	[SerializeField] private GameObject _doorObj;
	[SerializeField] private bool _isBlockOnStart;
	[SerializeField] private bool _isActiveDoor;
	[SerializeField] private bool _isEnterPlayerInDoor; public bool IsEnterPlayerInDoor => _isEnterPlayerInDoor;

	public bool IsActiveDoor => _isActiveDoor;

	private void Start()
	{
		if (_isBlockOnStart) { SwitchDoor(true); }
		else SwitchDoor(false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			_isEnterPlayerInDoor = true;
		}
	}
	public void SwitchDoor(bool status)
	{
		_isActiveDoor = status;
		CheckStatusDoor();
	}
	private void CheckStatusDoor()
	{
		_doorObj.SetActive(_isActiveDoor);
		
	}

}
