using UnityEngine;

/// <summary>
/// Скрипт отвечает за вывод из кнопки Improve в ImproveListUI 
/// </summary>
public class OpenImproveButton : MonoBehaviour
{
    [SerializeField] private Improve[] _improves;

    public void Click()
    {
        ImproveListUI.Instance.OpenImproveList(_improves);
    }
}
