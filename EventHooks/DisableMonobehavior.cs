using UnityEngine;
using System.Collections;

public class DisableMonobehavior : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private MonoBehaviour[] components;
    public void Activate()
    {
        StartCoroutine(Delay());
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(_delay);
        {
            for (int i = 0; i < components.Length; i++)
            {
                Destroy(components[i]);
            }
        }
    }
}
