using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// скрипт назнает ник игрока и устанавливает подключение к комнате и лобби
public class NetworkCreateGame : Photon.MonoBehaviour
{
    public Text LogText;
    public InputField NickNameField;
    [SerializeField] private Transform _enterPlayerName;
    private string _nickName;
    [SerializeField] private bool _isConnectedMasterServer;
    [SerializeField] private bool _isConnectedToLobby;
    private int _curRegion;
    [SerializeField] private TMP_Dropdown _regionDropdown;
    [SerializeField] private ServerStatusUI _serverStatsUI;
    private CloudRegionCode _regionCode = 0;
    private void Start()
    {
        if (PhotonNetwork.connected) { PhotonNetwork.Disconnect(); }
        SceneLoadingUI.Instance.CloseLoadingUI();
        PhotonNetwork.gameVersion = "1";
    }
    public void CheckPlayerName(bool isAnyway = false)
    {
        if (!_isConnectedMasterServer && !_isConnectedToLobby)
        {
            int id = PlayerPrefs.GetInt("activeSlot");
            if ((!PlayerPrefs.HasKey(id + "_slot_nickName") || isAnyway))
            { _enterPlayerName.gameObject.SetActive(true); }
            else
            {
                _nickName = PlayerPrefs.GetString(id + "_slot_nickName");
                NickNameField.text = _nickName;
                TryToConnectMasterServer();
            }
        }
    }
    public void SetRegionDropdown()
    {
        _regionDropdown.value = PlayerPrefs.GetInt("region");
    }
    public void ChangeServer(int dropDownValue)
    {
        DisconnecteFromServer();
        SetRegionCode(dropDownValue);
        PlayerPrefs.SetInt("region", dropDownValue);
        StartCoroutine(ConnectToServerWithDelay());
    }
    private void SetRegionCode(int code)
    {
        if (code == 0) { _regionCode = CloudRegionCode.eu; }
        else if (code == 1) { _regionCode = CloudRegionCode.ru; }
        else if (code == 2) { _regionCode = CloudRegionCode.rue; }
    }
    // попытка подключения к мастеру сервера
    public void TryToConnectMasterServer()
    {
        _enterPlayerName.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(NickNameField.text))
        {
            MsgBoxUI.Instance.ShowAttention("Text field for name is empty"); return;
        }
        int id = PlayerPrefs.GetInt("activeSlot");
        _nickName = NickNameField.text;
        PlayerPrefs.SetString(id + "_slot_nickName", _nickName);
        PhotonNetwork.playerName = _nickName;
        SetRegionCode(PlayerPrefs.GetInt("region"));
        PhotonNetwork.ConnectToRegion(_regionCode, PhotonNetwork.gameVersion);
        _enterPlayerName.gameObject.SetActive(false);
        _serverStatsUI.SetNewStatus("connecting . . .", Color.white);
        StopCoroutine(CheckConnectToServer());
        StartCoroutine(CheckConnectToServer());
    }
    private IEnumerator CheckConnectToServer()
    {
        bool isConnectFirsTime = false;
        yield return new WaitForSecondsRealtime(2f);
        {
            if (!_isConnectedMasterServer) isConnectFirsTime = true;
        }
        yield return new WaitForSecondsRealtime(1.5f);
        {
            if (!_isConnectedMasterServer && isConnectFirsTime)
            {
                MsgBoxUI.Instance.ShowAttention("Unable connect to the server. You may not be connected to the Internet or the server is not avaible. Online game will not be avaible.");
                _serverStatsUI.SetNewStatus("offline!", Color.blue);
            }
        }
    }
    // подключение к мастер серверу успешно
    public virtual void OnConnectedToMaster()
    {
        _curRegion = ((int)PhotonNetwork.CloudRegion);
        _isConnectedMasterServer = true;
        PhotonNetwork.JoinLobby();
    }

    // подключение к лобби успешно
    public virtual void OnJoinedLobby()
    {
        LobbyRoomUI.instance.SetRoomsList();
        _isConnectedToLobby = true;
        _serverStatsUI.SetNewStatus("connected!", Color.green);
    }
    public void DisconnecteFromServer()
    {
        _isConnectedMasterServer = false;
        _isConnectedToLobby = false;
        if (PhotonNetwork.connected) PhotonNetwork.Disconnect();
        _serverStatsUI.SetNewStatus("disconnected!", Color.red);
    }
    public void ClickCreateOwnRoom()
    {
        LobbyRoomUI.instance.ClearFields();
        if (_isConnectedMasterServer && _isConnectedToLobby)
        {
            string roomName = NickNameField.text.ToLower();
            PlayerPrefs.SetString("NickName", NickNameField.text);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 }, null);
        }
        else
        {
            LobbyRoomUI.instance.SetSingleplayerRoom();
        }

    }
    public void ClickKickOutFromRoom()
    {
        photonView.RPC("DisconnectFromRoom", PhotonTargets.Others);
        LobbyRoomUI.instance.ClearSecondNickName();
    }
    public void ClickDisconnectFromRoom()
    {
        if (!PhotonNetwork.connected)
        {
            LobbyRoomUI.instance.OpenlostRoomsLobby();
        }
        else if (PhotonNetwork.isMasterClient)
        { photonView.RPC("DisconnectFromRoom", PhotonTargets.All); }
        else { LobbyRoomUI.instance.photonView.RPC("ClearSecondNickName", PhotonTargets.Others); DisconnectFromRoom(); }
    }
    [PunRPC]
    public void DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
        LobbyRoomUI.instance.OpenlostRoomsLobby();
    }
    public void ClickCreateRoom()
    {
        Settings.Instance.LoadingSettings();
        if (_isConnectedMasterServer && _isConnectedToLobby)
        {
            if (PhotonNetwork.room != null)
            {
                LobbyRoomUI.instance.ClearFields();
                if (PhotonNetwork.room.PlayerCount >= 2 || Settings.Instance.GetIsAlwayesNetwork)
                {
                    PhotonNetwork.room.IsOpen = false;
                    photonView.RPC("SendLoadScene", PhotonTargets.All);
                }
                else { SinglePlayerStart(); }
            }
            else { SinglePlayerStart(); }
        }
        else { SinglePlayerStart(); }
    }
    private void SinglePlayerStart()
    {
        DisconnecteFromServer();
        SceneLoadingUI.Instance.OpenLoadingUI("loading dungeon", true);
        StartCoroutine(LoadLevelWithDelay(2, "Level_2"));
    }
    private IEnumerator LoadLevelWithDelay(float time, string levelName)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            SceneManager.LoadScene(levelName);
        }
    }

    [PunRPC]
    public void SendLoadScene()
    {
        GlobalSounds.Instance.SLadder();
        SceneLoadingUI.Instance.OpenLoadingUI("loading dungeon", true);
        PhotonNetwork.LoadLevel("Level_2");
    }
    public void JoinToRoom()
    {
        LobbyRoomUI.instance.ClearFields();
        RoomInfo roomInfo = LobbyRoomUI.instance.GetRoomInfo;
        if (roomInfo != null)
        {
            PhotonNetwork.playerName = NickNameField.text;
            PlayerPrefs.SetString("NickName", NickNameField.text);
            if (roomInfo.PlayerCount < 2) { PhotonNetwork.JoinRoom(roomInfo.Name); }
            else MsgBoxUI.Instance.ShowAttention("Room is full or already started");
        }
        else MsgBoxUI.Instance.ShowAttention("Select room from list");
    }
    // подключение к комнате успешно
    public void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient) { LobbyRoomUI.instance.StartCreateRoom(); }
        else LobbyRoomUI.instance.SetRoomInfo();
        _serverStatsUI.SetNewStatus("in room!", Color.white);
    }
    public IEnumerator ConnectToServerWithDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        {
            CheckPlayerName();
        }
    }
}
