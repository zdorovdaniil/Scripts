using UnityEngine;

public class DestroyWithDelay : Photon.MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private GameObject _gameObject;
    private void Start()
    {
        StartCoroutine(DestroyThisWithDelay(_timeToDestroy));
    }
    private IEnumerator DestroyThisWithDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            if (!_gameObject)
                Destroy(this.gameObject);
            else Destroy(_gameObject);
        }
    }
}
