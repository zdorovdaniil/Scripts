using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventItem : MonoBehaviour
{
    [SerializeField] private BuffClass _buff; public BuffClass GetBuff => _buff;
    [SerializeField] private bool forAllPlayers; public bool IsForAllPlayers => forAllPlayers;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().isMine)
            {
                GUIControl guiControl = other.GetComponent<PlayerLinks>().GetGUIControl;
                guiControl.UseButton.SetActive(true);
                guiControl.UseButton.GetComponent<ButtonUse>().SetPlayer(other.gameObject.GetComponent<PlayerStats>(), this);
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
                guiControl.UseButton.GetComponent<ButtonUse>().RemovePlayer();
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
