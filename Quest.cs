using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Project/Quest", order = 5)]
public class Quest : ScriptableObject
{
    public int Id;
    public string Name;
    [Multiline(2)] public string Description;
    [Multiline(2)] public string RewardText;
    // для обозначения квестов, которые действуют только во время прохождения подземелья
    // т.е. при выходе из Dungeon результат таких Quests пропадает
    public bool isInOnlyDungeon;
    // главный ли это квест
    public bool isMainQuest;
    [Header("Type Quest")]
    [Space]
    // Тип квеста
    [SerializeField] QuestType m_QuestType = QuestType.None;
    public QuestType questType { get { return m_QuestType; } set { m_QuestType = value; } }
    // Тип награды
    [SerializeField] PriceType m_PriceType = PriceType.None;
    public PriceType priceType { get { return m_PriceType; } set { m_PriceType = value; } }

    public int priceModificator;

    public List<Item> priceItems = new List<Item>();

    public int numberSkillorAtt;
    // Необходимые выполненные квесты для появления текущего квеста
    public List<Quest> NeededCompletedQuests = new List<Quest>();

    // условия выполнения
    public float neededProgress;
    // текущий ход выполнения
    public float progressStatus;
    // выполнен ли квест(если да то за него нельзя получит награду)
    public bool IsCompletePermanent = false;
    public bool isComplete = false;
    // тип противника для выполнения квеста
    public Enemy EnemyType;

    public Item ItemQuest;
    public void Reset()
    {
        progressStatus = 0;
        isComplete = false;
        IsCompletePermanent = false;
    }
    public void CheckPermanentComplete()
    {
        if (progressStatus >= neededProgress)
        {
            IsCompletePermanent = true;
            GlobalSounds.Instance.SCompleteQuest();
        }
    }
    public void Save()
    {
        int temp = 0;
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetFloat(id + "_questProcess_" + +Id, progressStatus);
        if (isComplete == true) { temp = 1; }
        PlayerPrefs.SetInt(id + "_questCompleted_" + +Id, temp);
    }
}

public enum PriceType
{
    None,
    Money, // деньги
    SkillUp, // повышение навыка
    AttributeUp, // повышение атрибута
    Item, // получение предмета


}
public enum QuestType
{
    None = 0,
    // задания в подземелье 
    PassRooms = 1, // Прохождение всех комнат в подземелье
    SeachAllSecretPlace = 3, // отыскать все секретные места ****
    OpenChests = 4, // открыть столько-то N-число сундуков 
    DefeatEnemyType = 5, // одолеть число Т-число противников такого-то типа *
    CollectItemFromDrop = 6, // получить предмет указанный в квесте из дропа
    WatchReclama = 7, // просмотреть рекламу 
    DefeatBoss = 8, // одолеть босса
    LevelUp = 9, // повысить уровень
    CompleteDungeon = 16,// дойти до конца подземелья
    CompleteDungeonWithoutDeath = 10, // дойти до конца подземелья без смертей
    CompleteDungeonOnTime = 2, // дойти до конца подземелья за время T

    //

    // основные задания (не пропадают)
    GetLevel = 11, // получить уровень N
    GetDungeonLevel = 12, // достичь N уровння подземелья
    PassTotalRooms = 13, // пройти за все время N комнат
    KillTotalEnemy = 14, // убить за все время N противников
    HasItem = 15, // наличие предмета у персонажа



}