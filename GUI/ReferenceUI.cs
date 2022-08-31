using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ReferenceUI : MonoBehaviour
{
    [SerializeField] private ReferenceButtonType _buttonsType;
    [SerializeField] private GameObject _itemInfoPanel;// панель информации о предмете
    [SerializeField] private InventorySlot _slot;
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
    [SerializeField] private TextMeshProUGUI _numSlot;
    [SerializeField] private TextMeshProUGUI _nameItem;
    [SerializeField] private TextMeshProUGUI _amountItemsInSlot;
    [SerializeField] private Image _imageItem;
    [SerializeField] private TextMeshProUGUI _itemType;
    [SerializeField] private TextMeshProUGUI _itemCost;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private TextMeshProUGUI _armor;
    [SerializeField] private TextMeshProUGUI _buffValue;
    [SerializeField] private TextMeshProUGUI _needAttr;
    [SerializeField] private TextMeshProUGUI _nameAttr;
    [SerializeField] private TextMeshProUGUI _needSkill;
    [SerializeField] private TextMeshProUGUI _nameSkill;
    [SerializeField] private TextMeshProUGUI _needLevel;
    [SerializeField] private TextMeshProUGUI _features;


    public void ClickMoveToChest()
    {
        int temp = _slot.amount;
        ChestUI.Instance.PlaceItemToChest(_slot);
        if (temp == 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
        GlobalSounds.Instance.SPlaceItem();
    }
    public void ClickMoveToInvFromChest()
    {
        int temp = _slot.amount;
        ChestUI.Instance.PlaceItemToInventory(_slot);
        if (temp == 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
        GlobalSounds.Instance.SPlaceItem();
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
        GlobalSounds.Instance.SBuySell();
    }
    public void ClickSellOne()
    {
        int temp = _slot.amount;
        ShopingUI.instance.SellSlot(_slot.item, 1);
        SetValueSlot(_slot, _buttonsType);
        if (temp <= 1)
        { Destroy(this.gameObject); }
        else { SetValueSlot(_slot, _buttonsType); }
        GlobalSounds.Instance.SBuySell();
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
        GlobalSounds.Instance.SEquipArmor();
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
            //if (_playerStats.stats.GetBuyModif() != 0) 
            _itemCost.text = (item.cost * _playerStats.stats.GetBuyModif()).ToString();
            // else _itemCost.text = (item.cost * 2).ToString();
        }
        else if (buttonsType == ReferenceButtonType.Sell) _itemCost.text = (item.cost * _playerStats.stats.GetSaleModif()).ToString();
        else _itemCost.text = item.cost.ToString();
        ItemTupe _tupe = item.itemTupe;
        _itemType.text = _tupe.ToString().ToLower();
        _level.text = item.Level.ToString();
        _features.text = item.Features;
        if (item.Attack > 0)
        {
            OnOffParentGameObject(true, _attack);
            _attack.text = item.Attack.ToString();
        }
        else { OnOffParentGameObject(false, _attack); }
        if (item.Armor > 0)
        {
            OnOffParentGameObject(true, _armor);
            _armor.text = item.Armor.ToString();
        }
        else { OnOffParentGameObject(false, _armor); }
        /*if (item.BuffValue > 0)
        {
            OnOffParentGameObject(true, _buffValue);
            _buffValue.text = item.BuffValue.ToString();
        }
        else { OnOffParentGameObject(false, _buffValue); }
        if (item.neededAttr > 0)
        {
            OnOffParentGameObject(true, _needAttr);
            _needAttr.text = item.neededAttr.ToString();
            _nameAttr.text = item.nameAttr.ToString();
        }
        else { OnOffParentGameObject(false, _needAttr); }
        if (item.neededSkill > 0)
        {
            OnOffParentGameObject(true, _needSkill);
            _needSkill.text = item.neededSkill.ToString();
            _nameSkill.text = item.nameSkill.ToString();
        }
        else { OnOffParentGameObject(false, _needSkill); }
        if (item.neededLevel > 0)
        {
            OnOffParentGameObject(true, _needLevel);
            _needLevel.text = item.neededLevel.ToString();
        }
        else { OnOffParentGameObject(false, _needLevel); }
        */

        CheckReferenceButtonType(buttonsType);
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
