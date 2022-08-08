using UnityEngine;
using System.Collections;

public class SoundObj : MonoBehaviour
{
    [SerializeField] private AudioSource[] _sounds;
    [SerializeField] private bool _runAtStart;
    [SerializeField] private float _runWithDelay;
    private void Start()
    {
        if (_runAtStart) StartSound();
    }
    public void StartSoundNumber(int num)
    {
        if (_sounds[num] != null) StartCoroutine(PlayerWithDelay(_sounds[num]));
        else Debug.Log("Sound is null");
    }
    public void StartSound()
    {
        int random = Random.Range(0, _sounds.Length);
        if (_sounds[random] != null) StartCoroutine(PlayerWithDelay(_sounds[random]));
        else Debug.Log("Sound is null");
    }
    private IEnumerator PlayerWithDelay(AudioSource audio)
    {
        yield return new WaitForSecondsRealtime(_runWithDelay);
        {
            audio.Play();
        }
    }
}
