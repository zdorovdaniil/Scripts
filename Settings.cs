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
