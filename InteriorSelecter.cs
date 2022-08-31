using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSelecter : Photon.MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private int _spawnChanceObjects;
    [SerializeField] private List<InteriorObject> _objects = new List<InteriorObject>();
    [SerializeField] private List<DestroyedObject> _destroyedObject = new List<DestroyedObject>();

    private void Start()
    {
        SearchDestroyObject();
        if (PhotonNetwork.isMasterClient)
        {
            bool[] data = GetMassBoolObjects(_spawnChanceObjects, _objects);
            int[] variantData = GesMassVariantObjects(data, _objects);
            if (!PhotonNetwork.offlineMode)
            {
                photonView.RPC("SetStatusObjects", PhotonTargets.All, (bool[])data, (int[])variantData);
            }
            else { SetStatusObjects(data, variantData); }
        }
    }
    private void SearchDestroyObject()
    {
        DestroyedObject[] children = this.GetComponentsInChildren<DestroyedObject>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetInterior(this, i);
            _destroyedObject.Add(children[i]);
        }
    }
    [PunRPC]
    public void SendDamageDestroyObject(int id, float value)
    {
        _destroyedObject[id].TakeDamage(value);
    }
    private int[] GesMassVariantObjects(bool[] data, List<InteriorObject> objects)
    {
        int[] numVariantsInterior = new int[objects.Count];
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i])
            {
                int countVariants = objects[i].GetCountVariants;
                int rdm = Random.Range(0, countVariants);
                numVariantsInterior[i] = rdm;
            }
            else
            {
                numVariantsInterior[i] = 0;
            }
        }
        return numVariantsInterior;
    }
    private bool[] GetMassBoolObjects(int chance, List<InteriorObject> objects)
    {
        bool[] data = new bool[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            int randomValue = Random.Range(0, 100);
            if (randomValue < chance)
            {
                data[i] = true;
            }
            else
            {
                data[i] = false;
            }
        }
        return data;
    }

    [PunRPC]
    public void SetStatusObjects(bool[] data, int[] variantData)
    {
        for (int i = 0; i < data.Length; i++)
        {
            _objects[i].gameObject.SetActive(data[i]);
            _objects[i].SetVariant(variantData[i]);
        }
    }
}
