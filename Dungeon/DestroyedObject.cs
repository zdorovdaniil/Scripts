using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class DestroyedObject : MonoBehaviour
{
    [SerializeField] private float _objectHp;
    [SerializeField] private SoundMaterials _soundOnHit;
    [SerializeField] private List<Transform> _destroyObjects = new List<Transform>();
    [SerializeField] private UnityEvent _onDestroyObject;
    [SerializeField] private InteriorSelecter _interiorSelecter;
    [SerializeField] private int _id;
    private bool _isTakeDamage = true;

    public void MinusObjectHP(float value)
    {
        Debug.Log("Hit Destroy Object On = " + value + " Id = " + _id);
        LogUI.Instance.Loger("Hit Destroy Object On = " + value + " Id = " + _id);
        _objectHp -= value;
        UpdateHP();
    }
    public void TakeDamage(float value)
    {
        if (_isTakeDamage)
        {
            _isTakeDamage = false;
            StartCoroutine(ProcessCommand.DoActionDelay(ResetDamageGet, 0.5f));
            //StartCoroutine(ResetDamageGet());
            if (_interiorSelecter && !PhotonNetwork.offlineMode)
            {
                _interiorSelecter.photonView.RPC("SendDamageDestroyObject", PhotonTargets.All, (int)_id, (float)value);
            }
            else
            {
                MinusObjectHP(value);
            }
        }
        GlobalSounds.Instance.CreateSoundMaterial(this.transform, _soundOnHit);
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
        GlobalSounds.Instance.CreateDestroySound(this.transform, _soundOnHit);
        foreach (Transform trf in _destroyObjects) { if (trf) Destroy(trf.gameObject); }
        Destroy(this.gameObject);
    }
    private void ResetDamageGet()
    {
        //yield return new WaitForSecondsRealtime(0.6f);
        {
            _isTakeDamage = true;
        }
    }
}

