using System.Collections.Generic;
using UnityEngine;

public class DungeonStats : Photon.MonoBehaviour
{
    public static DungeonStats Instance; private void Awake() { Instance = this; }
    [SerializeField] private DungeonRules _dungeonRules; public DungeonRules Rule => _dungeonRules;
    [SerializeField] private bool _isCompleteDungeon = false; public bool IsCompleteDungeon => _isCompleteDungeon;
    [SerializeField] private int _dungeonLevel; public void SetDungeonLevel(int value) => _dungeonLevel = value; public int GetDungeonLevel => _dungeonLevel;

    // Статистика за подземелье
    public int numEnemyInDungeon; // общее количество противников в подземелье
    public int numRoomsInDungeon; // общее количество комнат в подземелье
    public int numChestsInDungeon; // общее количество сундуков в подземелье
    public int passedRoom; // количество пройденых комнат в подземелье
    public int curKills; // количество уничтоженных врагов в тукущем подземелье
    public int numOpenChest; // кол-во открытых сундуков
    public int numDeath; /*Кол-во смертей в подземелье */ public void AddDeath() { numDeath += 1; allDeath += 1; }

    public List<RoomControl> interedRooms = new List<RoomControl>(); // список комнат, где были игроки

    // Собираемая за всю игру статистика
    public int allPassRoom; // количество пройденных комнат за всю игру
    public int allKills; // уничтожено врагов за всю игру
    public int allOpenChest; // всего открытых сундуков
    public int allDeath; // кол-во смертей в подземелье
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }

    public void CheckIsEnteredRoom(RoomControl enteredRoom)
    {
        if (!interedRooms.Contains(enteredRoom))
        {
            interedRooms.Add(enteredRoom);
            allPassRoom += 1;
            passedRoom += 1;
            PlayerQuest.instance.UpdateProcessQuests();
        }
    }
    public void DefeatEnemy()
    {
        allKills += 1;
        if (PhotonNetwork.offlineMode != true) { photonView.RPC("CountKills", PhotonTargets.All); }
        else curKills += 1;
    }
    public int GetNumRooms => Rule.NumRoomsFromLevel(_dungeonLevel);

    /// <summary>
    /// Вызывается из TriggerZone
    /// </summary>
    [PunRPC]
    public void CompleteDungeon()
    {
        int check = ProcessCommand.CheckIsLevelUpDungeonLevel();
        if (check == 0)
        {
            ProcessCommand.SetMaxDungeonLevel(_dungeonLevel + 1);
            MsgBoxUI.Instance.ShowInfo(TextBase.Field(2), TextBase.Field(3) + " " + ProcessCommand.MaxDungeonLevel);
        }
        else if (check == 1)
        {
            MsgBoxUI.Instance.ShowInfo(TextBase.Field(2), TextBase.Field(4));
        }
        _isCompleteDungeon = true;
    }
    [PunRPC]
    public void CountKills()
    {
        curKills += 1;
    }
    [PunRPC]
    public void CountPassRoom()
    {
        passedRoom += 1;
    }
    public void LoadDungeonStats()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        allKills = PlayerPrefs.GetInt(id + "_slot_allKills");
        allPassRoom = PlayerPrefs.GetInt(id + "_slot_allPassRoom");
        allOpenChest = PlayerPrefs.GetInt(id + "_slot_allOpenChest");
        allDeath = PlayerPrefs.GetInt(id + "_slot_allDeath");
    }
    public void ResetDungeonStats()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        allKills = 0; allPassRoom = 0; allOpenChest = 0;
        PlayerPrefs.DeleteKey(id + "_slot_allKills");
        PlayerPrefs.DeleteKey(id + "_slot_allPassRoom");
        PlayerPrefs.DeleteKey(id + "_slot_allOpenChest");
        PlayerPrefs.DeleteKey(id + "_slot_allDeath");
    }
    public void SaveDungeonStats()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_allKills", allKills);
        PlayerPrefs.SetInt(id + "_slot_allPassRoom", allPassRoom);
        PlayerPrefs.SetInt(id + "_slot_allOpenChest", allOpenChest);
        PlayerPrefs.SetInt(id + "_slot_allDeath", allDeath);
    }
}
