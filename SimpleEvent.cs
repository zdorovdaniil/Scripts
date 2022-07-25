using UnityEngine;
using UnityEngine.Events;

public class SimpleEvent : MonoBehaviour
{
    public UnityEvent evention;
    public void ExeEvent()
    {
        evention.Invoke();
    }
}

