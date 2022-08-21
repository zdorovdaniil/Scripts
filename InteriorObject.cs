using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> _variableGameObjects = new List<GameObject>() {this.gameObject};
    public int GetCountVariants => _variableGameObjects;

    public void SetVariant(int num)
    {
        ProcessCommand.SwithStatusInCollection(false,_variableGameObjects);
        numVariantsInterior[i].SetActive(true);
    }
}


