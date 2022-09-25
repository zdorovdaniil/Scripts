using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    private Quest _quest;
    private PlayerQuest _playerQuest;
    [SerializeField] private TMP_Text _nameQuest;
    [SerializeField] private TMP_Text _questType;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private TMP_Text _reward;
    [SerializeField] private Transform _buttonGainReward;
    [SerializeField] private Image _background;
    [SerializeField] private Color _backgroungColor;
    public void SetUp(Quest quest, PlayerQuest playerQuest)
    {
        _buttonGainReward.gameObject.SetActive(false);
        _playerQuest = playerQuest;
        _quest = quest;
        _nameQuest.text = quest.Name;
        _description.text = quest.Description;
        _status.text = quest.progressStatus.ToString() + " / " + quest.neededProgress.ToString();
        _reward.text = quest.RewardText;
        if (quest.isMainQuest) { _background.color = _backgroungColor; _questType.text = "MAIN"; }
        else { _questType.text = "IN THIS DUNGEON"; }
        if (quest.progressStatus >= quest.neededProgress && quest.isComplete == false)
        { _buttonGainReward.gameObject.SetActive(true); }


    }
    public void GainReward()
    {
        _playerQuest.GainRewardQuest(_quest);
        _quest.isComplete = true;
        LogUI.Instance.Loger("Quest: " + _quest.Name + " is complete");
        QuestUI.instance.FillListsOfQuest(_playerQuest);
        GlobalSounds.Instance.SGetRewardQuest();
    }
}

