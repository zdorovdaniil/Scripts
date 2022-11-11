using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Скрипт отвечает за сетевую инциализацию начала игры
/// </summary>
public class GameManager : Photon.MonoBehaviour
{
    public static GameManager Instance; private void Awake() { Instance = this; }
    private DungeonStats _dungeonStats;
    public int GetCountPlayers => PhotonPlayers.Count;

    public GameObject tempView;

    public GameObject PlayerPrefab;
    public List<PlayerStats> PhotonPlayers = new List<PlayerStats>();

    public Transform SpawnPointOwner;
    public Transform SpawnPointClient;
    [SerializeField] private Transform _errorPoint;

    public ChunkPlacer chunkPlacer;
    private float _timerDungeonGoing; public float GetTimeDungeonGoing => _timerDungeonGoing;
    [SerializeField] private GameObject _allMapCamera; // камера которая делает карту
    [Header("Cheats")]
    [SerializeField] private bool _disableFog; public bool IsDisabledFog => _disableFog;
    [SerializeField] private bool _isCreatingDungeon = true; public bool IsCreatingDungeon => _isCreatingDungeon;

    private void Start()
    {
        Application.targetFrameRate = 60;
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
            { player.photonView.RPC("AddBuffPlayer", PhotonTargets.All, (int)id); }
        }
        else { PhotonPlayers[0].AddBuffPlayer(id); }

    }
    private void CreatePlayerPrefab()
    {
        GameObject obj;
        if (PhotonNetwork.isMasterClient)
        {
            obj = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPointOwner.position, SpawnPointOwner.rotation, 0);
            obj.name = "1Player";
            DungeonStats.Instance.SetDungeonLevel(ProcessCommand.GetDungeonLevel);
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
        if (PhotonNetwork.isMasterClient && !PhotonNetwork.offlineMode) photonView.RPC("OnDungeonGenerated", PhotonTargets.AllBuffered);
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
            chunkPlacer.isMyltiplayer = true;
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("SetDungeonLevel", PhotonTargets.AllBuffered, (int)ProcessCommand.GetDungeonLevel);
                StartCoroutine(chunkPlacer.StartRespawnRooms(this));
            }
        }
        else
        {
            DungeonStats.Instance.SetDungeonLevel(ProcessCommand.GetDungeonLevel);
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
        photonView.RPC("LeaveDungeon", PhotonTargets.Others);
        GameManager.Instance.LeaveDungeon();
    }
    // Получение уровня подземелья от владельца сервера
    [PunRPC]
    public void SetDungeonLevel(int level)
    { DungeonStats.Instance.SetDungeonLevel(level); }

    [PunRPC]
    public void LeaveDungeon()
    {
        StopCoroutine(UpdateTimer());
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


}

