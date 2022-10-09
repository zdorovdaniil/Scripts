using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI instance;
    [SerializeField] private PlayerQuest _playerQuest;
    [Header("GUI Quests")]
    [SerializeField] private GameObject _buttonQuestPrefab;
    [SerializeField] private RectTransform _activeQuests;
    [SerializeField] private RectTransform _completeQuests;

    private void Start() { instance = this; }
    public void FillListsOfQuest(PlayerQuest plQuest)
    {
        ProcessCommand.ClearChildObj(_activeQuests);
        ProcessCommand.ClearChildObj(_completeQuests);

        int countCompleteQuest = 1;
        int countActiveQuest = 1;
        foreach (Quest quest in plQuest.GetActiveQuests)
        {
            if (!quest.isComplete)
            { Instantiate(_buttonQuestPrefab, _activeQuests).GetComponent<QuestButton>().SetUp(quest, _playerQuest); countActiveQuest += 1; }
            else
            {
                Instantiate(_buttonQuestPrefab, _completeQuests).GetComponent<QuestButton>().SetUp(quest, _playerQuest); countCompleteQuest += 1;
            }
        }
        _activeQuests.sizeDelta = new Vector2(0, countActiveQuest * 90f);
        _completeQuests.sizeDelta = new Vector2(0, countCompleteQuest * 90f);

    }
}
