using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI instance;
    public Quest SelectedQuest;
    [SerializeField] private PlayerQuest _playerQuest;
    [Header("GUI Quests")]
    [SerializeField] private GameObject _buttonQuestPrefab;
    [SerializeField] private Transform _activeQuests;
    [SerializeField] private Transform _complete;

    private void Start() {instance = this;}
    public void FillListsOfQuest(PlayerQuest plQuest)
    {
        for (int i = 0; i < _activeQuests.childCount; i++)
        {
            Destroy(_activeQuests.GetChild(i).gameObject);
        }
        for (int i = 0; i < _complete.childCount; i++)
        {
            Destroy(_complete.GetChild(i).gameObject);
        }

        foreach (Quest quest in plQuest.GetActiveQuests)
        {
            if (quest.isComplete == false)
                Instantiate(_buttonQuestPrefab, _activeQuests).GetComponent<QuestButton>().SetUp(quest, _playerQuest);
            else Instantiate(_buttonQuestPrefab, _complete).GetComponent<QuestButton>().SetUp(quest, _playerQuest);

        }
        //Debug.Log("Листы квестов заполнены");
    }
}
