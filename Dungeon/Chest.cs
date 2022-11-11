using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private int _sizeChest = 8;
    // Слоты для хранения предметов в сундуке
    [SerializeField] private List<InventorySlot> _chestSlots = new List<InventorySlot>(8);
    public List<InventorySlot> GetChestSlot => _chestSlots;
    // Тип предмета в каждом слоте
    [SerializeField]
    private List<ItemTupe> _typeInSlots = new List<ItemTupe>(8)
    {ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing,ItemTupe.Nothing};
    // Предметы что находятся в сундуке по умолчанию
    [SerializeField] private List<Item> _defaultItemsInChest = new List<Item>(8);
    // База данных предметов от куда будут браться предметы для спавна
    [SerializeField] private ItemDatabase _itemDatabase;
    private bool _isOpened = false; public bool IsOpened => _isOpened;
    // чем выше, тем больше шанс появления предмета в слоте сундука от 50 до 75. Зависит от уровня подземелья.
    // устанавливается уровнем подземелья
    private int _coffSpawnSlot;
    private void Start()
    {
        DungeonObjects.Instance.AddChest(this);
        _coffSpawnSlot = DungeonStats.Instance.Rule.GetModifSpawnItemsChest(DungeonStats.Instance.GetDungeonLevel);
    }
    public void ChangeMesh(bool status)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (status) meshFilter.mesh = BasePrefs.instance.GetMeshOpenedChest;
        else meshFilter.mesh = BasePrefs.instance.GetMeshClosedChest;
    }

    public void SpawnItemsInChest()
    {
        int willSpanwItems = 0;
        int spawnedItems = 0;
        foreach (ItemTupe type in _typeInSlots)
        {
            if (type == ItemTupe.Nothing) { continue; }
            else
            {
                willSpanwItems += 1;
                List<Item> sortedItems = new List<Item>();
                // получение списка предметов по типу в слоте из БД
                sortedItems = _itemDatabase.GetListItemsByType(type);
                int countItems = sortedItems.Count;
                int chance = Random.Range(0, 100);
                if (chance >= _coffSpawnSlot) continue;
                else
                {
                    spawnedItems += 1;
                    int numSpawn = Random.Range(0, countItems);
                    AddItemToChest(sortedItems[numSpawn], 1);
                }
            }
        }
        if (willSpanwItems != 0 && spawnedItems != 0)
        { Debug.Log("WillSpawn: " + willSpanwItems + " . Spawned: " + spawnedItems + " Coff: " + spawnedItems * 100 / willSpanwItems + ". ChanceSpawnSlot:" + _coffSpawnSlot); }
        // добавление предметов в сундук по умолчанию
        foreach (Item item in _defaultItemsInChest)
        { AddItemToChest(item, 1); }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {   // выполняется при первом открытии сундука
            if (_isOpened == false && _itemDatabase != null)
            {
                _isOpened = true;
                DungeonStats.Instance.allOpenChest += 1;
                DungeonStats.Instance.numOpenChest += 1;
                PlayerQuest.instance.UpdateProcessQuests();
                // Добавление в сундук предметов
                SpawnItemsInChest();
            }
            PlayerLinks playerLinks = other.GetComponent<PlayerLinks>();
            playerLinks.GetChestUI.SetCurChest(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Выключение кнопки открытия сундука
            ChestUI.Instance.ResetCurChest();
        }
    }
    public bool AddItemToChest(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in _chestSlots)
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
        if (_chestSlots.Count >= _sizeChest) { Debug.Log("Inventory FUll"); return false; }
        InventorySlot new_slot = new InventorySlot(item, amount);
        _chestSlots.Add(new_slot);

        return true;
    }
    public void DeleteItemId(int id, int amount)
    {
        // количество предметов для удаления 
        int countDeleting = amount;
        InventorySlot deletingSlot = null;
        foreach (InventorySlot slot in _chestSlots)
        {
            if (slot.item.Id == id)
            {
                if (slot.amount == countDeleting)
                { deletingSlot = slot; }
                else if (slot.amount < countDeleting)
                { deletingSlot = slot; countDeleting -= slot.amount; }
                else if (slot.amount > countDeleting)
                { slot.amount -= countDeleting; }
            }
        }
        if (deletingSlot != null) _chestSlots.Remove(deletingSlot);
    }
}

