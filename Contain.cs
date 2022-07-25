using System.Collections.Generic;
using UnityEngine;

public class Contain : MonoBehaviour
{
    [SerializeField] private int _containId;
    [SerializeField] private int _startCapacity;
    [SerializeField] private bool _isAutoSaving;
    [SerializeField] private List<InventorySlot> _contains = new List<InventorySlot>();

    [SerializeField] private int _capacity;
    [SerializeField] private const int _maxCapacity = 64;

    public bool AddItem(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in _contains)
        {
            if (slot.item.Id == item.Id)
            {
                // заполнение слотов где есть предмет
                while (slot.amount < slot.item.MaxSizeSlot && amount > 0)
                {
                    slot.amount += 1;
                    amount -= 1;
                }
                if (amount <= 0) return true;
                else continue;
            }
        }
        if (IsFull()) return false;
        InventorySlot new_slot = new InventorySlot(item, amount, _contains.Count);
        _contains.Add(new_slot);
        if (_isAutoSaving) SaveContain();
        return true;
    }
    public void DeleteItemById(int id, int amount)
    {
        int countDeleting = amount;
        InventorySlot deletingSlot = null;
        foreach (InventorySlot slot in _contains)
        {
            if (slot.item.Id == id)
            {
                if (slot.amount == countDeleting)
                {
                    deletingSlot = slot;
                    break;
                }
                else if (slot.amount < countDeleting)
                { deletingSlot = slot; countDeleting -= slot.amount; }
                else if (slot.amount > countDeleting)
                {
                    slot.amount -= countDeleting;
                    break;
                }

            }
        }
        if (deletingSlot != null) _contains.Remove(deletingSlot);
        if (_isAutoSaving) SaveContain();
    }
    public void DeleteItemByNum(int slotNum, int amount)
    {
        int countDeleting = amount;
        InventorySlot deletingSlot = null;
        foreach (InventorySlot slot in _contains)
        {
            if (slot.SlotNum == slotNum)
            {
                if (slot.amount == countDeleting)
                {
                    deletingSlot = slot;
                    break;
                }
                else if (slot.amount < countDeleting)
                { deletingSlot = slot; countDeleting -= slot.amount; }
                else if (slot.amount > countDeleting)
                {
                    slot.amount -= countDeleting;
                    break;
                }

            }
        }
        if (deletingSlot != null) _contains.Remove(deletingSlot);
        if (_isAutoSaving) SaveContain();
    }
    public InventorySlot GetSlotByNum(int num)
    {
        return _contains[num - 1];
        /*foreach (InventorySlot slot in _contains)
        {
            if (slot.SlotNum == num) return slot; else continue;
        }
        return null;*/
    }
    public InventorySlot GetSlotById(int num)
    {
        foreach (InventorySlot slot in _contains)
        {
            if (slot.item.Id == num) return slot; else continue;
        }
        return null;
    }

    public void SaveContain()
    {
        _activeSlot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < _capacity; i++)
        {
            int id = _contains[i].item.Id;
            int amount = _contains[i].amount;
            PlayerPrefs.SetInt("itemId_" + _activeSlot + "_slot_" + _containId + "_contain_" + i + "_num", id);
            PlayerPrefs.SetInt("itemAmount_" + _activeSlot + "_slot_" + _containId + "_contain_" + i + "_num", amount);
        }
    }
    private bool _isFirstLoad = true;
    private int _activeSlot;
    private ItemDatabase itemDatabase;
    public void LoadContain()
    {
        if (_isFirstLoad) { FirstLoad(); }
        _activeSlot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < _maxCapacity; i++)
        {
            if (PlayerPrefs.HasKey("itemId_" + _activeSlot + "_slot_" + _containId + "_contain_" + i + "_num"))
            {
                int id = PlayerPrefs.GetInt("itemId_" + _activeSlot + "_slot_" + _containId + "_contain_" + i + "_num");
                int amount = PlayerPrefs.GetInt("itemAmount_" + _activeSlot + "_slot_" + _containId + "_contain_" + i + "_num");
                Item item = itemDatabase.GetItem(id);
                AddItem(item, amount);

            }
            else continue;
        }
    }
    private void FirstLoad()
    {
        _isFirstLoad = false;
        BasePrefs basePrefs = BasePrefs.instance;
        itemDatabase = basePrefs.GetItemDatabase;
    }
    public bool IsFull()
    {
        if (_contains.Count >= _capacity) { Debug.Log("Inventory FUll"); return true; }
        else return false;
    }
    public static void ClearAllContainsInSlot(int numSlot = -1)
    {
        int activeSlot = 0;
        if (numSlot == -1) activeSlot = PlayerPrefs.GetInt("activeSlot");
        else activeSlot = numSlot;
        for (int containId = 0; containId < 10; containId++)
        {
            for (int itemId = 0; itemId < _maxCapacity; itemId++)
            {
                PlayerPrefs.DeleteKey("itemId_" + activeSlot + "_slot_" + containId + "_contain_" + itemId + "_num");
                PlayerPrefs.DeleteKey("itemAmount_" + activeSlot + "_slot_" + containId + "_contain_" + itemId + "_num");
            }
        }
    }
}
