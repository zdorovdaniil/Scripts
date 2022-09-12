using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
public class PlayerSetup : Photon.MonoBehaviour
{
    // компоненты или объекты которые необходмо отключить

    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject[] gameobjectToDisable;
    [SerializeField] private TextMeshPro _nickNameText; // Игровое имя текущего игрока
    [SerializeField] private TextMeshPro _hpText;
    private PlayerStats _playerStats;
    private GameManager _gameManager; public GameManager GetGameManager() { return _gameManager; }// объект игрока для противника
    [SerializeField] private PlayerType _playerType;
    private Quaternion startRotationCamPlayer;
    private Quaternion startRotationCamMiniMap;
    [SerializeField] private Camera _camPlayer;
    [SerializeField] private Camera _camMiniMap;
    private void Awake() { DisableGameObjects(); }
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerStats = GetComponent<PlayerStats>();
        if (PhotonNetwork.isMasterClient) { _gameManager.AddPlayer(_playerStats); }
        if (PhotonNetwork.isNonMasterClientInRoom) StartCoroutine(SetSecondPlayerWithDelay());
        if (PhotonNetwork.offlineMode) { _gameManager.AddPlayer(_playerStats); }
        if (photonView.isMine)
        {
            _playerType = PlayerType.Host;
            _nickNameText.color = Color.green;
            startRotationCamPlayer = _camPlayer.transform.rotation;
            startRotationCamMiniMap = _camMiniMap.transform.rotation;
        }
        // если мультиплеер
        if (PhotonNetwork.offlineMode != true)
        {
            _playerType = PlayerType.Guest;
            _nickNameText.SetText(photonView.owner.NickName);
            _playerStats.SetNickName(photonView.owner.NickName);
        }
        else
        {
            _nickNameText.gameObject.SetActive(false);
            _hpText.gameObject.SetActive(false);
        }
        if (PhotonNetwork.isMasterClient) StartCoroutine(SendClothIdWithDelay());
        if (_playerType == PlayerType.Host) { };
    }
    private IEnumerator SendClothIdWithDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        {
            GetComponent<Inventory>().UpdateClothVisible();
        }
        yield return new WaitForSecondsRealtime(4f);
        {
            GetComponent<Inventory>().UpdateClothVisible();
        }
    }
    private IEnumerator SetSecondPlayerWithDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        {
            _gameManager.AddPlayer(_playerStats);
        }
    }
    private float timer = 0;
    // Update Работает у всех игроков на которых висит PlayerSetup
    private void Update()
    {
        _camPlayer.transform.rotation = startRotationCamPlayer;
        _camMiniMap.transform.rotation = startRotationCamMiniMap;
        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            timer = 0;
            _hpText.text = _playerStats.curHP.ToString() + " / " + _playerStats.stats.HP.ToString();
        }
    }
    // отключение игровых объектов для сетевой игры
    private void DisableGameObjects()
    {
        // если не мастер клиент
        if ((!photonView.isMine) && (PhotonNetwork.isMasterClient || PhotonNetwork.isNonMasterClientInRoom))
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            for (int i = 0; i < gameobjectToDisable.Length; i++)
            {
                gameobjectToDisable[i].SetActive(false);
            }
        }
    }
}
public enum PlayerType
{
    None,
    Host,
    Guest
}
