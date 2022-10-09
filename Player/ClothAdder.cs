using UnityEngine;

public class ClothAdder : MonoBehaviour
{
    [SerializeField] private GameObject Weapon;
    [SerializeField] private GameObject Helmet;
    [SerializeField] private GameObject Tors;
    [SerializeField] private GameObject Pants;
    [SerializeField] private GameObject Shoes;
    [SerializeField] private GameObject Hair;
    // ссылка на mesh персонажа, изменяемый в скрипте
    [SerializeField] private SkinnedMeshRenderer playerSkin;
    // ссылка на mesh персонажа, для восстановления исходного mesh персонажа
    [SerializeField] private SkinnedMeshRenderer FullMeshBody;
    [SerializeField] private SkinnedMeshRenderer MeshWithoutShoes;
    [SerializeField] private Inventory _inv;
    [SerializeField] private Item _emptyPantsSlot;

    // проверка одежды (удаляется вся одежда с персонажа, а затем добавляется снова из инвентаря)
    public void IterateCloth()
    {
        if (_inv != null)
        {
            // удаление одежды с персонажа
            DeleteAllCloth();
            _inv.SetEquipSlotFromItemsIDs();
            foreach (InventorySlot _equip in _inv.GetEquipsSlots())
            {
                if (_equip != null)
                {
                    // добавление одежды для персонажа из его инвентаря inv
                    SetArmorVisible(_equip);
                }
            }
        }
    }
    private void DeleteAllCloth()
    {
        Destroy(Weapon); Weapon = null;
        Destroy(Helmet); Helmet = null;
        Destroy(Tors); Tors = null;
        Destroy(Pants); Pants = null;
        Destroy(Shoes); Shoes = null;
        UpdateProxy();
    }
    // функция добавления одежды
    public void addClothes(InventorySlot _slot)
    {
        GameObject clothObj = null;
        // инициализация одежды на сцене
        // 
        clothObj = Instantiate(_slot.item.PrefabItem, playerSkin.transform.parent);
        SkinnedMeshRenderer[] renderers = clothObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.bones = playerSkin.bones;
            renderer.rootBone = playerSkin.rootBone;
        }
        // перебор типа предмета
        if (_slot.item.itemTupe == ItemTupe.Weapon)
        {
            Destroy(Weapon);
            Weapon = clothObj; // запись одежды в переменную
            SetLayer(Weapon); // установка слоЯ
        }
        if (_slot.item.armorTupe == ArmorTupe.Helmet) { Destroy(Helmet); Helmet = clothObj; SetLayer(Helmet); }
        if (_slot.item.armorTupe == ArmorTupe.Tors) { Destroy(Tors); Tors = clothObj; SetLayer(Tors); }
        if (_slot.item.armorTupe == ArmorTupe.Pants) { Destroy(Pants); Pants = clothObj; SetLayer(Pants); }
        if (_slot.item.armorTupe == ArmorTupe.Shoes) { Destroy(Shoes); Shoes = clothObj; SetLayer(Shoes); }
        UpdateProxy();
    }
    private void SetLayer(GameObject _obj)
    {
        // всем дочерним объектам _obj, ставится layer 3
        if (_obj != null)
        {
            foreach (Transform child in _obj.transform)
            {
                child.gameObject.layer = 3;
            }
        }
    }
    // удаление видимости одежды на персонаже
    public void removeClothes(InventorySlot _slot)
    {
        if (_slot.item.itemTupe == ItemTupe.Weapon) { Destroy(Weapon); Weapon = null; }
        if (_slot.item.armorTupe == ArmorTupe.Helmet) { Destroy(Helmet); Helmet = null; }
        if (_slot.item.armorTupe == ArmorTupe.Tors) { Destroy(Tors); Tors = null; }
        if (_slot.item.armorTupe == ArmorTupe.Pants) { Destroy(Pants); Pants = null; }
        if (_slot.item.armorTupe == ArmorTupe.Shoes) { Destroy(Shoes); Shoes = null; }
        UpdateProxy();
    }
    // установка видимости одежды на персонаже и удаление предыдущей на старом месте
    public void SetArmorVisible(InventorySlot _slot)
    {
        if (_slot.item.itemTupe == ItemTupe.Weapon) { removeClothes(_slot); addClothes(_slot); }
        if (_slot.item.armorTupe == ArmorTupe.Helmet) { removeClothes(_slot); addClothes(_slot); }
        if (_slot.item.armorTupe == ArmorTupe.Tors) { removeClothes(_slot); addClothes(_slot); }
        if (_slot.item.armorTupe == ArmorTupe.Pants) { removeClothes(_slot); addClothes(_slot); }
        if (_slot.item.armorTupe == ArmorTupe.Shoes) { removeClothes(_slot); addClothes(_slot); }
        UpdateProxy();

    }
    private void UpdateProxy()
    {
        // если есть ботинки, то активирует proxe без голеней
        if (Shoes) { playerSkin.sharedMesh = MeshWithoutShoes.sharedMesh; } else { playerSkin.sharedMesh = FullMeshBody.sharedMesh; }
        // если есть шлем то волосы убираются
        if (Helmet) { Hair.SetActive(false); }
        else Hair.SetActive(true);
        // если нет штанов, то добавляются шорты
        if (!Pants) { addClothes(InventorySlot.CreateSlot(_emptyPantsSlot)); }

        // 
        /*
            if (Tors == null && Pants == null) // если нет ни торса ни штанов
            {
                playerSkin.sharedMesh = FullMeshBody.sharedMesh;
            }
            if (Tors == null && Pants != null)
            {
                playerSkin.sharedMesh = OnlyTorsMeshBody.sharedMesh;
            }
            if (Tors != null && Pants == null)
            {
                playerSkin.sharedMesh = OnlyPantsMeshBody.sharedMesh;
            }
            if (Tors != null && Pants != null)
            {
                playerSkin.sharedMesh = NoMeshBody.sharedMesh;
            }
        */
    }


}
