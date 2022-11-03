using UnityEngine;

[CreateAssetMenu(fileName = "MsgObject", menuName = "Project/MsgObject", order = 14)]
public class MsgObject : ScriptableObject
{
    [SerializeField] private TextLocalize _title;
    [SerializeField] private TextLocalize _description;
}
