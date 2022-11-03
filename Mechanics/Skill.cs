using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Project/Skill", order = 7)]
[System.Serializable]
public class Skill : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public string Description;
    public int Level;
    public int MaxLevel;

    public void LevelUp()
    {
        Level += 1;
    }
    public int SpendToLeveling()
    {
        if (Level <= 0) return 3;
        else return 1;
    }
    public virtual bool IsAvaibleToLevelUp(int points = 1)
    {
        if (Level < MaxLevel && points >= SpendToLeveling()) return true;
        else return false;
    }
    public void Reset()
    {
        Level = 0;
        Save();
    }
    public virtual void Save()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_" + Id + "_skill", Level);
    }
    public virtual void Load()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        Level = PlayerPrefs.GetInt(id + "_slot_" + Id + "_skill");

    }


}

