using UnityEngine;
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource[] _death;
    [SerializeField] private AudioSource[] _attackAir;
    [SerializeField] private AudioSource[] _hit;
    [SerializeField] private AudioSource[] _step;
    [SerializeField] private AudioSource[] _idle;
    [SerializeField] private AudioSource[] _hearth;
    [SerializeField] private AudioSource[] _takeDrop;

    public void StartSound(SoundType type)
    {
        switch (type)
        {
            case SoundType.Death: if (RandomOfSource(_death) != null) RandomOfSource(_death).Play(); break;
            case SoundType.AttackAir: if (RandomOfSource(_attackAir) != null) RandomOfSource(_attackAir).Play(); break;
            case SoundType.Hit: if (RandomOfSource(_hit) != null) RandomOfSource(_hit).Play(); break;

            case SoundType.Step: if (RandomOfSource(_step) != null) RandomOfSource(_step).Play(); break;
            case SoundType.Idle: if (RandomOfSource(_idle) != null) RandomOfSource(_idle).Play(); break;
            case SoundType.Hearth: if (RandomOfSource(_hearth) != null) RandomOfSource(_hearth).Play(); break;
            case SoundType.TakeDrop: if (RandomOfSource(_takeDrop) != null) RandomOfSource(_takeDrop).Play(); break;
        }
    }
    private AudioSource RandomOfSource(AudioSource[] source)
    {
        return source[Random.Range(0, source.Length)];
    }
}
public enum SoundType
{
    Death,
    AttackAir,
    Hit,
    Step,
    Idle,
    Hearth,
    TakeDrop,
}

