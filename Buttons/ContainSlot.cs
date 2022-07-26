﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainSlot : MonoBehaviour
{
    [SerializeField] private InventorySlot _Slot;
    [SerializeField] private Image _imageSlot;
    [SerializeField] private Image _backgroungSlot;
    [SerializeField] private TextMeshProUGUI _amountItems;
    [SerializeField] private ContainerType _containerType;
    [SerializeField] private Inventory _inv;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Sprite _nullImage;


    public void UpdateSlotInfo(InventorySlot slot, Inventory inv, ContainerType containerType)
    {
        _inv = inv;
        _containerType = containerType;
        _Slot = slot;
        _imageSlot.sprite = _Slot.item.Icon;
        int level = _Slot.item.Level;
        _backgroungSlot.sprite = BasePrefs.instance.GetIcon(level - 1);

        if (_containerType == ContainerType.CraftingInfo || _containerType == ContainerType.LevelingInfo)
        {
            if (_inv)
            {
                int haveCount = _inv.GetCountItemsWithId(slot.item.Id);
                int needCount = slot.amount;
                _description.text = haveCount.ToString() + " / " + needCount.ToString();
                _amountItems.text = "";
            }
        }
        else if (slot.amount > 1)
        { _amountItems.text = slot.amount.ToString(); ; }
        else { _amountItems.text = ""; }
    }
    public void ClearClot()
    {
        _imageSlot.sprite = _nullImage;
        _backgroungSlot.sprite = _nullImage;
        _description.text = "";
        _amountItems.text = "";
        _containerType = ContainerType.None;
    }
    public void ClickOnSlot()
    {
        switch (_containerType)
        {
            case ContainerType.CraftingInfo: CraftingUI.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.None); break;
            case ContainerType.Crafting: CraftingUI.instance.FillSecondUI(_Slot); break;
            case ContainerType.Shop: ShopingUI.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Sell); break;
            case ContainerType.ShopInfo: ShopingUI.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Buy); break;
            case ContainerType.Storage: Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Move); break;
            case ContainerType.StorageInfo: Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Get); break;
            case ContainerType.Equip:
                if (Storage.instance != null)
                { Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Equip); }
                else PlayerUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Equip); break;
            case ContainerType.LevelingInfo: PlayerUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.None); break;
            case ContainerType.Inventory: PlayerUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Inventory); break;
            case ContainerType.Chest: ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.ToChest); break;
            case ContainerType.ChestInfo: ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.TakeFromChest); break;
            case ContainerType.Info: ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.TakeFromChest); break;

        }
        switch (_Slot.item.itemTupe)
        {
            case ItemTupe.Poison: GlobalSounds.Instance.SClickGlass(); break;
            case ItemTupe.BookImprove: GlobalSounds.Instance.SClickPaper(); break;
            case ItemTupe.Usable: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.Armor: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.Weapon: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.Food: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.Improve: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.MonsterDrop: GlobalSounds.Instance.SClickItem(); break;
            case ItemTupe.Material: GlobalSounds.Instance.SClickItem(); break;
        }
    }
}

