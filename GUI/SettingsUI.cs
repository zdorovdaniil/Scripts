using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _sliderSound;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private TMP_Dropdown _dropdownQuality;
    [SerializeField] private TMP_Dropdown _regionDropdown;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    private GameManager _gameManager;
    [SerializeField] private bool _isGoingGame; // Происходит ли сейас активный процесс игры в подземелье
    [SerializeField] private TMP_Text _dungeonLevelText;
    [SerializeField] private Transform _exitFromDungeonButton;
    [SerializeField] private Transform _changeLevelDungeonPanel;
    [SerializeField] private Toggle _isAlwayesNetworkToggle;

    public void ActivateUI(bool status)
    {
        _gameManager = FindObjectOfType<GameManager>();
        Settings.Instance.LoadingSettings();
        this.gameObject.SetActive(status);
        if (status)
        {
            SetUI();
            Settings.Instance.UpdateMixer();
            if (_isGoingGame)
            {
                ActivateGoingGameObjs();
                GlobalSounds.Instance.SOpenWindow();
            }
            else
            {
                ActivateMainMenuObjs();
                GlobalSounds.Instance.SCloseWindow();
            }
        }
        else { return; }
    }
    private void SetUI()
    {
        if (_sliderSound) _sliderSound.value = Settings.SoundsValue;
        if (_sliderMusic) _sliderMusic.value = Settings.MusicValue;
        if (_dropdownQuality) _dropdownQuality.value = Settings.QualityLevel;
        if (_regionDropdown) _regionDropdown.value = Settings.RegionCode;
        if (_dungeonLevelText) _dungeonLevelText.text = ProcessCommand.GetDungeonLevel.ToString();
        if (_isAlwayesNetworkToggle) _isAlwayesNetworkToggle.isOn = Settings.IsAlwayesNetwork;
        if (_languageDropdown) _languageDropdown.value = Settings.CurLanguage;
    }
    private void SaveSettings() { Settings.SaveSettings(); }
    public void ChangeLanguage(int dropDownValue)
    {
        Settings.CurLanguage = dropDownValue;
        SaveSettings();
        Settings.Instance.SendUpdateLanguage();
        Debug.Log(Settings.CurLanguage);
    }
    public void ChangeServer(int dropDownValue)
    {
        Settings.RegionCode = dropDownValue;
        SaveSettings();
        Debug.Log(Settings.RegionCode);
        NetworkCreateGame.Instance.OnChangedServer();
    }
    public void ChangeAlwayesNetwork(bool status)
    {
        Settings.IsAlwayesNetwork = status;
        SaveSettings();
        Debug.Log(Settings.IsAlwayesNetwork);
    }
    public void ChangeSoundsValue(float value)
    {
        Settings.SoundsValue = value;
        Settings.Instance.UpdateMixer();
        SaveSettings();
        Debug.Log(Settings.SoundsValue);

    }
    public void ChangeMusicValue(float value)
    {
        Settings.MusicValue = value;
        Settings.Instance.UpdateMixer();
        SaveSettings();
    }
    public void SetQuality(int qualityIndex)
    {
        Settings.QualityLevel = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings();
    }
    public void ClickDungeonLevelDown()
    {
        MsgBoxUI.Instance.Show(this.gameObject, "down dungeon level", "select button", "dungeonLevelDown");
    }
    private void DungeonLevelDown()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        if (ProcessCommand.GetDungeonLevel <= 1)
            MsgBoxUI.Instance.ShowInfo("attention", "dungeon level is already minimum");
        else
        {
            ProcessCommand.SetDungeonLevel(ProcessCommand.GetDungeonLevel - 1);
        }
        ActivateUI(true);
    }
    public void GetReport(string reportStatus)
    {
        if (reportStatus == "dungeonLeave")
        { CheckLeaveDengeon(); }
        else if (reportStatus == "dungeonLevelDown")
        { DungeonLevelDown(); }
    }
    public void ClickLeaveDungeon()
    {
        MsgBoxUI.Instance.Show(this.gameObject, "Exit from dungeon", "select button", "dungeonLeave");
    }
    private void CheckLeaveDengeon()
    {
        SceneLoadingUI.Instance.OpenLoadingUI("exit from dungeon", true);

        // если выходит владелец подземелья, то выкидываюся все игроки
        if (PhotonNetwork.offlineMode == true)
        { PhotonNetwork.offlineMode = false; SceneManager.LoadScene("Menu"); }
        else if (PhotonNetwork.isMasterClient)
        {
            if (_gameManager != null) { GameManager.Instance.SendAllLeaveDungeon(); }
            else GameManager.Instance.LeaveDungeon();
        }
        else { GameManager.Instance.LeaveDungeon(); }
    }
    private void ActivateGoingGameObjs()
    {
        _exitFromDungeonButton.gameObject.SetActive(true);
        _changeLevelDungeonPanel.gameObject.SetActive(false);
    }
    private void ActivateMainMenuObjs()
    {
        _exitFromDungeonButton.gameObject.SetActive(false);
        _changeLevelDungeonPanel.gameObject.SetActive(true);
    }
}
