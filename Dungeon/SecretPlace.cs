using UnityEngine;
using UnityEngine.Events;

public class SecretPlace : Photon.MonoBehaviour
{
    /// <summary>
    /// чем меньше значение _secretModif, чем меньше вероятность активация события
    /// </summary>
    [SerializeField] private float _secretModif = 1f;
    [SerializeField] private UnityEvent _onEventExe;
    [SerializeField] private bool _isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetPhotonView().isMine && !_isActivated)
            {
                int skillValue = other.gameObject.GetComponent<PlayerStats>().stats.Skills[0].Level;
                if (IsProcChancePlayer(skillValue)) SendToActivate();
                _isActivated = true;
            }
        }
    }
    private bool IsProcChancePlayer(int skillValue)
    {
        if (skillValue <= 0) { Debug.Log("Secret was LOST: "); return false; }
        else
        {
            int randomValue = Random.Range(0, 100);
            float chance = skillValue * 2.5f * _secretModif;
            if (randomValue <= chance) { Debug.Log("Secret FOUND with chance: " + chance + " of random: " + randomValue); return true; }
            else { Debug.Log("Secret was LOST: " + chance + " of random: " + randomValue); return false; }
        }
    }
    public void SendToActivate()
    {
        if (!PhotonNetwork.offlineMode)
            photonView.RPC("Activate", PhotonTargets.All);
        else Activate();
    }
    [PunRPC]
    public void Activate()
    {
        GlobalSounds.Instance.PEventFound(this.transform);
        _isActivated = true;
        _onEventExe.Invoke();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
