using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
    public static GameManager Instance;
    private DungeonStats _dungeonStats;
    [SerializeField] private int _dungeonLevel = 1; // уровень подземелья
    public int GetDungeonLevel => _dungeonLevel;
    public int GetCountPlayers => PhotonPlayers.Count;

    public GameObject tempView;

    public GameObject PlayerPrefab;
    public List<PlayerStats> PhotonPlayers = new List<PlayerStats>();

    public Transform SpawnPointOwner;
    public Transform SpawnPointClient;
    [SerializeField] private Transform _errorPoint;
    [SerializeField] private DungeonRules _dungeonRules; public DungeonRules GetRules => _dungeonRules;
    public ChunkPlacer chunkPlacer;
    private float _timerDungeonGoing; public float GetTimeDungeonGoing => _timerDungeonGoing;
    [SerializeField] private GameObject _allMapCamera; // камера которая делает карту
    [Header("Cheats")]
    [SerializeField] private bool _disableFog; public bool IsDisabledFog => _disableFog;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        //SceneLoadingUI.Instance.OpenLoadingUI("creating rooms");
        InterstitialAd.S.LoadAd();
        RewardedAds.S.LoadAd();
        _dungeonStats = DungeonStats.Instance;
        _allMapCamera.SetActive(false);
        GenerateDungeon();
        CreatePlayerPrefab();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }

    public void SendAllBuff(int id)
    {
        if (!PhotonNetwork.offlineMode)
        {
            foreach (PlayerStats player in PhotonPlayers)
            { player.photonView.RPC("AddBuff", PhotonTargets.All, (int)id); }
        }
        else { PhotonPlayers[0].AddBuffPlayer(id); }

    }
    public bool CheckDungeonLevel()
    {
        if (PhotonNetwork.isMasterClient || PhotonNetwork.isNonMasterClientInRoom)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (_dungeonLevel <= 4) { UpDungeonLevel(); return true; }
                else { Debug.Log("Dungeon level is already max"); return false; }
            }
            else { Debug.Log("You no master server"); return false; }
        }
        else
        {
            if (_dungeonLevel <= 4) { UpDungeonLevel(); return true; }
            else { Debug.Log("Dungeon level is already max"); return false; }
        }
    }
    private void UpDungeonLevel()
    {
        _dungeonLevel += 1;
        PlayerPrefs.SetInt(PlayerPrefs.GetInt("activeSlot") + "_slot_dungeonLevel", _dungeonLevel);
    }
    private void CreatePlayerPrefab()
    {
        GameObject obj;
        if (PhotonNetwork.isMasterClient)
        {
            obj = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPointOwner.position, SpawnPointOwner.rotation, 0);
            obj.name = "1Player";
            KnowDungeonLevelFromSave();
        }
        // появление клиента (2 игрока)
        else if (PhotonNetwork.isNonMasterClientInRoom)
        {
            obj = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPointClient.position, SpawnPointClient.rotation, 0);
            obj.name = "2Player";
        }
        else
        {
            PhotonNetwork.offlineMode = true;
            if (chunkPlacer != null)
            {
                chunkPlacer.isMyltiplayer = false;
            }
            obj = Instantiate(PlayerPrefab, SpawnPointOwner.position, SpawnPointOwner.rotation);
        }
    }

    [PunRPC]
    public void OnDungeonGenerated()
    {
        StartCoroutine(LoadingDungeonDataWithDelay());
    }
    public void GetReport(string reportStatus)
    {
        if (reportStatus == "dungeonLeave")
        { LeaveDungeon(); }
    }
    [PunRPC]
    public void DungeonFailGenerated()
    {
        MsgBoxUI.Instance.Show(this.gameObject, "Fail Generation", "Dungeon creation error. Try creating the dungeon agein", "dungeonLeave", true);
        if (PhotonNetwork.isMasterClient) photonView.RPC("DungeonFailGenerated", PhotonTargets.Others);
    }

    public void DungeonSuccessGenerated()
    {
        StartCoroutine(UpdateTimer());
        SetDungeonStats();
        if (!PhotonNetwork.offlineMode) photonView.RPC("OnDungeonGenerated", PhotonTargets.AllBuffered);
        else { OnDungeonGenerated(); }
    }
    [PunRPC]
    public void SendLoadScene()
    {
        PhotonNetwork.LoadLevel("Level_2");
    }
    private void GenerateDungeon()
    {
        // в myltiplayer подземелье создает вледелец сервера
        if (PhotonNetwork.isMasterClient || PhotonNetwork.isNonMasterClientInRoom)
        {
            if (chunkPlacer != null)
            {
                chunkPlacer.isMyltiplayer = true;
                if (PhotonNetwork.isMasterClient)
                {
                    KnowDungeonLevelFromSave();
                    photonView.RPC("SetDungeonLevel", PhotonTargets.AllBuffered, (int)_dungeonLevel);
                    StartCoroutine(chunkPlacer.StartRespawnRooms(this));
                }
                else
                {
                    // клиент
                }
            }
        }
        else
        {
            KnowDungeonLevelFromSave();
            StartCoroutine(chunkPlacer.StartRespawnRooms(this));
        }

    }
    private IEnumerator UpdateTimer()
    {
        yield return new WaitForSecondsRealtime(1f);
        {
            _timerDungeonGoing += 1;
            if (!PhotonNetwork.offlineMode)
            { photonView.RPC("SetDungeonData", PhotonTargets.Others, (int)_dungeonStats.numRoomsInDungeon, (int)_dungeonStats.numEnemyInDungeon, (float)_timerDungeonGoing); }
            StartCoroutine(UpdateTimer());
        }
    }
    private IEnumerator LoadingDungeonDataWithDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        {
            SceneLoadingUI.Instance.CloseLoadingUI();
            PlayerQuest.instance.InstainceQuests();
            DungeonObjects.Instance.UpdateParametrsRooms();
            DungeonObjects.Instance.UpdatePortalsInRooms();
        }
    }
    [PunRPC]
    private void SetDungeonData(int numRooms, int countEnemyDungeon, float timerValue)
    {
        _dungeonStats.numRoomsInDungeon = numRooms;
        _dungeonStats.numEnemyInDungeon = countEnemyDungeon;
        _timerDungeonGoing = timerValue;
    }
    public void SetDungeonStats()
    {
        _dungeonStats.numEnemyInDungeon = chunkPlacer.GetCountEnemyes;
        _dungeonStats.numRoomsInDungeon = chunkPlacer.NumRooms;
    }
    public void SendAllLeaveDungeon()
    {
        photonView.RPC("LeaveDungeon", PhotonTargets.AllBuffered);
    }
    // Получение уровня подземелья от владельца сервера
    [PunRPC]
    public void SetDungeonLevel(int level)
    { _dungeonLevel = level; }

    [PunRPC]
    public void LeaveDungeon()
    {

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }
    public PlayerStats GetPlayerIndex(int index)
    {
        return index < PhotonPlayers.Count ? PhotonPlayers[index] : null;
    }
    public void SwitchAllMapCamera(bool status)
    {
        _allMapCamera.SetActive(status);
    }
    public void AddPlayer(PlayerStats _stats)
    {
        PhotonPlayers.Add(_stats);
    }
    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        LogUI.Instance.Loger(player.NickName + " was connected");
    }
    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        LogUI.Instance.Loger(player.NickName + " was disconnected");
    }
    static public GameObject SpawnEnemyIn(Transform pos, GameObject prefab)
    {
        // если singleplayer
        if (PhotonNetwork.offlineMode == true)
        {
            return Instantiate(prefab, pos.position, pos.rotation);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            return PhotonNetwork.Instantiate(prefab.name, pos.position, pos.rotation, 0);
        }
        else return null;
    }
    static public GameObject SpawnDrop(Transform pos, GameObject prefab)
    {
        // если singleplayer
        if (PhotonNetwork.offlineMode == true)
        {
            return Instantiate(prefab, pos.position, pos.rotation);
        }
        // если myltiplayer
        else if (PhotonNetwork.isMasterClient)
        {
            return PhotonNetwork.Instantiate(prefab.name, pos.position, pos.rotation, 0);
        }
        else return null;
    }
    static public GameObject SpawnArrowIn(Transform pos, GameObject prefab)
    {
        // если singleplayer
        if (PhotonNetwork.offlineMode == true)
        {
            return Instantiate(prefab, pos.position, pos.rotation);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            return PhotonNetwork.Instantiate(prefab.name, pos.position, pos.rotation, 0);
        }
        else return null;
    }
    // установка _dungeonLevel владельцем подземелья
    private void KnowDungeonLevelFromSave()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        _dungeonLevel = PlayerPrefs.GetInt(id + "_slot_dungeonLevel");
    }
}

