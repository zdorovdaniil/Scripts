using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQuest : MonoBehaviour
{
    [SerializeField] private Quest _addingQuest;

    public void QuestInstance(PlayerQuest playerQuest)
    {
        playerQuest.AddActiveQuest(_addingQuest);
    }
}
