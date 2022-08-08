using UnityEngine;

// этот скрипт удерживает поворот на значении данном при старте
public class RotationFreez : MonoBehaviour
{
    Quaternion startRotation;


    private void Start()
    {
        startRotation = this.transform.rotation;
    }
    private void FixedUpdate()
    {
        this.transform.rotation =
        this.transform.rotation = startRotation;
    }
}

