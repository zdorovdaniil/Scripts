using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Project/Skill", order = 7)]
[System.Serializable]
public class Skill : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public string Description;
    public int Level = 0;
    public int MaxLevel = 1;

    public void LevelUp()
    {
        Level += 1;
    }
    public int SpendToLeveling()
    {
        if (Level <= 0) return 3;
        else return 1;
    }
    public bool IsAvaibleToLevelUp(int skillPoints = 1)
    {
        if (Level < MaxLevel && skillPoints >= SpendToLeveling()) return true;
        else return false;
    }
    public void ResetSkill()
    {
        Level = 0;
        SaveSkill();
    }
    public void SaveSkill()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_" + Id + "_skill", Level);
    }
    public void LoadSkill()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        Level = PlayerPrefs.GetInt(id + "_slot_" + Id + "_skill");

    }


}

