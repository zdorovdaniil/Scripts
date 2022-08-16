using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    [SerializeField] private EffectType _effectType;
    [SerializeField] private Transform _transform;
    [SerializeField] private int _chanceSpawn = 100;
    [SerializeField] private float _yChangeCoord;
    [SerializeField] private int _zRoration;

    public void SpawnEffect()
    {
        if (_chanceSpawn == 100) GlobalEffects.Instance.CreateParticle(_transform, _effectType, _yChangeCoord, _zRoration);
        else
        {
            int random = Random.Range(0, 100);
            if (random < _chanceSpawn) GlobalEffects.Instance.CreateParticle(_transform, _effectType, _yChangeCoord, _zRoration);
            else { return; }
        }
    }
}
