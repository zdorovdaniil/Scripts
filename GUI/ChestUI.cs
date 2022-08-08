using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    public static ChestUI Instance;
    [SerializeField] private GameObject _chestUI;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private SlotsUI _chestSlotsUI;
    [SerializeField] private SlotsUI _inventorySlotsUI;
    [SerializeField] private SlotsUI _equipSlotsUI;
    [SerializeField] private Chest _curChest;
    [SerializeField] private Transform _buttonOpenChest;
    [SerializeField] private Transform _spawnReferenceGUI;
    [SerializeField] private GameObject _prefReferenceGUI;
    private GameObject _spawnedPreferanceGUI;
    private void Start() { Instance = this; }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        DestroyReferenceUI();
        Instantiate(_prefReferenceGUI, _spawnReferenceGUI).GetComponent<ReferenceUI>().SetValueSlot(slot, buttonType);
    }
    public void ResetCurChest()
    { _curChest = null; _buttonOpenChest.gameObject.SetActive(false); }
    public void SetCurChest(Chest curChest)
    { _curChest = curChest; _buttonOpenChest.gameObject.SetActive(true); }
    public void SwitchChestUI(bool status)
    {
        _curChest.ChangeMesh(status);
        _chestUI.SetActive(status);
        if (status) GlobalSounds.Instance.SOpenChest();
        else GlobalSounds.Instance.SCloseChest();
    }
    public void FillChestUI()
    {
        _chestSlotsUI.FullSlots(_curChest.GetChestSlot);
        _inventorySlotsUI.FullSlots(_inventory.GetItems);
        _equipSlotsUI.FullSlots(_inventory.GetEquipsList());
    }
    public void PlaceItemToChest(InventorySlot slot)
    {
        if (_curChest.AddItemToChest(slot.item, 1))
        { _inventory.DeleteItemId(slot.item.Id, 1); _inventory.SerializeItemId(); }
        else { MsgBoxUI.Instance.ShowAttention("chest full"); }
        FillChestUI();
    }
    public void PlaceAllItemsToInv()
    {
        DestroyReferenceUI();
        List<InventorySlot> chestSlots = new List<InventorySlot>(_curChest.GetChestSlot);
        foreach (InventorySlot slot in chestSlots)
        {
            if (_inventory.AddItems(slot.item, slot.amount))
            { _curChest.DeleteItemId(slot.item.Id, slot.amount); }
        }
        FillChestUI();
        GlobalSounds.Instance.STakeAll();
    }
    public void PlaceItemToInventory(InventorySlot slot)
    {
        if (_inventory.AddItems(slot.item, 1))
        { _curChest.DeleteItemId(slot.item.Id, 1); _inventory.SerializeItemId(); }
        else { MsgBoxUI.Instance.ShowAttention("inventory full"); }
        FillChestUI();
    }
    public void DestroyReferenceUI()
    {
        for (int i = 0; i < _spawnReferenceGUI.childCount; i++)
        { Destroy(_spawnReferenceGUI.GetChild(i).gameObject); }
    }
}
