﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{ Default, Big, Final, BeetweenRooms, End, MiniBoss, Start, Random }
public class RoomControl : Photon.MonoBehaviour
{
    [SerializeField] private RoomType _roomType; public RoomType GetRoomType => _roomType;
    [SerializeField] private List<Transform> _teleportPointRoom; 
    // места спавна противников
    [SerializeField] private List<Transform> _spawnPointsForEnemy = new List<Transform>();
    // противники которые могут появиться в _spawnPointsForEnemy
    public Enemy[] SpawnEnemies;
    // количество противников которые заспавняться в комнате
    [SerializeField] private int _countEnemy; public int GetCountEnemy => _countEnemy;
    // определяет, будут ли появляться объекты после смерти всех противников в комнате
    [SerializeField] private bool _isActivingObjects;
    // объекты, что будут активироваться после смерти всех противников в комнате
    [SerializeField] private List<Transform> _activingObjAfterClear = new List<Transform>();
    [SerializeField] private List<Transform> _activingObjAfterEnter = new List<Transform>();
    [Multiline(2)] public string RoomDescription;
    private Chunk chunk;
    [Space]
    [Header("Settings Room")]
    // настраивает, является ли комната засадой. После захождения в комнату, дверь обратно блокируется
    [SerializeField] private bool _isAmbushRoom;
    // время через которое закроется дверь в засаду
    [SerializeField] private float _doorClosingTime = 0.5f;
    [Space]
    [Header("Realtime room info")]
    // список игроков находящихся в комнате
    public List<GameObject> ListPlayers = new List<GameObject>();
    // определяет, включены ли объекты после смерти противников
    [SerializeField] private bool _isActivedObject = false;
    public int CountDefeatEnemy;
    private void Start()
    {
        chunk = GetComponent<Chunk>();
        foreach (Transform tr in _activingObjAfterEnter)
        {
            if (tr != null) tr.gameObject.SetActive(false);
        }
        StartCoroutine(AddRoomToDungeonObjects());
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {}
    // событие вызываемое после уничтожения 1 противника в комнате
    public void DefeatEnemyInRoom()
    {
        CountDefeatEnemy += 1;
        if (!PhotonNetwork.offlineMode) photonView.RPC("GetCountDefeatedEnemyes", PhotonTargets.Others, (int)CountDefeatEnemy);
        UpdateParametrs();
    }
    [PunRPC] public void GetCountDefeatedEnemyes(int count)
	{
        CountDefeatEnemy = count;
        UpdateParametrs();
    }
    public void UpdateParametrs()
    {
        if (CountDefeatEnemy >= _countEnemy && _countEnemy > 0)
        {
            if (_isActivingObjects && _isActivedObject == false)
            {
                Debug.Log("RoomClear");
                SwitchGameObjectsAfterDefeatEnemyes(true);
                chunk.UnlockDoors();
                chunk.UnlockNearRooms();
                SwitchDoorsInChunk(false);
            }
        }
        else
        {
            SwitchGameObjectsAfterDefeatEnemyes(false);
        }
        if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		{
            chunk.SendAllThisChunkData();
        }
    }

    private void SwitchGameObjectsAfterDefeatEnemyes(bool status)
    {
        // подтверждение что объекты появились
        _isActivedObject = status;
        foreach (Transform obj in _activingObjAfterClear)
        {
            if (obj != null) obj.gameObject.SetActive(status);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!ListPlayers.Contains(other.gameObject)) {ListPlayers.Add(other.gameObject);}
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.SetCurRoom(this);
            RoomControl roomControl = GetComponent<RoomControl>();
            DungeonStats.Instance.CheckIsEnteredRoom(roomControl);
            // Когда игрок в первый раз входит в комнату
            EnterToRoom();
            chunk.EnterPlayer();
            UpdateParametrs();
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            ListPlayers.Remove(other.gameObject);
            UpdateParametrs();
        }
    }
    private void EnterToRoom()
    {
        if (!chunk.IsPlayerInter)
        {
            StartCoroutine(SpawnEnemy());
            chunk.EnterPlayer();
            chunk.EnterFirst();
            if (_countEnemy >= 1)
			{
                chunk.BlockDoors();
                chunk.BlockNearRooms();
            }
        }
        if (_isAmbushRoom && ListPlayers.Count >= 1 && CountDefeatEnemy < GetCountEnemy)
        {
            StartCoroutine(CloseDoors());
        }
        foreach (Transform tr in _activingObjAfterEnter)
		{
            if (tr != null) tr.gameObject.SetActive(true);
        }
    }
    public IEnumerator CloseDoors()
    {
        yield return new WaitForSecondsRealtime(_doorClosingTime);
        {
            if (ListPlayers.Count >= 1) SwitchDoorsInChunk(true);
        }
    }
    public Vector3 GetRandomTeleportPointRoom()
    {
        int num = Random.Range(0, _teleportPointRoom.Count);
        return _teleportPointRoom[num].transform.position;
    }
    public IEnumerator SpawnEnemy()
    {
        yield return new WaitForSecondsRealtime(_doorClosingTime);
        {
            if (_spawnPointsForEnemy.Count != 0)
            {
                Debug.Log("Спавн противников");
                foreach (Transform point in _spawnPointsForEnemy)
                {
                    int en = Random.Range(0, SpawnEnemies.Length);
                    GameObject _enemy = GameManager.SpawnEnemyIn(point, SpawnEnemies[en].PrefabEnemy);
                    if (_enemy != null)
                    {
                        EnemyStats _enemyStats = _enemy.GetComponent<EnemyStats>();
                        _enemyStats.BelongRoom = this.gameObject.GetComponent<RoomControl>();
                    }
                }
            }
        }
    }
    public IEnumerator AddRoomToDungeonObjects()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        {
            DungeonObjects.Instance.AddChunk(chunk);
        }
    }
    public void DeathInRoom(GameObject obj)
    {
        ListPlayers.Remove(obj);
        if (ListPlayers.Count <= 0) SwitchDoorsInChunk(false);
        UpdateParametrs();
    }
    public void SwitchDoorsInChunk(bool status)
    {
        if (!PhotonNetwork.offlineMode) chunk.photonView.RPC("SwitchDoorBlocks", PhotonTargets.All, (bool)status);
        else chunk.SwitchDoorBlocks(status);
    }


}