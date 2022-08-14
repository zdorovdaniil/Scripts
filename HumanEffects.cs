using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEffects : MonoBehaviour
{
    private BasePrefs _basePrefs;
    private List<GameObject> _buffEffects = new List<GameObject>();
    [SerializeField] private GameObject _weaponParticle;
    [SerializeField] private ParticleSystem _jercParticle;

    private void Start()
    {
        _basePrefs = BasePrefs.instance;
    }
    public void SpawnSwordEffects(int zRoration = 0)
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject obj = Instantiate(_weaponParticle, spawnPos, transform.rotation).gameObject;
        if (zRoration != 0)
        {
            obj.transform.Rotate(0, 0, zRoration);
        }
        StartCoroutine(DestroyParticle(obj, 0.6f));
    }
    public void PlayeJercEffect()
    {
        _jercParticle.Play();
    }
    private IEnumerator DestroyParticle(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            Destroy(obj);
        }
    }
    public void ActivateEffect(BuffClass buff)
    {
        if (buff.EffectOnUse)
        {
            GameObject obj = Instantiate(buff.EffectOnUse, transform.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            StartCoroutine(DestroyObjWithDelay(obj, buff.EffectLifetime));
        }
        if (buff.EffectOnUsing)
        {
            GameObject obj = Instantiate(buff.EffectOnUsing, transform.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            StartCoroutine(DestroyObjWithDelay(obj, buff.Time));
        }
    }
    public void DiactivateEffects()
    {
        foreach (GameObject buffObj in _buffEffects)
        {
            Destroy(buffObj);
        }
    }
    private IEnumerator DestroyObjWithDelay(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            Destroy(obj);
        }
    }

}