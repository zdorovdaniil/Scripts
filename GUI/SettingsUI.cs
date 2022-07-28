using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _sliderSound;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private TMP_Dropdown _dropdownQuality;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private float _soundsValue;
    [SerializeField] private float _musicValue;
    [SerializeField] private int _dungeonLevel;
    [SerializeField] private int _qualityLevel;
    [SerializeField] private bool _isAlwayesNetwork;
    private GameManager _gameManager;
    [SerializeField] private bool _isGoingGame; // Происходит ли сейас активный процесс игры в подземелье
    [SerializeField] private TMP_Text _dungeonLevelText;
    [SerializeField] private Transform _exitFromDungeonButton;
    [SerializeField] private Transform _changeLevelDungeonPanel;
    [SerializeField] private Toggle _isAlwayesNetworkToggle;

    public void ActivateUI(bool status)
    {
        _gameManager = FindObjectOfType<GameManager>();
        LoadSetting();
        this.gameObject.SetActive(status);
        if (status)
        {
            if (_isGoingGame)
            {
                ActivateGoingGameObjs();
            }
            else
            {
                ActivateMainMenuObjs();
            }
        }
        else{ return;}
    }
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicValue",_musicValue);
        PlayerPrefs.SetFloat("soundsValue",_soundsValue);
        PlayerPrefs.SetInt("qualityLevel",_qualityLevel);
        if (_isAlwayesNetwork) PlayerPrefs.SetInt("alwayesNetwork", 1);
        else PlayerPrefs.SetInt("alwayesNetwork", 0);
    }
    public void ChangeAlwayesNetwork(bool status)
    {
        _isAlwayesNetwork = status;
        SaveSettings();
    }
    public void ChangeSoundsValue(float value)
    {
        _soundsValue =  value;
        _audioMixer.SetFloat("Sounds", Mathf.Lerp(-80,0,value));
        SaveSettings();

    }
    public void ChangeMusicValue(float value)
    {
        _musicValue =  value;
        _audioMixer.SetFloat("Music", Mathf.Lerp(-80,0,value));
        SaveSettings();
    }
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings();
    }
    public void LoadSetting()
    {
        int id = PlayerPrefs.GetInt("activeSlot"); 
        _dungeonLevel = PlayerPrefs.GetInt(id + "_slot_dungeonLevel");
        _musicValue = PlayerPrefs.GetFloat("musicValue");
        _soundsValue = PlayerPrefs.GetFloat("soundsValue");
        _qualityLevel = PlayerPrefs.GetInt("qualityLevel");
        if (PlayerPrefs.GetInt("alwayesNetwork") == 1) _isAlwayesNetwork = true;
        else _isAlwayesNetwork = false;
        _sliderMusic.value = _musicValue;
        _sliderSound.value = _soundsValue;
        _dropdownQuality.value = _qualityLevel;
        if (_isAlwayesNetworkToggle != null) _isAlwayesNetworkToggle.isOn = _isAlwayesNetwork;
    }
    public void ClickDungeonLevelDown()
    {
        MsgBoxUI.Instance.Show(this.gameObject, "down dungeon level","select button","dungeonLevelDown");
        
    }
    private void DungeonLevelDown()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        if (_dungeonLevel <= 1)
            MsgBoxUI.Instance.ShowInfo("attention", "dungeon level is already minimum");
        else
        {
            _dungeonLevel -= 1;
            PlayerPrefs.SetInt(id + "_slot_dungeonLevel", _dungeonLevel);
        }
        ActivateUI(true);
    }
    public void GetReport(string reportStatus)
    {
        if (reportStatus == "dungeonLeave")
        {CheckLeaveDengeon();}
        else if (reportStatus == "dungeonLevelDown")
        {DungeonLevelDown();}
    }
    public void ClickLeaveDungeon()
    {
        MsgBoxUI.Instance.Show(this.gameObject, "Exit from dungeon","select button","dungeonLeave");
        // если выходит владелец подземелья, то выкидываюся все игроки

    }
    private void CheckLeaveDengeon()
    {
        SceneLoadingUI.Instance.OpenLoadingUI("exit from dungeon",true);

        // если выходит владелец подземелья, то выкидываюся все игроки
        if (PhotonNetwork.offlineMode == true)
        { PhotonNetwork.offlineMode = false; SceneManager.LoadScene("Menu"); }
        else if (PhotonNetwork.isMasterClient)
        {
            if (_gameManager != null) {GameManager.Instance.SendAllLeaveDungeon();}
            else GameManager.Instance.LeaveDungeon();
        }
        else { GameManager.Instance.LeaveDungeon();}
    }
    private void ActivateGoingGameObjs()
    {
        _exitFromDungeonButton.gameObject.SetActive(true);
        _changeLevelDungeonPanel.gameObject.SetActive( false);
    }
    private void ActivateMainMenuObjs()
    {
        _dungeonLevelText.text = _dungeonLevel.ToString();
        _exitFromDungeonButton.gameObject.SetActive(false);
        _changeLevelDungeonPanel.gameObject.SetActive(true);
    }
    public void SetPlayerStats(PlayerStats playerStats)
    {_playerStats = playerStats;}
}
