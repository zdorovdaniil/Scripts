using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JerkButton : MonoBehaviour
{
    public static JerkButton Instance; private void Awake() { Instance = this; }
    [SerializeField] private GameObject _buttonObject;
    [SerializeField] private TMP_Text _timer;

    public void Update()
    {
        _buttonObject.SetActive(true);

    }


}
