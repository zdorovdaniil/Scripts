using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Transform _posSpawn;

    public void SpawnWithDelay(float time = 0)
    {
        if (time > 0) StartCoroutine(Spawn(time));
        else GameManager.SpawnEnemyIn(_posSpawn, _enemy.PrefabEnemy);
    }
    private IEnumerator Spawn(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            GameManager.SpawnEnemyIn(_posSpawn, _enemy.PrefabEnemy);
        }
    }

}
