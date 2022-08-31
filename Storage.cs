using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Storage : MonoBehaviour
{
    public static Storage instance;
    [SerializeField] private List<InventorySlot> storage = new List<InventorySlot>();
    // id-шники загруженных предметов из сохранения
    [SerializeField] private int[] storageIDs = new int[40];
    [SerializeField] private int _startStorageCapacity = 20;
    public int StorageCapacity; // вместимость хранилища
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private SlotsUI _invSlots;
    [SerializeField] private SlotsUI _equipSlots;
    [SerializeField] private SlotsUI _storageSlots;
    [SerializeField] private Transform _spawnReferenceGUI;
    [SerializeField] private GameObject _prefReferenceGUI;
    [SerializeField] private Inventory _inv;
    public PlayerStats PlStats;

    [SerializeField] private Improve _improve;
    [SerializeField] private TMP_Text _countItemsInStorage;
    [SerializeField] private TMP_Text _textStorageCapacity;
    public void FillStorageSlotsUI()
    {
        ImproveInstance();
        LoadingStorage();
        _inv.SetEquipSlotFromItemsIDs();
        _equipSlots.FullSlots(_inv.GetEquipsList());
        _invSlots.FullSlots(_inv.GetItems);
        InstanceUI();
        _storageSlots.FullSlots(storage);
    }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        for (int i = 0; i < _spawnReferenceGUI.childCount; i++)
        {
            Destroy(_spawnReferenceGUI.GetChild(i).gameObject);
        }
        Instantiate(_prefReferenceGUI, _spawnReferenceGUI).GetComponent<ReferenceUI>().SetValueSlot(slot, buttonType);
    }
    //
    private void ImproveInstance()
    {
        _improve.LoadImprove();
        for (int i = 0; i < _improve.GetLvlValues.Count; i++)
            if (_improve.GetCurLvl == i) StorageCapacity = _improve.GetLvlValues[i];

    }
    private void InstanceUI()
    {
        _countItemsInStorage.text = storage.Count.ToString();
        _textStorageCapacity.text = StorageCapacity.ToString();
    }
    private void Start()
    {
        instance = this;
    }

    // перемещение экипировки в свободный слот для экипировки
    private void SelectArmor(InventorySlot _slot, int numE, int numI)
    {
        if (_inv.GetEquipSlot(numE) != null)
        { MsgBoxUI.Instance.ShowInfo("armor", "Slot busy"); return; }
        else if (_slot.item.CheckReqirement(PlStats))
        {
            _inv.SetToEquipSlot(_slot, numE);
            _inv.DeleteItemId(_slot.item.Id, 1);
        }
        else MsgBoxUI.Instance.ShowInfo("armor", "not requiment player stats");
    }
    public void Equip(InventorySlot slot)
    {
        if (slot.item.itemTupe == ItemTupe.Weapon)
        {
            if (_inv.GetEquipSlot(0) != null)
            { MsgBoxUI.Instance.ShowInfo("equip", "Slot busy ar not requiment player stats"); return; }
            else if (slot.item.CheckReqirement(PlStats))
            {
                _inv.SetToEquipSlot(slot, 0);
                _inv.DeleteItemId(slot.item.Id, 1);
            }
            else MsgBoxUI.Instance.ShowInfo("equip", "not requiment player stats");
        }
        if (slot.item.itemTupe == ItemTupe.Armor)
        {
            if (slot.item.armorTupe == ArmorTupe.Helmet) { SelectArmor(slot, 1, 25); }
            if (slot.item.armorTupe == ArmorTupe.Tors) { SelectArmor(slot, 2, 26); }
            if (slot.item.armorTupe == ArmorTupe.Pants) { SelectArmor(slot, 3, 27); }
            if (slot.item.armorTupe == ArmorTupe.Shoes) { SelectArmor(slot, 4, 28); }
            if (slot.item.armorTupe == ArmorTupe.Amulet) { SelectArmor(slot, 5, 29); }
            if (slot.item.armorTupe == ArmorTupe.Ring) { SelectArmor(slot, 6, 30); }
        }
        _inv.SerializeItemId();
        _inv.SaveItemsId();
        FillStorageSlotsUI();
    }
    public void MoveToStorageSlot(Item item, int amount)
    {
        if (AddStorage(item, amount))
        {
            //Debug.Log("Перемещение на склад предмета успешно!");
            _inv.DeleteItemId(item.Id, amount);
        }
        else MsgBoxUI.Instance.ShowInfo("storage", "inventory ii full");
        _inv.SaveItemsId();
        _invSlots.FullSlots(_inv.GetItems);
        SaveStorageId();
        FillStorageSlotsUI();
    }
    public void RemoveEquipSlot(Item item)
    {
        if (_inv.AddItems(item, 1))
        { _inv.DeleteEquipId(item.Id); }
        else MsgBoxUI.Instance.ShowInfo("storage", "inventory ii full");
        _inv.SaveItemsId();
        SaveStorageId();
        FillStorageSlotsUI();
    }
    public void MoveToInvFromStorage(Item item, int amount)
    {
        if (_inv.AddItems(item, amount))
        {
            DeleteItemId(item.Id, amount);
            _inv.SaveItemsId();
            _invSlots.FullSlots(_inv.GetItems);
        }
        else MsgBoxUI.Instance.ShowInfo("storage", "inventory ii full");
        SaveStorageId();
        FillStorageSlotsUI();

    }

    public void DeleteItemFromInvId(int id)
    {
        _inv.DeleteItemId(id, 1);
        FillStorageSlotsUI();
    }
    public void DeleteItemId(int id, int amount)
    {

        // количество предметов для удаления 
        int countDeleting = amount;
        InventorySlot deletingSlot = null;
        foreach (InventorySlot slot in storage)
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
        if (deletingSlot != null) storage.Remove(deletingSlot);
    }
    public void LoadingStorage()
    {
        ClearItemsInStorage();
        storage.Clear();
        LoadStorageId();
        SetStorageFromStorageIDs();
    }
    private void LoadStorageId()
    {
        int _slot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < StorageCapacity; i++)
        {

            if (PlayerPrefs.HasKey(_slot + "storageId_" + i))
            { storageIDs[i] = PlayerPrefs.GetInt(_slot + "storageId_" + i); }
        }
    }
    public void DeleteStorageItem(int i)
    {
        if (storage[i].item != null) { storage.RemoveAt(i); }
        else Debug.Log("Empty Slot");
    }
    public bool AddStorage(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in storage)
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
        if (storage.Count >= StorageCapacity) { Debug.Log("Inventory FUll"); return false; }
        InventorySlot new_slot = new InventorySlot(item, amount);
        storage.Add(new_slot);
        new_slot.SlotNum = storage.Count;
        return true;
    }
    public void SaveStorageId()
    {
        SerializeStorageId();
        int _slot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < StorageCapacity; i++)
        {
            PlayerPrefs.SetInt(_slot + "storageId_" + i, storageIDs[i]);
            if (GetStorageInventorySlot(i) != null)
            { PlayerPrefs.SetInt(_slot + "storage_" + i + "_amount", storage[i].amount); }
        }
    }
    public int GetSize()
    {
        return storage.Count;
    }
    public Item GetItem(int i)
    {
        return i < storage.Count ? storage[i].item : null;
    }
    public int GetAmount(int i)
    {
        return i < storage.Count ? storage[i].amount : 0;
    }
    public InventorySlot GetStorageInventorySlot(int i)
    {
        return i < storage.Count ? storage[i] : null;
    }
    private void SerializeStorageId()
    {
        for (int i = 0; i < StorageCapacity; i++)
        {
            int storageID = (GetStorageInventorySlot(i) != null) ? storage[i].item.Id : -1;
            storageIDs[i] = storageID;
        }
    }
    private void SetStorageFromStorageIDs()
    {

        int _slot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < StorageCapacity; i++)
        {
            int _amount = 1;
            Item item = itemDatabase.GetItem(storageIDs[i]);
            if (PlayerPrefs.HasKey(_slot + "storage_" + i + "_amount"))
            {
                _amount = PlayerPrefs.GetInt(_slot + "storage_" + i + "_amount");
            }
            if (item != null)
            {
                InventorySlot new_slot = new InventorySlot(item, _amount);
                storage.Add(new_slot);
                new_slot.SlotNum = storage.Count;
            }
        }
    }
    // очистка itemIDs в инвентаре
    private void ClearItemsInStorage()
    {
        for (int i = 0; i < 40; i++)
        {
            storageIDs[i] = 0;
        }
    }
}

