using UnityEngine;

[CreateAssetMenu(fileName = "Attribut", menuName = "Project/Attribut", order = 8)]
[System.Serializable]
public class Attribut : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    [SerializeField] private TextLocalize _descriptionText; public string Description => _descriptionText ? _descriptionText.Text() : " ";

    public int MaxLevel;


}
public class AttributeStat
{
    public Attribut Attr;
    public int Level;
    public AttributeStat(Attribut attr, int level = 10)
    {
        Attr = attr;
        Level = level;
    }
    public void LevelUp()
    {
        Level += 1;
    }
    public void Reset()
    {
        Level = 10;
        Save();
    }
    public void Save()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_" + Attr.Id + "_attribute", Level);
    }
    public void Load()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        if (PlayerPrefs.HasKey(id + "_slot_" + Attr.Id + "_attribute"))
        {
            Level = PlayerPrefs.GetInt(id + "_slot_" + Attr.Id + "_attribute");
        }
        else { Level = 10; }

    }
    public bool IsAvaibleToLevelUp(int points)
    {
        if (Level < Attr.MaxLevel && points >= 1) return true; else return false;
    }
}
