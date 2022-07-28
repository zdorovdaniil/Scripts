using System.Collections.Generic;
using UnityEngine;

public class DungeonStats : Photon.MonoBehaviour
{
    #region Singleton
    private void Awake() { Instance = this; }

    #endregion
    public static DungeonStats Instance;

    // Статистика за подземелье
    public int numEnemyInDungeon; // общее количество противников в подземелье
    public int numRoomsInDungeon; // общее количество комнат в подземелье
    public int numChestsInDungeon; // общее количество сундуков в подземелье

    public int passedRoom; // количество пройденых комнат в подземелье
    public int curKills; // количество уничтоженных врагов в тукущем подземелье
    public int numOpenChest; // кол-во открытых сундуков
    public List<RoomControl> interedRooms = new List<RoomControl>(); // список комнат, где были игроки

    // Собираемая за всю игру статистика
    public int allPassRoom; // количество пройденных комнат за всю игру
    public int allKills; // уничтожено врагов за всю игру
    public int allOpenChest; // всего открытых сундуков
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
    }
    public void ResetDungeonStats()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        allKills = 0; allPassRoom = 0; allOpenChest = 0;
        PlayerPrefs.DeleteKey(id + "_slot_allKills");
        PlayerPrefs.DeleteKey(id + "_slot_allPassRoom");
        PlayerPrefs.DeleteKey(id + "_slot_allOpenChest");
    }
    public void SaveDungeonStats()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_allKills", allKills);
        PlayerPrefs.SetInt(id + "_slot_allPassRoom", allPassRoom);
        PlayerPrefs.SetInt(id + "_slot_allOpenChest", allOpenChest);
    }
}
