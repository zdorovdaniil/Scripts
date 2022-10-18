using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using System.Collections;

public class Settings : MonoBehaviour
{
    public static Settings Instance; private void Awake() { Instance = this; }
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private LocalizationSettings _localizationSettings;
    public static int RegionCode;
    public static float SoundsValue;
    public static float MusicValue;
    public static int QualityLevel;
    public static int CurLanguage;
    public static bool IsAlwayesNetwork;

    private void Start()
    {
        LoadingSettings();
        UpdateMixer();
        SendUpdateLanguage(true);
    }
    public void SetDefaultSettings()
    {
        PlayerPrefs.SetInt(ProcessCommand.CurActiveSlot + "_slot_dungeonLevel", 1);
        PlayerPrefs.SetInt("region", 0);
        PlayerPrefs.SetFloat("musicValue", 1);
        PlayerPrefs.SetFloat("soundsValue", 1);
        PlayerPrefs.SetInt("qualityLevel", 2);
        PlayerPrefs.SetInt("curLanguage", 0);
        PlayerPrefs.SetInt("alwayesNetwork", 0);
        LoadingSettings();
    }
    public void UpdateMixer()
    {
        _audioMixer.SetFloat("Sounds", Mathf.Lerp(-80, 0, SoundsValue));
        _audioMixer.SetFloat("Music", Mathf.Lerp(-80, 0, MusicValue));
    }
    public void SendUpdateLanguage(bool withDelay = false)
    {
        if (withDelay)
        { StartCoroutine(Dalay()); }
        else
        { UpdateLanguage(); }

    }
    private void UpdateLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[CurLanguage];
        TextBase.SetTextLanguage(BasePrefs.instance.GetTextLanguage(CurLanguage));
    }
    IEnumerator Dalay()
    { yield return new WaitForSecondsRealtime(1f); { UpdateLanguage(); } }

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("region", RegionCode);
        PlayerPrefs.SetFloat("musicValue", MusicValue);
        PlayerPrefs.SetFloat("soundsValue", SoundsValue);
        PlayerPrefs.SetInt("qualityLevel", QualityLevel);
        PlayerPrefs.SetInt("curLanguage", CurLanguage);
        PlayerPrefs.SetInt("alwayesNetwork", ProcessCommand.ConvertBoolToInt(IsAlwayesNetwork));
    }

    public void LoadingSettings()
    {
        RegionCode = PlayerPrefs.GetInt("region");
        MusicValue = PlayerPrefs.GetFloat("musicValue");
        SoundsValue = PlayerPrefs.GetFloat("soundsValue");
        QualityLevel = PlayerPrefs.GetInt("qualityLevel");
        CurLanguage = PlayerPrefs.GetInt("curLanguage");
        IsAlwayesNetwork = ProcessCommand.ConvertIntToBool(PlayerPrefs.GetInt("alwayesNetwork"));
        QualitySettings.SetQualityLevel(QualityLevel);
    }
}
