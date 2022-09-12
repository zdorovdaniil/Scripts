using UnityEngine;
using UnityEngine.Events;

public class EventObj : Photon.MonoBehaviour
{
    [SerializeField] private BuffClass _buff;
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
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().isMine)
            {
                GUIControl guiControl = other.GetComponent<PlayerLinks>().GetGUIControl;
                guiControl.UseButton.GetComponent<ButtonUse>().Remove();
            }
        }
    }
    public void SendToActivate(PlayerStats playerStats)
    {
        if (IsForAllPlayers)
        {
            GameManager.Instance.SendAllBuff(_buff.BuffId);
        }
        else
        {
            playerStats.AddBuffPlayer(_buff.BuffId);
        }
        if (!PhotonNetwork.offlineMode)
            photonView.RPC("Activate", PhotonTargets.All);
        else Activate();
    }
    [PunRPC]
    public void Activate()
    {
        _isActivated = true;
        onEventExe.Invoke();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
