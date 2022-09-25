using System.Collections.Generic;
using UnityEngine;
public class PlayerQuest : MonoBehaviour
{
    public static PlayerQuest instance;
    [Header("List quests")]
    [SerializeField] private List<Quest> _listActiveQuests = new List<Quest>();
    public List<Quest> GetActiveQuests => _listActiveQuests;

    [Header("Quest count")]

    private Inventory _inv;
    private PlayerStats _plStats;


    private void Start()
    {
        instance = this;
        _inv = GetComponent<Inventory>();
        _plStats = GetComponent<PlayerStats>();
    }
    public void InstainceQuests()
    {
        _listActiveQuests = DungeonQuests.Instance.GenerateActiveQuests();
        DungeonQuests.Instance.LoadQuestsValue();
    }
    public void AddActiveQuest(Quest quest)
    {
        if (_listActiveQuests.Contains(quest)) { Debug.Log("Player is already have quest in active list"); }
        else _listActiveQuests.Add(quest);
        GlobalSounds.Instance.SGetQuest();
    }
    public void UpdateProcessQuests(EnemyStats enemy = null, Item item = null, string param = "")
    {
        foreach (Quest quest in _listActiveQuests)
        {
            if (quest.IsCompletePermanent) continue;
            switch (quest.questType)
            {
                case QuestType.PassRooms:
                    quest.progressStatus = DungeonStats.Instance.passedRoom; break;
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
                        quest.progressStatus = _plStats.stats.Level;
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
                case QuestType.CompleteDungeon:
                    {
                        if (param == "completeDungeon") quest.progressStatus += 1;
                    }
                    break;
                case QuestType.CompleteDungeonWithoutDeath:
                    {
                        if (param == "completeDungeon" && DungeonStats.Instance.numDeath <= 0) quest.progressStatus += 1;
                    }
                    break;
                case QuestType.CompleteDungeonOnTime:
                    {
                        if (param == "completeDungeon" && quest.neededProgress <= GameManager.Instance.GetTimeDungeonGoing) quest.progressStatus = quest.neededProgress;
                    }
                    break;

            }
            quest.CheckPermanentComplete();
            quest.Save();
        }
    }


    public void GainRewardQuest(Quest quest)
    {
        for (int i = 0; i < quest.priceModificator; i++)
        {
            if (quest.priceType == PriceType.Money)
            {
                //_plStats.GainGold(quest.priceValue);
            }

            if (quest.priceType == PriceType.Item)
            {
                foreach (Item item in quest.priceItems)
                { _plStats.GetInventory.AddItems(item); }
            }
            if (quest.priceType == PriceType.SkillUp)
            {
                _plStats.stats.Skills[quest.numberSkillorAtt].LevelUp();
            }
            if (quest.priceType == PriceType.AttributeUp)
            {
                _plStats.UpAttribute(quest.numberSkillorAtt, false);
            }
        }

    }
}
