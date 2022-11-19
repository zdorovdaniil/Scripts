using UnityEngine;
using System.Collections;

public class DestroyWithDelay : Photon.MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private bool _tickWithStart = true;
    private void Start()
    {
        if (_tickWithStart) StartTick();
    }
    public void StartTick()
    {
        StartCoroutine(DestroyThisWithDelay(_timeToDestroy));
    }
    private IEnumerator DestroyThisWithDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            if (_gameObjects.Length > 0)
            {
                for (int i = 0; i < _gameObjects.Length; i++)
                {
                    if (_gameObjects[i]) Destroy(_gameObjects[i]);
                }
            }
            else Destroy(this.gameObject);
        }
    }
}
