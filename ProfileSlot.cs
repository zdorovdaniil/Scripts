using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class ProfileSlot : MonoBehaviour
{
    public MainMenuUI menuUI;
    public int NumSlot;
    // GU интерфейс для слотов
    public Text SlotID;
    public Text SlotName;
    public Text SlotLevel;
    public List<TMP_Text> AttributesText = new List<TMP_Text>();
    public GameObject FreeSlot;

    private void Start()
    {
        OpenStatsToSlot();
    }
    public void OpenStatsToSlot()
    {
        SlotID.text = NumSlot.ToString();
        SlotName.text = PlayerPrefs.GetString(NumSlot + "_slot_nickName");
        for (int i = 0; i < BasePrefs.instance.AvaibleAttributes.Count; i++)
        {
            AttributesText[i].text = PlayerPrefs.GetInt(NumSlot + "_slot_" + i + "_attribute").ToString();
        }
        SlotLevel.text = PlayerPrefs.GetInt(NumSlot + "_slot_level").ToString();
        if (PlayerPrefs.GetInt(NumSlot + "_slot_level") == 0) // если уровень игрока равен 0, то ставится свободный слот
        {
            FreeSlot.SetActive(true);
        }
        else FreeSlot.SetActive(false);
    }
    public void SelectProfile()
    {
        PlayerPrefs.SetInt("activeSlot", NumSlot);
        // если уровень игрока равен 0, то создается новый персонаж
        // и открывается меню создание нового персонажа
        if (PlayerPrefs.GetInt(NumSlot + "_slot_level") == 0)
        {
            menuUI.ClickCreateNewCharacter();
        }
        // иначе открывается окно выбора подземелья
        else
        {
            menuUI.CreatePlayerStats();
            menuUI.ClickSelSlot();
        }
        GlobalSounds.Instance.SButtonClick();

    }
    public void GetReport(ReportType reportType)
    {
        if (reportType == ReportType.Accept)
        { RemoveSlot(NumSlot); }
    }
    public void DeleteSlot()
    {
        GlobalSounds.Instance.SButtonClick();
        MsgBoxUI.Instance.Show(this.gameObject, "deleting", "do you really want delete this game slot");
    }
    private void RemoveSlot(int _slot)
    {
        FreeSlot.SetActive(true);
        PlayerPrefs.SetInt(_slot + "_for_new_game", 0);
        PlayerPrefs.DeleteKey(_slot + "_slot_level");
        PlayerPrefs.DeleteKey(_slot + "_slot_exp");
        PlayerPrefs.DeleteKey(_slot + "_slot_nickName");

        PlayerPrefs.DeleteKey(_slot + "_slot_dungeonLevel");

        PlayerPrefs.DeleteKey(_slot + "_slot_strenght");
        PlayerPrefs.DeleteKey(_slot + "_slot_agility");
        PlayerPrefs.DeleteKey(_slot + "_slot_endurance");
        PlayerPrefs.DeleteKey(_slot + "_slot_speed");

        PlayerPrefs.DeleteKey(_slot + "_slot_point");
        PlayerPrefs.DeleteKey(_slot + "_slot_pointSkill");
        PlayerPrefs.DeleteKey(_slot + "_slot_curHP");
        PlayerPrefs.DeleteKey(_slot + "_slot_curEXP");
        PlayerPrefs.DeleteKey(_slot + "_slot_curGOLD");

        PlayerPrefs.DeleteKey(_slot + "_slot_detection");
        PlayerPrefs.DeleteKey(_slot + "_slot_evasion");
        PlayerPrefs.DeleteKey(_slot + "_slot_medicine");
        PlayerPrefs.DeleteKey(_slot + "_slot_trade");
        PlayerPrefs.DeleteKey(_slot + "_slot_hacking");
        PlayerPrefs.DeleteKey(_slot + "_slot_wearArmor");
        PlayerPrefs.DeleteKey(_slot + "_slot_wieldSword");

        // очистка данных инвентаря
        for (int i = 0; i < 29; i++)
        {
            PlayerPrefs.DeleteKey(_slot + "itemId_" + i);
        }
        // очистка данных хранилища
        for (int i = 0; i < 40; i++)
        {
            PlayerPrefs.DeleteKey(_slot + "storageId_" + i);
        }
        // очистка улучшений
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey(_slot + "_slot_" + i + "_improveLvl"))
            {
                PlayerPrefs.DeleteKey(_slot + "_slot_" + i + "_improveLvl");
            }
        }
        // очистка квестов, 50 имерное колво квестов
        for (int i = 0; i < 50; i++)
        {
            if (PlayerPrefs.HasKey(_slot + "_questProcess_" + i))
            {
                PlayerPrefs.DeleteKey(_slot + "_questProcess_" + i);
                PlayerPrefs.DeleteKey(_slot + "_questCompleted_" + i);
            }
        }
    }

}
