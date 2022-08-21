using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSelecter : Photon.MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] private int _spawnChanceObjects;
    [SerializeField] private List<InterierObject> _objects = new List<InteriorObject>();

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool[] data = GetMassBoolObjects(_spawnChanceObjects,_objects);
            int[] variantData = GesMassVariantObjects(data);
            if (!PhotonNetwork.offlineMode)
            {
                photonView.RPC("SetStatusObjects", PhotonTargets.All, (bool[])data,(int[])variantData);
            }
            else {SetStatusObjects(data,variantData);}
        }
    }
    private int[] GesMassVariantObjects(bool[] data)
    {
        int[] numVariantsInterior = new int[];
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i])
            {
                int countVariants = objects[i].GetCountVariants;
                int rdm = Random.Range(0,countVariants);
                numVariantsInterior[i] = rdm;
            }
            else{
                numVariantsInterior[i] = 0;
            }
        }
        return numVariantsInterior;
    }
    private bool[] GetMassBoolObjects(int chance, List<InterierObject> objects)
    {
        bool[] data = new bool[];
        for(int i = 0; i < objects.Count;i++)
        {
            int randomValue = Random.Range (0,100);
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
    public void SetStatusObjects(bool[] data,int[] variantData)
    {
        for(int i = 0;i<data.Length;i++)
        {
            _objects[i].gameObject.SetActive(data[i]);
            _objects[i].SetVariant(variantData[i]);
        }

    }
}