using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBuff : MonoBehaviour
{
    [SerializeField] private BuffClass _buff; public BuffClass GetBuff => _buff;

    public void SendBuff()
    {
            TriggerZone triggerZone = GetComponent<TriggerZone>();
        foreach (PlayerStats plStats in triggerZone.GetPlayers)
        {
            plStats.AddBuff(_buff.BuffId);
        }
    }
}
