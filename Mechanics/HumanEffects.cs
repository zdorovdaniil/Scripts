using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEffects : MonoBehaviour
{
    private BasePrefs _basePrefs;
    private List<GameObject> _buffEffects = new List<GameObject>();
    [SerializeField] private ParticlePlaces _particlePlaeces;
    public void AddParticleToContainer(GameObject obj) => obj.transform.SetParent(_particlePlaeces.ParcicleContainer);
    [SerializeField] private GameObject _weaponParticle;
    [SerializeField] private ParticleSystem _jercParticle;
    [SerializeField] private GameObject _flyingSlash;

    private void Start()
    {
        _basePrefs = BasePrefs.instance;
    }
    public void SpawnFlyingSword()
    {
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            {
                DamageZone damageZone = GlobalEffects.Instance.CreateParticle(this.transform, EffectType.FlyingSlash, 2.5f).GetComponent<DamageZone>();
                damageZone.playerStats = GetComponent<PlayerStats>();
            }
        }

    }
    public void SpawnSwordEffects(int zRoration = 0)
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject obj = Instantiate(_weaponParticle, spawnPos, transform.rotation).gameObject;
        if (zRoration != 0)
        {
            obj.transform.Rotate(0, 0, zRoration);
        }
        StartCoroutine(DestroyObjWithDelay(obj, 0.6f));
    }
    public void PlayJercEffect()
    {
        _jercParticle.Play();
    }
    public void ActivateEffect(BuffClass buff)
    {
        GameObject obj = null;
        if (buff.EffectOnUse)
        {
            obj = Instantiate(buff.EffectOnUse, _particlePlaeces.BuffPlace.position, _particlePlaeces.BuffPlace.rotation);
            obj.transform.SetParent(_particlePlaeces.BuffPlace);
            StartCoroutine(DestroyObjWithDelay(obj, buff.EffectLifetime));
        }
        if (buff.EffectOnUsing)
        {
            obj = Instantiate(buff.EffectOnUsing, _particlePlaeces.BuffPlace.position, _particlePlaeces.BuffPlace.rotation);
            obj.transform.SetParent(_particlePlaeces.BuffPlace);
            StartCoroutine(DestroyObjWithDelay(obj, buff.Duration));
        }
        if (obj) _buffEffects.Add(obj);
    }
    public void DiactivateEffects()
    {
        foreach (GameObject buffObj in _buffEffects)
        {
            Destroy(buffObj);
        }
        // удаление частиц в контейнере
        ProcessCommand.ClearChildObj(_particlePlaeces.ParcicleContainer);
    }
    private IEnumerator DestroyObjWithDelay(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            Destroy(obj);
        }
    }

}