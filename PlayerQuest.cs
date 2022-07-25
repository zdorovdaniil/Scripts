using System.Collections.Generic;
using UnityEngine;
public class PlayerQuest : MonoBehaviour
{
    public static PlayerQuest instance;
    [Header("List quests")]
    [SerializeField] private List<Quest> _listActiveQuests = new List<Quest>();
    public List<Quest> GetActiveQuests => _listActiveQuests;
    [SerializeField] private List<Quest> _allQuests = new List<Quest>();
    public List<Quest> GetAllQuests => _allQuests;
    [Header("Quest count")]
    [SerializeField] private int _countDunQuest;
    private Inventory _inv;


    private void Start()
    {
        instance = this;
        _inv = GetComponent<Inventory>();
    }
    public void InstainceQuests()
    {
        ResetQuestsValue();
        GenerateActiveQuests();
        LoadQuestsValue();
    }
    public void AddActiveQuest(Quest quest)
    {
        if (_listActiveQuests.Contains(quest)) { Debug.Log("Player is already have quest in active list"); }
        else _listActiveQuests.Add(quest);
    }
    public void UpdateProcessQuests(EnemyStats enemy = null, Item item = null, string param = "")
    {
        foreach (Quest quest in _listActiveQuests)
        {
            PlayerStats plStats = GetComponent<PlayerStats>();
            switch (quest.questType)
            {
                case QuestType.PassRooms:
                    quest.progressStatus = DungeonStats.Instance.passedRoom; break;
                case QuestType.CompleteDungeonOnTime:
                    quest.progressStatus = GameManager.Instance.GetTimeDungeonGoing; break;
                case QuestType.OpenChests:
                    quest.progressStatus = DungeonStats.Instance.numOpenChest; break;
                case QuestType.DefeatEnemyType:
                    {
                        if (enemy != null)
                        {
                            if (param == "kill_enemy")
                                if (enemy.enemyTupe.Name == quest.EnemyType.Name)
                                    quest.progressStatus += 1;
                        }
                    }
                    break;
                case QuestType.CollectItemFromDrop:
                    {
                        if (item != null)
                            if (item.Name == quest.ItemQuest.Name)
                                quest.progressStatus += 1;
                    }
                    break;

                case QuestType.WatchReclama:
                    {
                        if (param == "watch") quest.progressStatus += 1;
                    }
                    break;
                case QuestType.DefeatBoss:
                    {
                        if (param == "kill_boss") quest.progressStatus += 1;
                    }
                    break;
                case QuestType.LevelUp:
                    {
                        if (param == "level_up") quest.progressStatus += 1;
                    }
                    break;
                case QuestType.GetLevel:
                    {
                        quest.progressStatus = plStats.stats.Level;
                    }
                    break;
                case QuestType.GetDungeonLevel:
                    {
                        quest.progressStatus = GameManager.Instance.GetDungeonLevel;
                    }
                    break;
                case QuestType.PassTotalRooms:
                    {
                        quest.progressStatus = DungeonStats.Instance.allPassRoom;
                    }
                    break;
                case QuestType.KillTotalEnemy:
                    {
                        quest.progressStatus = DungeonStats.Instance.allKills;
                    }
                    break;
                case QuestType.HasItem:
                    {
                        if (_inv != null)
                        {
                            int count = _inv.GetCountItemsWithId(quest.ItemQuest.Id);
                            if (count >= 1)
                            {
                                quest.progressStatus = count;
                            }
                        }
                    }
                    break;
            }
        }

        SaveQuestsValue();
    }
    private void ResetQuestsValue()
    {
        foreach (Quest quest in _allQuests)
        {
            quest.progressStatus = 0;
            quest.isComplete = false;
        }
    }
    // загрузка результатов ТОЛЬКО основных квестов()
    private void LoadQuestsValue()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        foreach (Quest quest in _allQuests)
        {
            if (quest.isMainQuest)
            {
                float processValue = PlayerPrefs.GetFloat(id + "_questProcess_" + quest.Id);
                quest.progressStatus = processValue;
                int isComplete = PlayerPrefs.GetInt(id + "_questCompleted_" + quest.Id);
                if (isComplete == 1) quest.isComplete = true;
                else quest.isComplete = false;
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
    private void GenerateActiveQuests()
    {
        // заполнение списка основных квестов
        foreach (Quest quest in _allQuests)
        {
            if (quest.isMainQuest == true)
                _listActiveQuests.Add(quest);
        }
        // заполнения списка квестов для текущего подземелья
        List<Quest> listOnlyDunList = new List<Quest>();
        foreach (Quest quest in _allQuests)
        {
            if (quest.isInOnlyDungeon == true)
            {
                listOnlyDunList.Add(quest);
            }
        }
        // выбор заданного количества квестов для подземелья
        for (int i = 0; i < _countDunQuest; i++)
        {
            int randomValue = Random.Range(0, listOnlyDunList.Count);
            if (listOnlyDunList[randomValue] != null)
            {
                _listActiveQuests.Add(listOnlyDunList[randomValue]);
                listOnlyDunList.RemoveAt(randomValue);
            }
        }
    }
    public void GainRewardQuest(Quest quest)
    {
        PlayerStats plStats = GetComponent<PlayerStats>();
        if (quest.priceType == PriceType.Money)
        {
            //plStats.GainGold(quest.priceValue);
        }
        if (quest.priceType == PriceType.SkillUp)
        {
            plStats.stats.Skills[quest.numberSkillorAtt].LevelUp();
        }
        if (quest.priceType == PriceType.Item)
        {
            foreach (Item item in quest.priceItems)
            { plStats.GetInventory.AddItems(item); }
        }
        if (quest.priceType == PriceType.AttributeUp)
        {
            plStats.UpAttribute(quest.numberSkillorAtt, false);
        }
    }

}
