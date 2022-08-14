using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DestroyWithDelay : Photon.MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private bool _tickWithStart = true;
    private void Start()
    {
        if (_tickWithStart) StartCoroutine(DestroyThisWithDelay(_timeToDestroy));
    }
    public void StartTick()
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
