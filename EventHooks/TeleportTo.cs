using UnityEngine;
using System.Collections.Generic;

public class TeleportTo : MonoBehaviour
{
    [SerializeField] private Transform _posDestination;
    private float[] pos = new float[3];

    private void Start() { pos = ProcessCommand.TransformToFloat(_posDestination); }
    public void TeleportObjects(List<PlayerStats> playerStats)
    {
        foreach (PlayerStats plS in playerStats)
        {
            if (PhotonNetwork.offlineMode) plS.TeleportTo(pos);
            plS.photonView.RPC("TeleportTo", PhotonTargets.AllBuffered, (float[])pos);
        }
    }
    public void TriggerTeleport()
    {
        List<PlayerStats> playerStats = GetComponent<TriggerZone>().GetPlayers;
        foreach (PlayerStats plS in playerStats)
        {
            if (PhotonNetwork.offlineMode) plS.TeleportTo(pos);
            plS.photonView.RPC("TeleportTo", PhotonTargets.AllBuffered, (float[])pos);
        }
    }

}
