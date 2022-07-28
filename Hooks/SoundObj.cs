using UnityEngine;

public class SoundObj : MonoBehaviour
{
    [SerializeField] private AudioSource[] _sounds;
    [SerializeField] private bool _runAtStart;
    private void Start()
    {
        if (_runAtStart) StartSound();
    }
    public void StartSound()
    {
        int random = Random.Range(0, _sounds.Length);
        if (_sounds[random] != null) _sounds[random].Play();
    }
}
