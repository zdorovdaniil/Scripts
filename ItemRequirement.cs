using UnityEngine;

[CreateAssetMenu(fileName = "ItemRequirement", menuName = "Project/ItemRequirement", order = 8)]
[System.Serializable]
public class ItemRequirement : ScriptableObject
{
    [SerializeField] private Attribut[] _attributes;
    [SerializeField] private int[] _levelPerAttribute;
    [SerializeField] private Skill[] _skills;
    [SerializeField] private int[] _levelPerSkills;
    [SerializeField] private int _needPlayerLevel;
    [SerializeField] private int _needDungeonLevel;

    public bool CheckReqirement(PlayerStats playerStats)
    {
        bool checkAttributes()
        {
            int succPass = 0;
            for (int i = 0; i < _attributes.Length; i++)
            {
                if (playerStats.stats.Attributes[_attributes[i].Id].Level >= _levelPerAttribute[i])
                { succPass = +1; }
            }
            if (succPass >= _attributes.Length) return true;
            else return false;
        }
        bool checkSkills()
        {
            int succPass = 0;
            for (int i = 0; i < _skills.Length; i++)
            {
                if (playerStats.stats.Skills[_skills[i].Id].Level >= _levelPerSkills[i])
                { succPass = +1; }
            }
            if (succPass >= _skills.Length) return true;
            else return false;
        }
        bool checkOthers()
        {
            if (playerStats.stats.Level >= _needPlayerLevel && GameManager.Instance.GetDungeonLevel >= _needDungeonLevel)
                return true;
            else return false;
        }
        if (checkOthers() && checkSkills() && checkAttributes())
        { return true; }
        else { return false; }
    }

}
