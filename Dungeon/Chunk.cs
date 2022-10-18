using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chunk : Photon.MonoBehaviour
{
    public RoomControl GetRoomControl => this.gameObject.GetComponent<RoomControl>();
    [SerializeField] private Vector2Int _positionCoordinate; // позиция комнаты на карте Чанков
    // Порталы
    // [SerializeField] private List<NavMeshLink> _portals = new List<NavMeshLink>();
    [SerializeField] private List<GameObject> _doorBlock = new List<GameObject>();
    public List<DoorBlock> DoorBlocks = new List<DoorBlock>();

    [SerializeField] private bool _isSupportRotation; public bool IsSupportRotation => _isSupportRotation; // поддерживает ли эта комната повороты
    [SerializeField] private bool _isSupportBiggest; public bool IsSupportBiggest => _isSupportBiggest; // поддерживает ли присоединение к комнатам без стен
    public void SetPositionCoordinate(Vector2Int vector2) { _positionCoordinate = vector2; }
    // Локальные объекты чанка
    public GameObject DoorU;
    public GameObject DoorR;
    public GameObject DoorL;
    public GameObject DoorD;

    public GameObject AllWallU;
    public GameObject AllWallR;
    public GameObject AllWallL;
    public GameObject AllWallD;

    public GameObject WallU;
    public GameObject WallR;
    public GameObject WallL;
    public GameObject WallD;

    [SerializeField] private GameObject _fog; // Туман над комнатой, изчезает после первого захода в комнату
    //[SerializeField] private MeshRenderer _floorMesh; // пол комнаты
    // все соседние комнаты
    [SerializeField] private List<Chunk> _nearConnectedRooms; public void AddNearConnectedRoom(Chunk chunk) { _nearConnectedRooms.Add(chunk); }

    private bool _isEnterPlayer; public bool IsPlayerInter => _isEnterPlayer;
    private bool server_isEnterPlayer;
    private void Start()
    {
        SwitchDoorBlocks(false);
        if (!_isSupportBiggest) SetDafaultObjStatus();
    }
    private void SetDafaultObjStatus()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (DoorU != null) DoorU.SetActive(false);
            if (DoorR != null) DoorR.SetActive(false);
            if (DoorL != null) DoorL.SetActive(false);
            if (DoorD != null) DoorD.SetActive(false);

            AllWallU.SetActive(true);
            AllWallR.SetActive(true);
            AllWallL.SetActive(true);
            AllWallD.SetActive(true);

            WallU.SetActive(true);
            WallR.SetActive(true);
            WallL.SetActive(true);
            WallD.SetActive(true);
        }
    }
    public void SendAllRotation()
    {
        photonView.RPC("RotateRandom", PhotonTargets.AllBuffered);
    }
    public void SendAllThisChunkData()
    {
        bool[] data = GetChunkData();
        photonView.RPC("SetChunk", PhotonTargets.AllBuffered, (bool[])data);
        float[] pos = GetChunkPos();
        photonView.RPC("SetChunkPosition", PhotonTargets.AllBuffered, (float[])pos);
        //float[] rot = GetChunkRot();
        //photonView.RPC("SetChunkRotation", PhotonTargets.AllBuffered, (float[])rot);
    }
    public void EnterPlayer()
    {
        if (PhotonNetwork.offlineMode) SetFog(false);
        else photonView.RPC("SetFog", PhotonTargets.AllBuffered, (bool)false);
    }
    [PunRPC]
    public void BlockDoors(bool isIgnorePassAcrossDoor = false, bool isActivatePassAcrossDoor = true)
    {
        if (!isIgnorePassAcrossDoor)
        {
            foreach (DoorBlock door in DoorBlocks)
            {
                if (!door.IsEnterPlayerInDoor) { door.SwitchDoor(true, isActivatePassAcrossDoor); }
                else door.SwitchDoor(false, isActivatePassAcrossDoor);
            }
        }
        else
        {
            foreach (DoorBlock door in DoorBlocks)
            {
                door.SwitchDoor(false, isActivatePassAcrossDoor);
            }
        }

    }
    [PunRPC]
    public void UnlockDoors()
    {
        foreach (DoorBlock door in DoorBlocks)
        {
            door.SwitchDoor(false);
        }
    }
    public void UnlockNearRooms()
    {
        foreach (Chunk chunk in _nearConnectedRooms)
        {
            if (PhotonNetwork.offlineMode) chunk.UnlockDoors();
            else chunk.photonView.RPC("UnlockDoors", PhotonTargets.All);
        }
    }
    public void BlockNearRooms()
    {
        foreach (Chunk chunk in _nearConnectedRooms)
        {
            bool isAvaibleToTeleportDoors = true;
            if (chunk.GetRoomControl.GetRoomType == RoomType.Ambush) isAvaibleToTeleportDoors = false;
            if (!chunk.IsPlayerInter)
            {
                if (PhotonNetwork.offlineMode) chunk.BlockDoors(false, isAvaibleToTeleportDoors);
                else chunk.photonView.RPC("BlockDoors", PhotonTargets.All, (bool)false, (bool)isAvaibleToTeleportDoors);
            }
        }
    }
    public void EnterFirst()
    {
        _isEnterPlayer = true;
        //_floorMesh.material = BasePrefs.instance.GetGreyMaterial;
        if (_fog.transform.GetChild(0) != null) Destroy(_fog.transform.GetChild(0).gameObject);
        server_isEnterPlayer = true;
        if (_isSupportBiggest && _nearConnectedRooms != null)
        {
            foreach (Chunk fChunk in _nearConnectedRooms)
            {
                // снятие тумана у соседней комнаты, где IsSupportBiggest = true
                if (fChunk.IsSupportBiggest) fChunk.SwitchFog(false);
            }
        }
    }
    [PunRPC]
    public void SwitchDoorBlocks(bool status)
    {
        foreach (GameObject obj in _doorBlock)
        {
            obj.GetComponent<NavMeshObstacle>().size = new Vector3(2f, 2f, 1.1f);
            obj.GetComponent<NavMeshObstacle>().carving = true;
            obj.SetActive(status);
        }
    }
    [PunRPC]
    public void SetChunkPosition(float[] pos)
    {
        transform.position = new Vector3(pos[0], pos[1], pos[2]);
    }
    [PunRPC]
    public void SetChunkRotation(float[] rot)
    {
        transform.Rotate(rot[0], rot[1], rot[2]);
    }

    [PunRPC]
    public void SetChunk(bool[] data)
    {
        if (DoorU != null) DoorU.SetActive(data[0]);
        if (DoorR != null) DoorR.SetActive(data[1]);
        if (DoorL != null) DoorL.SetActive(data[2]);
        if (DoorD != null) DoorD.SetActive(data[3]);

        AllWallU.SetActive(data[4]);
        AllWallR.SetActive(data[5]);
        AllWallL.SetActive(data[6]);
        AllWallD.SetActive(data[7]);

        WallU.SetActive(data[8]);
        WallR.SetActive(data[9]);
        WallL.SetActive(data[10]);
        WallD.SetActive(data[11]);
    }
    [PunRPC]
    private void SetFog(bool status)
    {
        if (_fog != null) _fog.SetActive(status);
        //if (_floorMesh != null) _floorMesh.material = BasePrefs.instance.GetGreyMaterial;
    }
    public float[] GetChunkPos()
    {
        float[] pos = new float[3]; pos[0] = transform.position.x; pos[1] = transform.position.y; pos[2] = transform.position.z;
        return pos;
    }
    public float[] GetChunkRot()
    {
        float[] rot = new float[3]; rot[0] = transform.rotation.x; rot[1] = transform.rotation.y; rot[2] = transform.rotation.z;
        return rot;
    }
    public bool[] GetChunkData()
    {
        bool[] data = new bool[12];

        if (DoorU != null) data[0] = DoorU.activeSelf; else data[0] = false;
        if (DoorR != null) data[1] = DoorR.activeSelf; else data[1] = false;
        if (DoorL != null) data[2] = DoorL.activeSelf; else data[2] = false;
        if (DoorD != null) data[3] = DoorD.activeSelf; else data[3] = false;

        data[4] = AllWallU.activeSelf;
        data[5] = AllWallR.activeSelf;
        data[6] = AllWallL.activeSelf;
        data[7] = AllWallD.activeSelf;

        data[8] = WallU.activeSelf;
        data[9] = WallR.activeSelf;
        data[10] = WallL.activeSelf;
        data[11] = WallD.activeSelf;

        return data;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            server_isEnterPlayer = _isEnterPlayer;
            stream.SendNext(server_isEnterPlayer);
        }
        if (stream.isReading)
        {
            server_isEnterPlayer = (bool)stream.ReceiveNext();
            _isEnterPlayer = server_isEnterPlayer;
        }
    }
    public IEnumerator CheckDoors(Vector2Int _selectDirect, Chunk _selectRoom)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        {
            // Выключаются две двери(одну в текущей комнате, и одну в cоседней) в зависимости от направления
            if (_selectDirect == Vector2Int.up) // если подсоединяемся к верхней комнате
            {
                WallU.SetActive(false);
                DoorU.SetActive(true);
            }
            else if (_selectDirect == Vector2Int.down)
            {
                WallD.SetActive(false);
                DoorD.SetActive(true);
            }
            else if (_selectDirect == Vector2Int.left)
            {

                WallL.SetActive(false);
                DoorL.SetActive(true);
            }
            else if (_selectDirect == Vector2Int.right)
            {

                WallR.SetActive(false);
                DoorR.SetActive(true);
            }
        }
    }
    public void SwitchFog(bool status)
    {
        if (_fog != null)
        {
            if (PhotonNetwork.offlineMode) _fog.SetActive(status);
            else photonView.RPC("SetFog", PhotonTargets.AllBuffered, status);
        }
    }
    public void CheckType(Vector2Int _selectDirect, Chunk _selectRoom)
    {
        // Выключаются две двери(одну в текущей комнате, и одну в оседней) в зависимости от направления
        if (_selectDirect == Vector2Int.up && _selectRoom.IsSupportBiggest == true && IsSupportBiggest == true) // если подсоединяемся к верхней комнате
        {
            AllWallU.SetActive(false);
            _selectRoom.AllWallD.SetActive(false);
        }
        else if (_selectDirect == Vector2Int.down && _selectRoom.IsSupportBiggest == true && IsSupportBiggest == true)
        {
            AllWallD.SetActive(false);
            _selectRoom.AllWallU.SetActive(false);
        }
        else if (_selectDirect == Vector2Int.left && _selectRoom.IsSupportBiggest == true && IsSupportBiggest == true)
        {
            AllWallL.SetActive(false);
            _selectRoom.AllWallR.SetActive(false);
        }
        else if (_selectDirect == Vector2Int.right && _selectRoom.IsSupportBiggest == true && IsSupportBiggest == true)
        {
            AllWallR.SetActive(false);
            _selectRoom.AllWallL.SetActive(false);
        }
        // создание списка всех соседних комнат для текущей комнаты
        _nearConnectedRooms.Add(_selectRoom);
        _selectRoom.AddNearConnectedRoom(this);
    }
    public IEnumerator CheckWalls(Vector2Int _selectDirect, Chunk _selectRoom)
    {
        yield return new WaitForSecondsRealtime(0.05f);
        {
            if (_selectDirect == Vector2Int.up) // если подсоединяемся к верхней комнате
            {
                _selectRoom.AllWallD.SetActive(false);
            }
            else if (_selectDirect == Vector2Int.down)
            {
                _selectRoom.AllWallU.SetActive(false);
            }
            else if (_selectDirect == Vector2Int.left)
            {
                _selectRoom.AllWallR.SetActive(false);
            }
            else if (_selectDirect == Vector2Int.right)
            {
                _selectRoom.AllWallL.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void RotateRandom()
    {
        transform.Rotate(0, 90, 0);
        GameObject temp1 = DoorL;
        GameObject temp2 = AllWallL;
        GameObject temp3 = WallL;
        DoorL = DoorD; AllWallL = AllWallD; WallL = WallD;
        DoorD = DoorR; AllWallD = AllWallR; WallD = WallR;
        DoorR = DoorU; AllWallR = AllWallU; WallR = WallU;
        DoorU = temp1; AllWallU = temp2; WallU = temp3;
    }
}
