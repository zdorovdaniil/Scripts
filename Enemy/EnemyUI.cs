using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using stats;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private bool _isFreezeRotation = true;
    public TextMeshPro lvlUI;
    public TextMeshPro NameEnemyUI;
    public TextMeshPro HealthEnemyUI;
    public Transform EnemyGUI;
    private Transform _player;
    private void Awake()
    {
        _player = transform.parent;
        transform.parent = null;
    }
    public void LateUpdate()
    {
        transform.position = _player.transform.position;

        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
    public void UpdateUI(EnemyStats enemyStats, Stats stats)
    {
        lvlUI.SetText("lvl - " + stats.Level.ToString());
        NameEnemyUI.SetText(enemyStats.enemyTupe.Name);
        HealthEnemyUI.SetText(enemyStats.curHP.ToString() + " / " + stats.HP);
    }
}
