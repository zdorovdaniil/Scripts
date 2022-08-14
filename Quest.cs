using System.Collections.Generic;
using stats; // Пространство имен stats из скрипта Stats
using UnityEngine;
[CreateAssetMenu]

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

    public int priceValue;

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

    public void CheckPermanentComplete()
    {
        if (neededProgress >= progressStatus)
        {
            IsCompletePermanent = true;
            GlobalSounds.Instance.SCompleteQuest();

        }

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
    None,
    // задания в подземелье ()
    PassRooms, // Прохождение всех комнат в подземелье
    CompleteDungeonOnTime, // пройти подземелье за время T
    SeachAllSecretPlace, // отыскать все секретные места ****
    OpenChests, // открыть столько-то N-число сундуков +
    DefeatEnemyType, // одолеть число Т-число противников такого-то типа *
    CollectItemFromDrop, // получить предмет указанный в квесте из дропа
    WatchReclama, // просмотреть рекламу 
    DefeatBoss, // одолеть босса
    LevelUp, // повысить уровень
    CompleteDungeonWithoutDeath, // пройти подземелье без смертей
    //

    // основные задания (не пропадают)
    GetLevel, // получить уровень N
    GetDungeonLevel, // достичь N уровння подземелья
    PassTotalRooms, // пройти за все время N комнат
    KillTotalEnemy, // убить за все время N противников

    HasItem, // наличие предмета


}