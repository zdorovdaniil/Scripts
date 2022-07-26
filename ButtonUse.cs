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
        if (_eventItem.IsForAllPlayers)
        {

        }
        else
        {
            _playerStats.AddBuff(_eventItem.GetBuff);
        }
    }


}
