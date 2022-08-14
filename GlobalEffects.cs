using UnityEngine;

public class GlobalEffects : MonoBehaviour
{
    public static GlobalEffects Instance;
    private void Awake() { Instance = this; }
    [SerializeField] private GameObject _middlePuff;
    [SerializeField] private GameObject _particleTrail;
    [SerializeField] private GameObject _teleportParticle;

    public void CreateParticle(Transform transform, EffectType effect, float yChangeCoord = 0)
    {
        switch (effect)
        {
            case EffectType.MiddlePuff: Create(_middlePuff); break;
            case EffectType.TeleportTrail: Create(_particleTrail); break;
            case EffectType.Teleport: Create(_teleportParticle); break;
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
            particle.transform.SetParent(null);
        }
    }
}
public enum EffectType
{
    MiddlePuff,
    TeleportTrail,
    Teleport,

}
