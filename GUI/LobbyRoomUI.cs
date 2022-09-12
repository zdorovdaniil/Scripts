using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyRoomUI : Photon.MonoBehaviour
{
    public static LobbyRoomUI instance;

    [SerializeField] private Transform _roomListTrf;
    [SerializeField] private GameObject _roomButtonPrefab;
    [SerializeField] private TMP_Text _numRoomsInRegion;
    [SerializeField] private TMP_Text _regionName;

    [SerializeField] private TextMeshProUGUI _textNameRoom;
    [SerializeField] private TextMeshProUGUI _textNumPlayers;

    [SerializeField] private TMP_Text _hostName;
    [SerializeField] private TMP_Text _secondPlayerName;
    [SerializeField] private Text _numRoomsInDungeon;
    [SerializeField] private Text _dungeonLevel;

    [SerializeField] private Transform _roomLobby;
    [SerializeField] private Transform _lostRoomsLobby;
    [SerializeField] private Transform _buttonCreateRoom;
    [SerializeField] private List<Transform> _disableClientObjects;
    [SerializeField] private Transform _kickButton;
    [SerializeField] private Transform _hostLine;
    [SerializeField] private Transform _clientLine;
    private string server_hostName;
    private string server_numRoomsInDungeon;
    private string server_dungeonLevel;

    private RoomInfo _selectedRoomInfo;
    public RoomInfo GetRoomInfo => _selectedRoomInfo;


    private float _timer;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f) { _timer = 0; SetRoomsList(); }
    }
    public void ClearFields()
    {
        _hostName.text = "";
        _secondPlayerName.text = "";
        _textNameRoom.text = "";
        _textNumPlayers.text = "";
    }
    [PunRPC]
    public void ClearSecondNickName()
    {
        _secondPlayerName.text = "";
        _kickButton.gameObject.SetActive(false);
    }
    private void Start()
    {
        instance = this;
    }
    public void StartCreateRoom()
    {
        ClearFields();
        SwitchClientObjects(true);
        _hostName.text = PhotonNetwork.playerName;
        int id = PlayerPrefs.GetInt("activeSlot");
        _numRoomsInDungeon.text = PlayerPrefs.GetInt("numRooms").ToString();
        _dungeonLevel.text = PlayerPrefs.GetInt(id + "_slot_dungeonLevel").ToString();
        _buttonCreateRoom.gameObject.SetActive(true);
        _hostLine.gameObject.SetActive(true);
        _clientLine.gameObject.SetActive(false);
        _kickButton.gameObject.SetActive(false);
        OpenRoomLobby();
    }
    public void SetSingleplayerRoom()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        _hostName.text = PlayerPrefs.GetString(id + "_slot_nickName");
        _numRoomsInDungeon.text = PlayerPrefs.GetInt("numRooms").ToString();
        _dungeonLevel.text = PlayerPrefs.GetInt(id + "_slot_dungeonLevel").ToString();
        _clientLine.gameObject.SetActive(false);
        _kickButton.gameObject.SetActive(false);
        OpenRoomLobby();
    }
    public void SetRoomInfo()
    {
        ClearFields();
        if (PhotonNetwork.isNonMasterClientInRoom)
        {
            SwitchClientObjects(false);
            _secondPlayerName.text = PhotonNetwork.playerName;
            photonView.RPC("SetSecondPlayerName", PhotonTargets.All, (string)PhotonNetwork.playerName);
            _buttonCreateRoom.gameObject.SetActive(false);
            _hostLine.gameObject.SetActive(false);
            _clientLine.gameObject.SetActive(true);
            _kickButton.gameObject.SetActive(false);
        }
        OpenRoomLobby();
    }
    [PunRPC]
    public void SetSecondPlayerName(string name)
    {
        _secondPlayerName.text = name;
        if (PhotonNetwork.isMasterClient) { _kickButton.gameObject.SetActive(true); } else { _kickButton.gameObject.SetActive(false); }
    }
    public void OpenRoomLobby()
    {
        _roomLobby.gameObject.SetActive(true);
        _lostRoomsLobby.gameObject.SetActive(false);
    }
    public void OpenlostRoomsLobby()
    {
        ClearFields();
        _roomLobby.gameObject.SetActive(false);
        _lostRoomsLobby.gameObject.SetActive(true);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            server_hostName = _hostName.text;
            stream.SendNext(server_hostName);
            server_numRoomsInDungeon = _numRoomsInDungeon.text;
            stream.SendNext(server_numRoomsInDungeon);
            server_dungeonLevel = _dungeonLevel.text;
            stream.SendNext(server_dungeonLevel);
        }
        if (stream.isReading)
        {

            _hostName.text = (string)stream.ReceiveNext();
            _numRoomsInDungeon.text = (string)stream.ReceiveNext();
            _dungeonLevel.text = (string)stream.ReceiveNext();
        }
    }
    private void SwitchClientObjects(bool status)
    {
        foreach (Transform obj in _disableClientObjects)
        { obj.gameObject.SetActive(status); }
    }

    public void SetRoomInfoField(RoomInfo roomInfo)
    {
        _selectedRoomInfo = roomInfo;
        _textNameRoom.text = _selectedRoomInfo.Name;
        _textNumPlayers.text = _selectedRoomInfo.PlayerCount.ToString() + " / " + _selectedRoomInfo.MaxPlayers.ToString();
    }
    public void SetRoomsList()
    {
        for (int i = 0; i < _roomListTrf.childCount; i++)
        {
            Destroy(_roomListTrf.GetChild(i).gameObject);
        }
        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            Instantiate(_roomButtonPrefab, _roomListTrf).GetComponent<ButtonRoom>().SetUp(room);
        }
        _numRoomsInRegion.text = PhotonNetwork.GetRoomList().Length.ToString();
        _regionName.text = PhotonNetwork.CloudRegion.ToString();


    }
}
