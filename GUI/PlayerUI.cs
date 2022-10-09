using UnityEngine;
public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;
    [SerializeField] private GameObject _playerUIObject;
    [SerializeField] private Camera _characterViewCamera;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private PlayerUpdate _playerUpdate;
    [SerializeField] private Transform _spawnReferenceGUI;
    [SerializeField] private GameObject _prefReferenceGUI;
    [SerializeField] private SlotsUI _invSlots;
    [SerializeField] private SlotsUI _equipSlots;
    [SerializeField] private ViewStatsUI _viewStatsUI;


    private void Start() { Instance = this; }

    public void SetInventory(Inventory inv) => _inventory = inv;
    public void SetPlayerStats(PlayerStats plS) => _playerStats = plS;
    public void SwitchPlayerUIObject(bool status)
    {
        if (_playerUIObject) _playerUIObject.SetActive(status);
        if (!status) GlobalSounds.Instance.SCloseWindow();
        else GlobalSounds.Instance.SOpenWindow();
    }
    public void FillPlayerUI()
    {
        _playerStats.UpdateArmor();
        _playerStats.CheckStatusEquip();
        if (_invSlots) _invSlots.FullSlots(_inventory.GetItems);
        if (_equipSlots) _equipSlots.FullSlots(_inventory.GetEquipsList());
        if (_viewStatsUI) _viewStatsUI.UpdateViewStatsUI(_playerStats);
    }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        ProcessCommand.ClearChildObj(_spawnReferenceGUI);
        Instantiate(_prefReferenceGUI, _spawnReferenceGUI).GetComponent<ReferenceUI>().SetValueSlot(slot, buttonType);
    }
    public void RemoveEquipSlot(Item item)
    {
        if (_inventory.AddItems(item, 1))
        { _inventory.DeleteEquipId(item.Id); }
        else Debug.Log("Инвентарь переполнен");
        FillPlayerUI();
    }
    public void UseSlot(InventorySlot slot)
    {
        if (slot.item.UsingItem(_playerStats, slot, _inventory, _playerUpdate))
        { Debug.Log("предмет использован"); }
        else Debug.Log("Предмет НЕ испольpован");
        FillPlayerUI();
    }
    public void DeleteSlot(InventorySlot slot)
    {
        _inventory.DeleteSlot(slot, 1);
        FillPlayerUI();
    }
    public void SwitchCharacterCamera(bool status)
    {
        _characterViewCamera.gameObject.SetActive(status);
    }
    public void PlaySound()
    {
        GlobalSounds.Instance.SOpenWindow();
    }

}
