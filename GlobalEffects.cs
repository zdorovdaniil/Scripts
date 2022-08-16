using UnityEngine;

public class GlobalEffects : MonoBehaviour
{
    public static GlobalEffects Instance;
    private void Awake() { Instance = this; }
    [SerializeField] private GameObject _middlePuff;
    [SerializeField] private GameObject _particleTrail;
    [SerializeField] private GameObject _teleportParticle;
    [SerializeField] private GameObject _webShot;
    [SerializeField] private GameObject _swordSplash;
    private Transform _garbage;
    private void Start()
    {
        _garbage = GameManager.Instance.tempView.transform;

    }
    public void CreateParticle(Transform transform, EffectType effect, float yChangeCoord = 0, int zRoration = 0)
    {
        switch (effect)
        {
            case EffectType.MiddlePuff: Create(_middlePuff); break;
            case EffectType.TeleportTrail: Create(_particleTrail); break;
            case EffectType.Teleport: Create(_teleportParticle); break;
            case EffectType.WebShot: Create(_webShot); break;
            case EffectType.SwordSplash: Rotate(_swordSplash); break;
        }
        void Create(GameObject objParticle)
        {
            GameObject particle = Instantiate(objParticle, transform);
            if (yChangeCoord != 0)
            {
                Vector3 newCoord = transform.position;
                newCoord.y = newCoord.y + yChangeCoord;
                particle.transform.position = newCoord;
            }
            particle.transform.SetParent(_garbage);
        }
        void Rotate(GameObject objParticle)
        {
            GameObject particle = Instantiate(objParticle, transform);
            if (zRoration != 0)
            {
                particle.transform.Rotate(0, 0, zRoration);
            }
            particle.transform.SetParent(_garbage);
        }

    }

}
public enum EffectType
{
    MiddlePuff,
    TeleportTrail,
    Teleport,
    WebShot,
    SwordSplash,

}
