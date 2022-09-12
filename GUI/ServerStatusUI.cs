using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ServerStatusUI : MonoBehaviour
{
    public static ServerStatusUI Instance; void Awake() { Instance = this; }
    [SerializeField] private TMP_Text _statucText;
    public void SetNewStatus(string text, Color color)
    {
        _statucText.text = text;
        _statucText.color = color;
    }
}
