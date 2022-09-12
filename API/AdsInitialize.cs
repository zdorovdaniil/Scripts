using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitialize : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOSGameId;
    [SerializeField] bool _testMode = true;

    private string _gameId;
    private void Awake() { InitializeAds(); }
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        StartCoroutine(Network.CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
            else
            {
                Debug.Log("Internet Not Available");
            }
        }));

    }
    public void OnInitializationComplete()
    {
        Debug.Log("AdsInitialize Complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

}
