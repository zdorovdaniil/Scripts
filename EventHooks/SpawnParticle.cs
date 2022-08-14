using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    [SerializeField] private EffectType _effectType;
    [SerializeField] private Transform _transform;

    public void SpawnEffect()
    {
        GlobalEffects.Instance.CreateParticle(_transform, _effectType);
    }
}
