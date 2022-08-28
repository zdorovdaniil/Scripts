using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DestroyedObject : MonoBehaviour
{
    [SerializeField] private float _objectHp;
    [SerializeField] private SoundMaterials _soundOnDestroy;
    [SerializeField] private List<Transform> _destroyObjects = new List<Transform>();
    [SerializeField] private UnityEvent _onDestroyObject;
    public Chunk Chunk;
    public int Id;

    public void TakeDamage(float value)
    {
        if (Chunk)
        {
            if (PhotonNetwork.offlineMode) Chunk.SendDamageDestroyObject(Id, value);
            else Chunk.photonView.RPC("SendDamageDestroyObject", PhotonTargets.Others, (int)Id, (float)value);
        }
        else
        {
            _objectHp = Mathf.FloorToInt(_objectHp - value);
        }
        UpdateHP();
    }
    private void UpdateHP()
    {
        if (_objectHp <= 0)
        {
            DestroyObject();
        }
    }
    private void DestroyObject()
    {
        _onDestroyObject.Invoke();

        foreach (Transform trf in _destroyObjects) { if (trf) Destroy(trf.gameObject); }
        Destroy(this.gameObject);
    }
}

public enum SoundMaterials
{
    Wood, Stone, Metal,
}
