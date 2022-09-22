using UnityEngine;
using TMPro;
using stats;
using System.Collections.Generic;
public class EnemyUI : MonoBehaviour
{
    public TextMeshPro lvlUI;
    public TextMeshPro NameEnemyUI;
    public TextMeshPro HealthEnemyUI;
    [SerializeField] private GameObject _buffField;
    [SerializeField] private Transform _containBuffFields;
    private Transform _enemyPos;

    private void Awake()
    {
        _enemyPos = transform.parent;
        transform.parent = null;
    }
    private void LateUpdate()
    {
        transform.position = _enemyPos.transform.position;
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
    public void UpdateUI(EnemyStats enemyStats, Stats stats)
    {
        lvlUI.SetText("lvl - " + stats.Level.ToString());
        NameEnemyUI.SetText(enemyStats.enemyTupe.Name);
        HealthEnemyUI.SetText((Mathf.Floor(enemyStats.curHP * 100.00f) * 0.01f).ToString() + " / " + stats.HP);
    }
    public void UpdateBuffPanel(List<BuffStat> buffStats)
    {
        ProcessCommand.ClearChildObj(_containBuffFields);
        foreach (BuffStat buffStat in buffStats)
        {
            Instantiate(_buffField, _containBuffFields).GetComponent<BuffField>().SetFieldsBuff(buffStat);
        }
    }
}
