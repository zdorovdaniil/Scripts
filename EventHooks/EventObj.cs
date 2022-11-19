using UnityEngine;
using UnityEngine.Events;

public class EventObj : Photon.MonoBehaviour
{
    [SerializeField] private BuffClass _buff;
    [SerializeField] private Item _item;
    [SerializeField] private bool _forAllPlayers; public bool IsForAllPlayers => _forAllPlayers;
    [SerializeField] private bool _isActivated = false;
    [SerializeField] private UnityEvent onEventExe;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().isMine && !_isActivated)
            {
                GUIControl guiControl = other.GetComponent<PlayerLinks>().GetGUIControl;
                guiControl.UseButton.SetActive(true);
                guiControl.UseButton.GetComponent<ButtonUse>().Activate(this);
            }
            else if (other.gameObject.GetPhotonView().isMine && _isActivated)
            {
                CloseGUIButton(other);
            }
        }
    }
    public void UseItemOnPlS(PlayerStats playerStats, Item item)
    {
        InventorySlot invSlot = new InventorySlot(item);
        item.UsingItem(playerStats, invSlot, null, null);
        Debug.Log("Use item: " + item.name + " onEvent");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().isMine)
            {
                CloseGUIButton(other);
            }
        }
    }
    private void CloseGUIButton(Collider other)
    {
        GUIControl guiControl = other.GetComponent<PlayerLinks>().GetGUIControl;
        guiControl.UseButton.GetComponent<ButtonUse>().Remove();
    }
    public void SendToActivate(PlayerStats playerStats)
    {
        if (IsForAllPlayers)
        {
            if (_buff) GameManager.Instance.SendAllBuff(_buff.BuffId);
        }
        else
        {
            if (_buff) playerStats.AddBuffPlayer(_buff.BuffId);
            if (_item) UseItemOnPlS(playerStats, _item);
        }
        if (!PhotonNetwork.offlineMode)
            photonView.RPC("Activate", PhotonTargets.All);
        else Activate();
    }
    [PunRPC]
    public void Activate()
    {
        GlobalSounds.Instance.PEventFound(this.transform);
        _isActivated = true;
        onEventExe.Invoke();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
