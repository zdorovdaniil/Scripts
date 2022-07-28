using UnityEngine;

public class CollectItem : Photon.MonoBehaviour
{
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_item == null) DestroyObject();
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                if (inventory.AddItems(_item, _amount))
                {
                    PlayerQuest.instance.UpdateProcessQuests(null, _item);
                    LogUI.Instance.Loger("Take drop: " + _item.Name);
                }
                DestroyObject();
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    public void DestroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
        /*
        if (!PhotonNetwork.offlineMode) {}
        else { Destroy(gameObject); }*/
    }

    public void SetData(Item item, int amount)
    {
        _item = item;
        _amount = amount;
    }
}
