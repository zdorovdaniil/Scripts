using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSelecter : Photon.MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] private int _spawnChanceObjects1;
    [SerializeField] private List<Transform> _objects1;

    [Range(0,100)]
    [SerializeField] private int _spawnChanceObjects2;
    [SerializeField] private List<Transform> _objects2;

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {

        }
        
    }
    private bool[] GetMassBoolObjects(int chance, List<Transform> objects)
    {
        bool[] data = new bool[];
        for(int i = 0; i < objects.Count;i++)
        {
            int randomValue = Random.Range (0,100);
            if (randomValue < chance)
            {
                data[i] = true;                
            }
            else data[i] = false;
        }
        return data;
    }

    [PunRPC] public void SetStatusObjects(bool[] data)
    {
        
    }
}
