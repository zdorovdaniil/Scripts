using UnityEngine;
using System.Collections.Generic;
using TMPro;

// скрипт отвечает за интерфейс отображения текущего статуса повышения уровня персонажа
public class PlayerLevelingUI : MonoBehaviour
{
    [SerializeField] private SlotsUI _slotsLeveling;
    [SerializeField] private TMP_Text _levelingText;
    [SerializeField] private Transform _buttonLevelUp;
    [SerializeField] private Inventory _inv;
    public void FillLevelingUI()
    {
        _slotsLeveling.ClearSlots();
        List<InventorySlot> itemsForCurLeveling = PlayerLeveling.Instance.GetItemsForLeveling();
        if (itemsForCurLeveling != null)
        {
            _slotsLeveling.FullSlots(itemsForCurLeveling);
            if (PlayerLeveling.Instance.IsExpFull() && _inv.CheckContainItemsInCollection(itemsForCurLeveling))
            { _buttonLevelUp.gameObject.SetActive(true); _levelingText.text = ""; }
            else { _buttonLevelUp.gameObject.SetActive(false); _levelingText.text = "not enought exp or items"; }
        }
        else
        {
            if (PlayerLeveling.Instance.IsExpFull()) { _buttonLevelUp.gameObject.SetActive(true); }
            else { _buttonLevelUp.gameObject.SetActive(false); }
        }
    }
    public void ClickLevelUp()
    {
        List<InventorySlot> itemsForCurLeveling = PlayerLeveling.Instance.GetItemsForLeveling();
        if (itemsForCurLeveling != null)
        {
            foreach (InventorySlot slot in itemsForCurLeveling)
            { _inv.DeleteItemId(slot.item.Id, slot.amount); }
        }
        PlayerLeveling.Instance.LevelUp();
        PlayerUI.Instance.FillPlayerUI();
        FillLevelingUI();
    }
}
