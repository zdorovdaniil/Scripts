using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Photon.MonoBehaviour
{
    // Для обновления статистики
    private PlayerStats Plstats;

    [SerializeField] private InventorySlot[] equips = new InventorySlot[7];
    [SerializeField] private List<InventorySlot> items = new List<InventorySlot>(24);
    public List<InventorySlot> GetItems => items;
    [SerializeField] private int[] itemIDs = new int[31];
    private ItemDatabase _itemDatabase;
    [SerializeField] private ClothAdder _clothAdder;

    private void Start()
    {
        Plstats = GetComponent<PlayerStats>();
        _clothAdder = GetComponent<ClothAdder>();
        _itemDatabase = BasePrefs.instance.GetItemDatabase;
        if (PhotonNetwork.offlineMode) StartCoroutine(CoroutineLoadItemsId());
        else if (photonView.isMine) StartCoroutine(CoroutineLoadItemsId());
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    public void SendOthersInventoryCloth()
    {
        if (!PhotonNetwork.offlineMode)
        {
            int[] idCloth = new int[5];
            for (int i = 0; i < 5; i++)
            {
                idCloth[i] = itemIDs[i + 24];
            }
            if (photonView != null) photonView.RPC("GetClothSlots", PhotonTargets.OthersBuffered, (int[])idCloth);
        }
    }
    [PunRPC]
    private void GetClothSlots(int[] clothId)
    {
        for (int i = 0; i < 5; i++)
        {
            itemIDs[i + 24] = clothId[i];
        }
        _clothAdder.IterateCloth();
    }
    public void UpdateClothVisible()
    {
        SendOthersInventoryCloth();
        if (PhotonNetwork.offlineMode) { _clothAdder.IterateCloth(); }
        else { photonView.RPC("GetUpdateClothVisible", PhotonTargets.AllBuffered); }
    }
    [PunRPC]
    private void GetUpdateClothVisible()
    {
        if (_clothAdder != null) _clothAdder.IterateCloth();
    }
    // сохранения id предметов
    public void SaveItemsId()
    {
        SerializeItemId();
        int _slot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < 31; i++)
        {
            PlayerPrefs.SetInt(_slot + "itemId_" + i, itemIDs[i]);
        }
        // сохранение количество предметов в каждом слоте
        for (int i = 0; i < 24; i++)
        {
            if (GetInventorySlot(i) != null)
            {
                PlayerPrefs.SetInt(_slot + "item_" + i + "_amount", items[i].amount);
            }
        }
    }
    // запись предметов из items в itemsId для сохранения или загрузки
    public void SerializeItemId()
    {
        for (int i = 0; i < 24; i++)
        {
            var itemID = (GetInventorySlot(i) != null) ? items[i].item.Id : -1;
            itemIDs[i] = itemID;
        }
        for (int i = 24; i < 31; i++)
        {
            var itemID = (GetEquipSlot(i - 24) != null) ? GetEquipSlot(i - 24).item.Id : -1;
            itemIDs[i] = itemID;
        }
    }
    // установка Inventory слотов из itemsIDs
    public void SetInvSlotsFromItemsIDs()
    {
        items.Clear();
        int _slot = PlayerPrefs.GetInt("activeSlot");
        // выгрузка из вектора itemsIDs предметов, и добавление их в инвентарь
        for (int i = 0; i < 24; i++)
        {
            // !!! ДОРАБОТАТЬ СОЗДАНИЕ ПРЕДМЕТОВ ИЗ БД И
            // !!! И СОХРАНЕНИЕ УРОВНЕЙ ПРЕДМЕТОВ
            int _amount = 1;
            Item item = _itemDatabase.GetItem(itemIDs[i]);
            //Item temp_item = ScriptableObject.CreateInstance<Item>();
            if (item != null)
            {
                //temp_item.SetValues(item.Id, item.Icon, item.Name, item.MaxSizeSlot, item.Level);
            }
            if (PlayerPrefs.HasKey(_slot + "item_" + i + "_amount"))
            {
                _amount = PlayerPrefs.GetInt(_slot + "item_" + i + "_amount");
            }
            if (item != null)
            {
                InventorySlot new_slot = new InventorySlot(item, _amount);
                items.Add(new_slot);
                new_slot.SlotNum = items.Count;
            }
        }
    }
    // установка Equip слотов из ItemsIDs
    public void SetEquipSlotFromItemsIDs()
    {
        for (int i = 0; i < equips.GetLength(0); i++)
        {
            equips[i] = null;
        }
        // выгрузка из вектора itemsIDs предметов и добавление их в экипированные слоты
        for (int i = 24; i < 31; i++)
        {
            Item item = _itemDatabase.GetItem(itemIDs[i]);
            if (item != null)
            {
                InventorySlot new_slot = new InventorySlot(item);
                SetToEquipSlot(new_slot, i - 24);
            }
        }
    }
    public void LoadItemsId()
    {
        ClearItemsinInventory();
        int _slot = PlayerPrefs.GetInt("activeSlot");
        // загрузка в itemIDs предметов из PlayerPrefs
        for (int i = 0; i < 31; i++)
        {

            if (PlayerPrefs.HasKey(_slot + "itemId_" + i))
            { itemIDs[i] = PlayerPrefs.GetInt(_slot + "itemId_" + i); }
        }
    }
    public IEnumerator CoroutineLoadItemsId()
    {
        int slot = PlayerPrefs.GetInt("activeSlot");
        yield return new WaitForSecondsRealtime(0.1f);
        {
            LoadItemsId();
            // установка инвентарных слотов из itemsIds
            SetInvSlotsFromItemsIDs();
        }
        // установка EquipSlot из ItemsIDs
        SetEquipSlotFromItemsIDs();
        SendOthersInventoryCloth();
        UpdateClothVisible();
    }

    // очистка сохраненных данных инвентаря
    public void ClearInventory()
    {
        int _slot = PlayerPrefs.GetInt("activeSlot");
        for (int i = 0; i < 31; i++)
        {
            if (PlayerPrefs.HasKey(_slot + "itemId_" + i))
            { PlayerPrefs.DeleteKey(_slot + "itemId_" + i); }
        }
    }
    // очистка itemIDs в инвентаре
    public void ClearItemsinInventory()
    {
        for (int i = 0; i < 31; i++)
        {
            itemIDs[i] = 0;
        }
    }
    public bool IsFull()
    {
        if (items.Count >= 24) { Debug.Log("Inventory FUll"); return true; }
        else return false;
    }
    public bool AddItems(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in items)
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
        if (items.Count >= 24) { Debug.Log("Inventory FUll"); return false; } // если слоты в инвентаре заполнены
        InventorySlot new_slot = new InventorySlot(item, amount);
        items.Add(new_slot);
        new_slot.SlotNum = items.Count;
        return true;
    }
    // устанавливается Equip[] слот с указание индекса слота
    public void SetToEquipSlot(InventorySlot slot, int i)
    {
        equips[i] = slot;
        itemIDs[i + 24] = slot.item.Id;
        if (Plstats != null)
        {
            Plstats.CheckStatusEquip();
            Plstats.UpdateArmor();
        }
    }
    // проверка наличия всех предметов колекции в инвентаре
    public bool CheckContainItemsInCollection(List<InventorySlot> collection)
    {
        int count = 0;
        for (int i = 0; i < collection.Count; i++)
        {
            int haveCount = GetCountItemsWithId(collection[i].item.Id);
            int needCount = collection[i].amount;
            if (haveCount >= needCount)
            { count += 1; }
        }
        if (count >= collection.Count)
        { return true; }
        else { return false; }
    }
    // удаляет предмет по индексу из инвентаря
    public void DeleteItem(int i)
    {
        if (items[i].item != null) { items.RemoveAt(i); }
        else Debug.Log("Empty Slot");
        if (Plstats != null)
        {
            Plstats.CheckStatusEquip();
            Plstats.UpdateArmor();
        }
    }
    // удаляется Equip[] слот с указание индекса 
    public void DeleteEquip(int i, bool withItemsIDs = false)
    {
        equips[i] = null;
        itemIDs[i + 24] = -1;
        if (Plstats != null)
        {
            Plstats.CheckStatusEquip();
            Plstats.UpdateArmor();
        }
        UpdateClothVisible();
    }
    // удаление equip[] слот по id
    public void DeleteEquipId(int id)
    {
        for (int i = 0; i < equips.GetLength(0); i++)
        {
            if (equips[i] != null)
            {
                if (equips[i].item.Id == id)
                { DeleteEquip(i, true); }
            }
        }
        UpdateClothVisible();
    }
    // получить инвентарный слот из инвентаря
    public InventorySlot GetInventorySlot(int i)
    {
        return i < items.Count ? items[i] : null;
    }
    // получение euqip
    public InventorySlot GetEquipSlot(int i)
    {
        return i < equips.GetLength(0) ? equips[i] : null;
    }
    // получение экипировки
    public InventorySlot[] GetEquipsSlots()
    {
        return equips;
    }
    public List<InventorySlot> GetEquipsList()
    {
        List<InventorySlot> listEquips = new List<InventorySlot>();
        for (int i = 0; i < equips.GetLength(0); i++)
        {
            if (equips != null)
            {
                listEquips.Add(equips[i]);
            }
        }
        return listEquips;
    }
    // удалить предмет из инвентаря по id
    public void DeleteItemId(int id, int amount)
    {
        // количество предметов для удаления 
        int countDeleting = amount;
        InventorySlot deletingSlot = null;
        foreach (InventorySlot slot in items)
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
        if (deletingSlot != null) items.Remove(deletingSlot);
    }
    // получение количества предметов в инвентаре с указанным id
    public int GetCountItemsWithId(int idItem)
    {
        int count = 0;
        foreach (InventorySlot slot in items)
        {
            if (slot.item.Id == idItem)
            {
                count += slot.amount;
            }
        }
        return count;
    }
    // использование предмета из инвентаря с последующим уничтожением
    public bool UseItem(int i)
    {
        if (items[i].amount == 1)
        {
            DeleteItem(i);
            return true;
        }
        else
        {
            items[i].amount -= 1;
            return false;
        }
    }
}
public class InventorySlot
{
    public Item item;
    public int amount;
    public int SlotNum = 0;
    // Содержит предмет количество экземпляров этого предмета в инвентаре
    public InventorySlot(Item item, int amount = 1, int slotNum = 0)
    {
        this.item = item;
        if (amount > item.MaxSizeSlot) this.amount = 1;
        else this.amount = amount;
        SlotNum = slotNum;

    }
    // получает лист с items, складывает одинаковые, и получается лист InvSlots
    public static List<InventorySlot> CreateListInvSlots(List<Item> items)
    {
        List<InventorySlot> slots = new List<InventorySlot>();
        for (int i = 0; i < items.Count; i++)
        {
            InventorySlot newSlot = new InventorySlot(items[i], 1);
            if (slots.Count != 0)
            {
                bool isAdd = false;
                foreach (InventorySlot slot in slots)
                {
                    if (newSlot.item.Id == slot.item.Id)
                    {
                        slot.amount += newSlot.amount;
                        isAdd = true;
                    }
                }
                if (isAdd == false)
                {
                    slots.Add(newSlot);
                }
            }
            else
            {
                slots.Add(newSlot);
            }
        }
        return slots;
    }
}