using System.Collections.Generic;
using UnityEngine;

public class DungeonQuests : MonoBehaviour
{
    public static DungeonQuests Instance; private void Awake() { Instance = this; }
    [SerializeField] private int _countDunQuest;
    [SerializeField] private List<Quest> _allQuests = new List<Quest>();

    private void Start() { ResetQuestsValue(); }

    private void ResetQuestsValue()
    { foreach (Quest quest in _allQuests) { quest.Reset(); } }
    public List<Quest> GenerateActiveQuests()
    {
        _countDunQuest = DungeonStats.Instance.Rule.AddQuestFromLevel(DungeonStats.Instance.GetDungeonLevel);
        List<Quest> listActiveQuest = new List<Quest>();
        List<Quest> listOnlyDunList = new List<Quest>();
        // заполнение списка основных квестов и квестов для подземелья
        foreach (Quest quest in _allQuests)
        {
            if (quest.isMainQuest) listActiveQuest.Add(quest);
            else if (quest.isInOnlyDungeon) listOnlyDunList.Add(quest);
        }
        // выбор заданного количества квестов для подземелья
        for (int i = 0; i < _countDunQuest; i++)
        {
            int randomValue = Random.Range(0, listOnlyDunList.Count);
            if (listOnlyDunList[randomValue] != null)
            {
                listActiveQuest.Add(listOnlyDunList[randomValue]);
                listOnlyDunList.RemoveAt(randomValue);
            }
        }
        return listActiveQuest;
    }

    // загрузка результатов ТОЛЬКО основных квестов()
    public void LoadQuestsValue()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        foreach (Quest quest in _allQuests)
        {
            if (quest.isMainQuest)
            {
                float processValue = PlayerPrefs.GetFloat(id + "_questProcess_" + quest.Id);
                quest.progressStatus = processValue;
                int isComplete = PlayerPrefs.GetInt(id + "_questCompleted_" + quest.Id);
                if (isComplete == 1) { quest.isComplete = true; quest.IsCompletePermanent = true; }
                else { quest.isComplete = false; quest.IsCompletePermanent = false; }
            }
        }
    }

    public void SaveQuestsValue()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        foreach (Quest quest in _allQuests)
        {
            PlayerPrefs.SetFloat(id + "_questProcess_" + +quest.Id, quest.progressStatus);
            int temp = 0;
            if (quest.isComplete == true)
                temp = 1;
            PlayerPrefs.SetInt(id + "_questCompleted_" + +quest.Id, temp);
        }
    }

}
