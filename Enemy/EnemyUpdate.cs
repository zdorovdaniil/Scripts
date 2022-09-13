using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyUpdate : MonoBehaviour
{
    private EnemyStats _enemyStats;

    [SerializeField] private EnemyUI _enemyUI;
    private float _time;
    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
    }
    public void FixedUpdate()
    {
        _time += Time.deltaTime;
        if (_time >= 0.25f)
        {
            _time = 0;
            UpdateBuffFields();
        }
    }
    private void UpdateBuffFields()
    {
        if (_enemyStats.GetStats.ActiveBuffes.Count > 0)
        {
            foreach (BuffStat buffStat in _enemyStats.GetStats.ActiveBuffes)
            {
                if (buffStat.DoingBuff())
                {
                    Debug.Log(buffStat.BuffClass.BuffId + " " + buffStat.Time + "-" + buffStat.BuffClass.Duration);
                }
                else
                {
                    _enemyStats.GetStats.ResetBuff(buffStat);
                    break;
                }
            }
            _enemyUI.UpdateBuffPanel(_enemyStats.GetStats.ActiveBuffes);
        }
    }
}
