using TMPro;
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
            int haveCount = _inv.GetCountItemsWithId(slot.item.Id);
            int needCount = slot.amount;
            _description.text = haveCount.ToString() + " / " + needCount.ToString();
            _amountItems.text = "";
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
        if (_containerType == ContainerType.Crafting)
        {
            CraftingUI.instance.FillSecondUI(_Slot);
        }
        if (_containerType == ContainerType.Shop)
        {
            ShopingUI.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Sell);
        }
        if (_containerType == ContainerType.ShopInfo)
        {
            ShopingUI.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Buy);
        }
        if (_containerType == ContainerType.Storage)
        {
            Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Move);
        }
        if (_containerType == ContainerType.StorageInfo)
        {
            Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Get);
        }
        if (_containerType == ContainerType.Equip)
        {
            if (Storage.instance != null)
            { Storage.instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Equip); }
            else PlayerUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Equip);
        }
        if (_containerType == ContainerType.Inventory)
        {
            PlayerUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.Inventory);
        }
        if (_containerType == ContainerType.Chest)
        {
            ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.ToChest);
        }
        if (_containerType == ContainerType.ChestInfo)
        {
            ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.TakeFromChest);
        }
        if (_containerType == ContainerType.Info)
        {
            ChestUI.Instance.SpawnReferenceGUI(_Slot, ReferenceButtonType.TakeFromChest);
        }


        if (_Slot.item.itemTupe == ItemTupe.Poison)
        {
            GlobalSounds.Instance.SClickGlass();
        }
        else { GlobalSounds.Instance.SClickItem(); }

    }

}

