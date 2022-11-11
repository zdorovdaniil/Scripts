using UnityEngine;

[CreateAssetMenu(fileName = "ImproveDatabase", menuName = "DungeonMaster/ImproveDatabase", order = 15)]
public class ImproveDatabase : ScriptableObject
{
    [SerializeField] Improve[] _improves;
    public void LoadImprovesFromSave()
    {
        for (int i = 0; i < _improves.Length; i++)
        {
            _improves[i].LoadImprove();
        }
    }
}
