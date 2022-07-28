using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCommand : MonoBehaviour
{
    public static string ToTime(float timeInt)
    {
        int minutes = Mathf.RoundToInt(timeInt / 60);
        int seconds = Mathf.RoundToInt(timeInt % 60);

        string min;
        string sec;
        if (minutes <= 9) min = "0" + minutes; else min = minutes.ToString();
        if (seconds <= 9) sec = "0" + seconds; else sec = seconds.ToString();
        return min + ":" + sec;
    }
    public static void ClearChildObj(Transform obj)
    {
        for (int i = 0; i < obj.childCount; i++)
        {
            Destroy(obj.GetChild(i).gameObject);
        }
    }
    public static int ChanceToCraft(int craftDifficult, int craftSkill)
    {
        int craftChance = 0;
        if (craftSkill <= 0) return craftChance;
        else
        {
            craftChance = Mathf.FloorToInt(100- (1+Mathf.Sqrt((craftDifficult * craftDifficult) / craftSkill)*10));
        }
        return craftChance;
    }
}
