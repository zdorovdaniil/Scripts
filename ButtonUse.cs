using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUse : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _buttonObj;
    private EventObj _eventObj;
    public void Activate(EventObj eventObj)
    {
        _buttonObj.SetActive(true);
        _eventObj = eventObj;
    }
    public void Remove()
    {
        _buttonObj.SetActive(false);
    }
    public void ClickUse()
    {
        if (_eventObj.IsForAllPlayers)
        {
            GameManager.Instance.SendAllBuff(_eventObj.GetBuff.BuffId);
        }
        else
        {
            _playerStats.AddBuff(_eventObj.GetBuff.BuffId);
        }
        if (!PhotonNetwork.offlineMode)
            _eventObj.photonView.RPC("Activate", PhotonTargets.All);
        else _eventObj.Activate();
        Remove();
    }


}
