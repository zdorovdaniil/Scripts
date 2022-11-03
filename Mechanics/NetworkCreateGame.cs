using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// скрипт назнает ник игрока и устанавливает подключение к комнате и лобби
public class NetworkCreateGame : Photon.MonoBehaviour
{
    public static NetworkCreateGame Instance; private void Awake() { Instance = this; }
    public Text LogText;
    public InputField NickNameField;
    [SerializeField] private Transform _enterPlayerName;
    private string _nickName;
    [SerializeField] private bool _isConnectedMasterServer;
    [SerializeField] private bool _isConnectedToLobby;
    [SerializeField] private ServerStatusUI _serverStatsUI;

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
    public void OnChangedServer()
    {
        DisconnecteFromServer();
        StartCoroutine(ConnectToServerWithDelay());
    }
    //public void StopChekingConnectToServer(){ StopCoroutine(CheckConnectToServer());}

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
        PhotonNetwork.ConnectToRegion(Network.ConvertToRegionCode(Settings.RegionCode), PhotonNetwork.gameVersion);
        _enterPlayerName.gameObject.SetActive(false);
        _serverStatsUI.SetNewStatus("connecting . . .", Color.white);
        StopCoroutine(CheckConnectToServer());
        StartCoroutine(CheckConnectToServer());
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    private IEnumerator CheckConnectToServer()
    {
        Debug.Log("Try Connecting ...");
        bool isConnectFirsTime = false;
        yield return new WaitForSecondsRealtime(2f);
        {
            if (!_isConnectedMasterServer) isConnectFirsTime = true;
        }
        yield return new WaitForSecondsRealtime(4f);
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
        Debug.Log("Joing to lobby ...");
        _isConnectedMasterServer = true;
        PhotonNetwork.JoinLobby();
    }

    // подключение к лобби успешно
    public virtual void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby");
        LobbyRoomUI.instance.SetRoomsList();
        _isConnectedToLobby = true;
        _serverStatsUI.SetNewStatus("connected!", Color.green);
        StopCoroutine(CheckConnectToServer());
    }
    public void DisconnecteFromServer()
    {
        _isConnectedMasterServer = false;
        _isConnectedToLobby = false;
        if (PhotonNetwork.connected) PhotonNetwork.Disconnect();
        _serverStatsUI.SetNewStatus("disconnected!", Color.red);
        StopCoroutine(CheckConnectToServer());
    }
    public void ClickCreateOwnRoom()
    {
        LobbyRoomUI.instance.ClearField();
        if (_isConnectedMasterServer && _isConnectedToLobby)
        {
            string roomName = NickNameField.text.ToLower();
            PlayerPrefs.SetString("NickName", NickNameField.text);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 }, null);
        }
        else
        {
            SelectDungeonUI.Instance.SetSingleplayerRoom();
        }

    }
    public void ClickKickOutFromRoom()
    {
        photonView.RPC("DisconnectFromRoom", PhotonTargets.Others);
        SelectDungeonUI.Instance.ClearSecondNickName();
    }
    public void ClickDisconnectFromRoom()
    {
        if (!PhotonNetwork.connected)
        {
            SelectDungeonUI.Instance.OpenlostRoomsLobby();
        }
        else if (PhotonNetwork.isMasterClient)
        { photonView.RPC("DisconnectFromRoom", PhotonTargets.All); }
        else { LobbyRoomUI.instance.photonView.RPC("ClearSecondNickName", PhotonTargets.Others); DisconnectFromRoom(); }
    }
    [PunRPC]
    public void DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
        SelectDungeonUI.Instance.OpenlostRoomsLobby();
    }
    public void ClickCreateRoom()
    {
        Settings.Instance.LoadingSettings();
        if (_isConnectedMasterServer && _isConnectedToLobby)
        {
            if (PhotonNetwork.room != null)
            {
                LobbyRoomUI.instance.ClearField();
                if (PhotonNetwork.room.PlayerCount >= 2 || Settings.IsAlwayesNetwork)
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
        LobbyRoomUI.instance.ClearField();
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
        if (PhotonNetwork.isMasterClient) { SelectDungeonUI.Instance.StartCreateRoom(); }
        else SelectDungeonUI.Instance.SetRoomInfo();
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
