using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ReferenceUI : MonoBehaviour
{
    [SerializeField] private ReferenceButtonType _buttonsType;
    [SerializeField] private Transform _fightingList;
    [Header("Requirement")]
    [SerializeField] private Transform _itemRequirement;
    [SerializeField] private Transform _requirementContainer;
    [SerializeField] private BuffField _prefRequirementField;
    [Header("Buff")]
    [SerializeField] private Transform _itemBuff;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private BuffField _prefBuffField;
    private PlayerStats _playerStats;
    [Header("Buttons")]

    [SerializeField] private Transform _buttonsSell;
    [SerializeField] private Transform _buttonsBuy;
    [SerializeField] private Transform _buttonsMoveOut;
    [SerializeField] private Transform _buttonsMoveGet;
    [SerializeField] private Transform _buttonsRemove;
    [SerializeField] private Transform _buttonsEquip;
    [SerializeField] private Transform _buttonsUse;
    [SerializeField] private Transform _buttonsMoveToChest;
    [SerializeField] private Transform _buttonsMoveFromChest;
    [SerializeField] private Transform _buttonsDelete;

    [Header("Item Information")]
    [SerializeField] private InventorySlot _slot;
    [SerializeField] private TextMeshProUGUI _numSlot;
    [SerializeField] private TextMeshProUGUI _nameItem;
    [SerializeField] private TextMeshProUGUI _amountItemsInSlot;
    [SerializeField] private Image _imageItem;
    [SerializeField] private TextMeshProUGUI _itemType;
    [SerializeField] private TextMeshProUGUI _itemCost;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private TextMeshProUGUI _armor;
    [SerializeField] private TextMeshProUGUI _critChance;
    [SerializeField] private TextMeshProUGUI _critValue;
    [SerializeField] private TextMeshProUGUI _features;


    public void ClickMoveToChest()
    {
        GlobalSounds.Instance.SPlaceItem();
        int temp = _slot.amount;
        ChestUI.Instance.PlaceItemToChest(_slot);
        if (temp == 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }

    }
    public void ClickMoveToInvFromChest()
    {
        GlobalSounds.Instance.SPlaceItem();
        int temp = _slot.amount;
        ChestUI.Instance.PlaceItemToInventory(_slot);
        if (temp == 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }

    }
    public void ClickSellAll()
    {
        ShopingUI.instance.SellSlot(_slot.item, _slot.amount);
        GlobalSounds.Instance.SBuySell();
        Destroy(this.gameObject);
    }
    public void ClickBuyOne()
    {
        ShopingUI.instance.BuyItem(_slot.item);
    }
    public void ClickSellOne()
    {
        GlobalSounds.Instance.SPlaceItem();
        int temp = _slot.amount;
        ShopingUI.instance.SellSlot(_slot.item, 1);
        SetValueSlot(_slot, _buttonsType);
        if (temp <= 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
    }
    // переключение видимости родительских объектов у TextMeshProUGUI
    private void OnOffParentGameObject(bool status, TextMeshProUGUI mesh)
    {
        GameObject parent = mesh.transform.parent.gameObject;
        parent.SetActive(status);
    }
    public void ClickDeleteItem()
    {
        int temp = _slot.amount;
        if (_buttonsType == ReferenceButtonType.Inventory)
        { if (PlayerUI.Instance != null) PlayerUI.Instance.DeleteSlot(_slot); }
        else if (_buttonsType == ReferenceButtonType.Move)
        { if (Storage.instance != null) { Storage.instance.DeleteItemFromInvId(_slot.item.Id); } }
        if (temp == 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
    }
    public void ClickMoveOutAll()
    {
        Storage.instance.MoveToStorageSlot(_slot.item, _slot.amount);
        GlobalSounds.Instance.SPlaceItem();
        Destroy(this.gameObject);
    }
    public void ClickMoveOutOne()
    {
        int temp = _slot.amount;
        Storage.instance.MoveToStorageSlot(_slot.item, 1);
        if (temp <= 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
        GlobalSounds.Instance.SPlaceItem();
    }
    public void ClickMoveGetAll()
    {
        Storage.instance.MoveToInvFromStorage(_slot.item, _slot.amount);
        GlobalSounds.Instance.SPlaceItem();
        Destroy(this.gameObject);
    }
    public void ClickMoveGetOne()
    {
        int temp = _slot.amount;
        Storage.instance.MoveToInvFromStorage(_slot.item, 1);
        GlobalSounds.Instance.SPlaceItem();
        Destroy(this.gameObject);
    }
    public void ClickRemoveEquip()
    {
        if (PlayerUI.Instance != null)
        { PlayerUI.Instance.RemoveEquipSlot(_slot.item); }
        else { Storage.instance.RemoveEquipSlot(_slot.item); }
        GlobalSounds.Instance.SPlaceItem();
        Destroy(this.gameObject);
    }
    public void ClickEquipSlotInStorage()
    {
        Storage.instance.Equip(_slot);

        Destroy(this.gameObject);
    }
    public void ClickUseSlotInInventory()
    {
        PlayerUI.Instance.UseSlot(_slot);
        Destroy(this.gameObject);
    }

    public void DestroyReferenceUI()
    {
        Destroy(this.gameObject);
        GlobalSounds.Instance.SCloseWindow();
    }
    private void CheckSlotAmount(int amount)
    { if (amount <= 1) Destroy(this.gameObject); }
    private void CheckReferenceButtonType(ReferenceButtonType buttonsType)
    {
        CloseDoingButtons();
        if (buttonsType == ReferenceButtonType.Sell)
        {
            _buttonsSell.gameObject.SetActive(true);
            _buttonsDelete.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.Buy)
        {
            _buttonsBuy.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.Move)
        {
            _buttonsMoveOut.gameObject.SetActive(true);
            if (_slot.item.itemTupe == ItemTupe.Armor || _slot.item.itemTupe == ItemTupe.Weapon)
            { _buttonsEquip.gameObject.SetActive(true); }
            _buttonsDelete.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.Get)
        {
            _buttonsMoveGet.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.Equip)
        {
            _buttonsRemove.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.Inventory)
        {
            if (_slot.item.itemTupe == ItemTupe.Armor || _slot.item.itemTupe == ItemTupe.Weapon)
            { _buttonsUse.gameObject.SetActive(true); }
            else if (_slot.item.itemTupe == ItemTupe.Food || _slot.item.itemTupe == ItemTupe.Poison || _slot.item.itemTupe == ItemTupe.Usable || _slot.item.itemTupe == ItemTupe.Improve)
            { _buttonsUse.gameObject.SetActive(true); }
            _buttonsDelete.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.ToChest)
        {
            _buttonsMoveToChest.gameObject.SetActive(true);
        }
        else if (buttonsType == ReferenceButtonType.TakeFromChest)
        {
            _buttonsMoveFromChest.gameObject.SetActive(true);
        }
    }
    public void SetPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }

    public void SetValueSlot(InventorySlot slot, ReferenceButtonType buttonsType)
    {
        _buttonsType = buttonsType;
        Item item = slot.item;
        _slot = slot;
        _numSlot.text = slot.SlotNum.ToString();
        _nameItem.text = item.Name.ToLower();
        _amountItemsInSlot.text = slot.amount.ToString() + " | " + slot.item.MaxSizeSlot.ToString();
        _imageItem.sprite = item.Icon;

        if (buttonsType == ReferenceButtonType.Buy)
        {
            _itemCost.text = (item.cost * _playerStats.stats.GetBuyModif()).ToString();
        }
        else if (buttonsType == ReferenceButtonType.Sell) _itemCost.text = (item.cost * _playerStats.stats.GetSaleModif()).ToString();
        else _itemCost.text = item.cost.ToString();
        ItemTupe _tupe = item.itemTupe;
        _itemType.text = _tupe.ToString().ToLower();
        _level.text = item.Level.ToString();
        _features.text = item.Features;
        if (IsFighting())
        {
            _fightingList.gameObject.SetActive(true);
            _attack.text = _slot.item.Attack.ToString();
            _armor.text = _slot.item.Armor.ToString();
            _critChance.text = _slot.item.CritChance.ToString();
            _critValue.text = _slot.item.CritValue.ToString();
        }
        else { _fightingList.gameObject.SetActive(false); }
        if (IsHasBuffs())
        {
            _itemBuff.gameObject.SetActive(true);
            ProcessCommand.ClearChildObj(_itemContainer);
            foreach (BuffClass buffClass in item.Buffs)
            {
                BuffStat buffStat = new BuffStat(buffClass);
                Instantiate(_prefBuffField, _itemContainer).GetComponent<BuffField>().SetBuffInfoField(buffStat); ;
            }
        }
        else { _itemBuff.gameObject.SetActive(false); }
        if (slot.item.GetItemRequirement != BasePrefs.instance.GetEmptyRequirement)
        {
            ProcessCommand.ClearChildObj(_requirementContainer);
            _itemRequirement.gameObject.SetActive(true);
            List<RequirementField> fields = slot.item.GetItemRequirement.GetRequirementFields();
            foreach (RequirementField field in fields)
            {
                Instantiate(_prefRequirementField, _requirementContainer).GetComponent<BuffField>().SetFieldRequirement(field);
            }
        }
        else
        {
            _itemRequirement.gameObject.SetActive(false);
        }

        CheckReferenceButtonType(buttonsType);
        bool IsFighting()
        {
            if (slot.item.Armor > 0 || slot.item.Attack > 0 || slot.item.CritChance > 0 || slot.item.CritValue > 0)
            { return true; }
            else { return false; }
        }
        bool IsHasBuffs()
        {
            if (item.Buffs.Count > 0) { return true; } else { return false; }
        }
    }

    private void CloseDoingButtons()
    {
        _buttonsEquip.gameObject.SetActive(false);
        _buttonsBuy.gameObject.SetActive(false);
        _buttonsSell.gameObject.SetActive(false);
        _buttonsMoveGet.gameObject.SetActive(false);
        _buttonsMoveOut.gameObject.SetActive(false);
        _buttonsRemove.gameObject.SetActive(false);
        _buttonsUse.gameObject.SetActive(false);
        _buttonsMoveToChest.gameObject.SetActive(false);
        _buttonsMoveFromChest.gameObject.SetActive(false);
        _buttonsDelete.gameObject.SetActive(false);
    }
}

public enum ReferenceButtonType
{
    None,
    Simple,
    Buy,
    Sell,
    Move,
    Get,
    Equip,
    Inventory,
    ToChest,
    TakeFromChest,
}
