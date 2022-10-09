using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    #region Sengleton
    public static Settings Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _soundsValue;
    [SerializeField] private float _musicValue;
    [SerializeField] private int _qualityLevel;
    [SerializeField] private bool _isAlwayesNetwork; public bool GetIsAlwayesNetwork => _isAlwayesNetwork;

    private void Start()
    {
        LoadingSettings();
        QualitySettings.SetQualityLevel(_qualityLevel);
    }
    public void SetDefaultSettings()
    {
        PlayerPrefs.SetFloat("musicValue", 1);
        PlayerPrefs.SetFloat("soundsValue", 1);
        PlayerPrefs.SetInt("qualityLevel", 2);
        PlayerPrefs.SetInt("alwayesNetwork", 0);
        LoadingSettings();
    }
    public void LoadingSettings()
    {
        _musicValue = PlayerPrefs.GetFloat("musicValue");
        _soundsValue = PlayerPrefs.GetFloat("soundsValue");
        _qualityLevel = PlayerPrefs.GetInt("qualityLevel");
        if (PlayerPrefs.GetInt("alwayesNetwork") == 1) _isAlwayesNetwork = true;
        else _isAlwayesNetwork = false;
        _audioMixer.SetFloat("Sounds", Mathf.Lerp(-80, 0, _soundsValue));
        _audioMixer.SetFloat("Music", Mathf.Lerp(-80, 0, _musicValue));
    }
}
