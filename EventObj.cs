using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventObj : Photon.MonoBehaviour
{
    [SerializeField] private BuffClass _buff; public BuffClass GetBuff => _buff;
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
                guiControl.UseButton.GetComponent<ButtonUse>().Activate(_buff);
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
    [PunRPC]
    public void Activate()
    {
        _isActivated = true;
        onEventExe.Invoke();

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
