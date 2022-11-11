using UnityEngine;

/// <summary>
/// Скрипт отвечает за формирования списка ImproveUI из входящего списка Improve
/// </summary>
public class ImproveListUI : MonoBehaviour
{
    public static ImproveListUI Instance; private void Awake() { Instance = this; }
    [SerializeField] private Transform _improveListPanel;
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _prefabImproveUI;

    private void Start()
    {
        _improveListPanel.gameObject.SetActive(false);
    }
    public void OpenImproveList(Improve[] improves)
    {
        _improveListPanel.gameObject.SetActive(true);
        ProcessCommand.ClearChildObj(_container);
        for (int i = 0; i < improves.Length; i++)
        {
            Instantiate(_prefabImproveUI, _container).GetComponent<ImproveUI>().SetImprove(improves[i]);
        }
    }



    public void CloseImproveList()
    {
        ProcessCommand.ClearChildObj(_container);
        _improveListPanel.gameObject.SetActive(false);
    }
}
