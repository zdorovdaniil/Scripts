using System.Collections.Generic;
using UnityEngine;

public class CollectItems : Photon.MonoBehaviour
{
    [SerializeField] private List<Item> _items;
    private bool _isPlayerEnter = false;
    [SerializeField] private ItemDatabase itemDatabase;
    private void Start()
    {
        itemDatabase = BasePrefs.instance.GetItemDatabase;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!_isPlayerEnter)
            {
                _isPlayerEnter = true;
                if (_items.Count <= 0) return;
                Inventory inventory = other.GetComponent<Inventory>();
                foreach (Item item in _items)
                {
                    if (inventory.AddItems(item, 1))
                    {
                        PlayerQuest.instance.UpdateProcessQuests(null, item);
                    }
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
    }
    public void AddItem(Item item)
    {

        if (!PhotonNetwork.offlineMode)
        {
            photonView.RPC("AddItemWithId", PhotonTargets.AllBuffered, (int)item.Id);
        }
        else
        {
            _items.Add(item);
        }

    }

    [PunRPC]
    public void AddItemWithId(int idItem)
    {
        itemDatabase = BasePrefs.instance.GetItemDatabase;
        if (itemDatabase != null) _items.Add(itemDatabase.GetItem(idItem));
        else Debug.LogError("Database == null!");

    }
}
