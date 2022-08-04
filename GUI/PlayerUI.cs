using UnityEngine;
public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;
    [SerializeField] private GameObject _playerUIObject;
    [SerializeField] private Camera _characterViewCamera;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private PlayerUpdate _playerUpdate;
    [SerializeField] private ClothAdder _clothAdder;
    [SerializeField] private Transform _spawnReferenceGUI;
    [SerializeField] private GameObject _prefReferenceGUI;
    [SerializeField] private SlotsUI _invSlots;
    [SerializeField] private SlotsUI _equipSlots;
    private void Start() { Instance = this; }

    public void SwitchPlayerUIObject(bool status)
    {
        _playerUIObject.SetActive(status);
        if (!status) GlobalSounds.Instance.SCloseWindow();
            else GlobalSounds.Instance.SOpenWindow();
    }
    public void FillPlayerUI()
    {
        _invSlots.FullSlots(_inventory.GetItems);
        _equipSlots.FullSlots(_inventory.GetEquipsList());
        ViewStatsUI.Instance.UpdateViewStatsUI(_playerStats);
    }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        for (int i = 0; i < _spawnReferenceGUI.childCount; i++)
        {
            Destroy(_spawnReferenceGUI.GetChild(i).gameObject);
        }
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
        if (Item.UsingItem(_playerStats, slot, _inventory, _clothAdder, _playerUpdate))
        { Debug.Log("предмет использован"); }
        else Debug.Log("Предмет НЕ испольpован");
        FillPlayerUI();
    }
    public void DeleteSlot(InventorySlot slot)
    {
        _inventory.DeleteItemId(slot.item.Id, 1);
        FillPlayerUI();
    }
    public void SwitchCharacterCamera(bool status)
    {
        _characterViewCamera.gameObject.SetActive(status);
    }

}
