using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalEffects : MonoBehaviour
{
    public static GlobalEffects Instance;
    private void Awake() { Instance = this; }
    [SerializeField] private GameObject _middlePuff;
    [SerializeField] private GameObject _particleTrail;
    [SerializeField] private GameObject _teleportParticle;
    [SerializeField] private GameObject _webShot;
    [SerializeField] private GameObject _swordSplash;
    [SerializeField] private GameObject _hitParticle;
    [SerializeField] private GameObject _swapHit;
    [SerializeField] private GameObject _bloodHit;
    [SerializeField] private GameObject _flyingSlash;
    private Transform _garbage;
    private void Start()
    {
        _garbage = GameManager.Instance.tempView.transform;

    }
    public GameObject CreateParticle(Transform transform, EffectType effect, float yChangeCoord = 0, int zRoration = 0, float delay = 0)
    {
        switch (effect)
        {
            case EffectType.MiddlePuff: return Create(_middlePuff);
            case EffectType.TeleportTrail: return Create(_particleTrail);
            case EffectType.Teleport: return Create(_teleportParticle);
            case EffectType.WebShot: return Create(_webShot);
            case EffectType.SwordSplash: return Rotate(_swordSplash);
            case EffectType.Hit: return Place(_hitParticle);
            case EffectType.SwapHit: return Create(_swapHit);
            case EffectType.BloodHit: return Create(_bloodHit);
            case EffectType.FlyingSlash: return Create(_flyingSlash);
        }
        return null;
        GameObject Create(GameObject objParticle)
        {
            GameObject particle = Instantiate(objParticle, transform);
            if (yChangeCoord != 0)
            {
                Vector3 newCoord = transform.position;
                newCoord.y = newCoord.y + yChangeCoord;
                particle.transform.position = newCoord;
            }
            particle.transform.SetParent(_garbage);
            return particle;
        }
        GameObject Place(GameObject objParticle)
        {
            GameObject particle = Instantiate(objParticle, transform);
            if (yChangeCoord != 0)
            {
                Vector3 newCoord = transform.position;
                newCoord.y = newCoord.y + yChangeCoord;
                particle.transform.position = newCoord;
            }
            return particle;
        }
        GameObject Rotate(GameObject objParticle)
        {
            GameObject particle = Instantiate(objParticle, transform);
            if (zRoration != 0)
            {
                particle.transform.Rotate(0, 0, zRoration);
            }
            particle.transform.SetParent(_garbage);
            return particle;
        }

    }

}
public enum EffectType
{
    None,
    MiddlePuff,
    TeleportTrail,
    Teleport,
    WebShot,
    SwordSplash,
    Hit,
    SwapHit,
    BloodHit,
    FlyingSlash,


}
