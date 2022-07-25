using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUse : MonoBehaviour
{
    private PlayerStats _playerStats;
    private EventItem _eventItem;
    public void SetPlayer(PlayerStats player, EventItem eventItem)
    {
        _playerStats = player;
        _eventItem = eventItem;

    }
    public void RemovePlayer()
    {
        _playerStats = null;
        _eventItem = null;
        this.gameObject.SetActive(false);

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
