using UnityEngine;

public class DoorBlock : MonoBehaviour
{
    [SerializeField] private GameObject _doorObj;
    [SerializeField] private bool _isBlockOnStart;
    [SerializeField] private bool _isActiveDoor;
    [SerializeField] private bool _isEnterPlayerInDoor; public bool IsEnterPlayerInDoor => _isEnterPlayerInDoor;
    [SerializeField] private bool _enableTriggerTeleporter = true;
    [SerializeField] Transform _trigerTeleporter;

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
    public void ResetTriggerZone()
    {
        if (_trigerTeleporter)
        {
            _trigerTeleporter.gameObject.GetComponent<TriggerZone>().Reset();
        }
    }
    public void SwitchDoor(bool status, bool avaibleTriggerTeleporter = true)
    {
        _enableTriggerTeleporter = avaibleTriggerTeleporter;
        _isActiveDoor = status;
        CheckStatusDoor();
    }
    private void CheckStatusDoor()
    {
        _doorObj.SetActive(_isActiveDoor);
        if (_trigerTeleporter) _trigerTeleporter.gameObject.SetActive(_enableTriggerTeleporter);
    }

}
