using System.Collections.Generic;
using UnityEngine;

public class InteriorObject : MonoBehaviour
{
    [Header("Activate one of this objects")]
    [SerializeField] private List<GameObject> _variableGameObjects = new List<GameObject>() { };
    public int GetCountVariants => _variableGameObjects.Count;

    public void SetVariant(int num)
    {
        ProcessCommand.SwithStatusInCollection(false, _variableGameObjects);
        _variableGameObjects[num].SetActive(true);
    }
}


