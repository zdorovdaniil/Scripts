using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DestroyedObject : MonoBehaviour
{
    [SerializeField] private float _objectHp;
    [SerializeField] private SoundMaterials _soundOnDestroy;
    [SerializeField] private List<Transform> _destroyObjects = new List<Transform>();
    [SerializeField] private UnityEvent _onDestroyObject;
    [SerializeField] private InteriorSelecter _interiorSelecter;
    [SerializeField] private int _id;

    public void TakeDamage(float value)
    {
        if (_interiorSelecter && !PhotonNetwork.offlineMode)
        {
            _interiorSelecter.photonView.RPC("SendDamageDestroyObject", PhotonTargets.All, (int)_id, (float)value);
        }
        else
        {
            _objectHp = Mathf.FloorToInt(_objectHp - value);
        }
        Debug.Log(_id + " _value_" + value);
        UpdateHP();
    }
    public void SetInterior(InteriorSelecter interiorSelecter, int id)
    {
        _interiorSelecter = interiorSelecter;
        _id = id;
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
