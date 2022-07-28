using UnityEngine;

public class AnimatorPrepare : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    private Sound _sound;
    private void Start()
    {
        _sound = _enemyController.gameObject.GetComponent<Sound>();
    }
    public void ActivateDamageZone()
    {
        _enemyController.SwitchDamageZone(true);
    }
    public void DisableDamageZone()
    {
        _enemyController.SwitchDamageZone(false);
    }
    public void ExeSound(SoundType type)
    {
        _sound.StartSound(type);
    }
}
