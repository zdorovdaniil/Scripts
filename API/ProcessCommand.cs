using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessCommand : MonoBehaviour
{
    private const int CONSTmaxDungeonLevel = 5;
    public static void ToTexture2D(RenderTexture rTex, Texture2D inputTexture)
    {
        inputTexture.width = rTex.width; inputTexture.height = rTex.height;
        // inputTexture = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        inputTexture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        inputTexture.Apply();
        Debug.Log("Texture set: " + inputTexture.name);
    }
    public static int RandomValue => UnityEngine.Random.Range(0, 100);
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
    public static float[] TransformToFloat(Transform trf)
    {
        float[] vector = new float[3];
        vector[0] = trf.position.x;
        vector[1] = trf.position.y;
        vector[2] = trf.position.z;
        return vector;
    }
    public static int ConvertBoolToInt(bool value) => value ? 1 : 0;
    public static bool ConvertIntToBool(int value) => value == 1 ? true : false;
    public static float[] ConvertVector3ToFloat(Vector3 vector3)
    {
        float[] vector = new float[3];
        vector[0] = vector3.x;
        vector[1] = vector3.y;
        vector[2] = vector3.z;
        return vector;
    }
    //public static void SetNumRooms =>
    //public static int GetNumRooms => PlayerPrefs.GetInt("numRooms");
    public static int MaxDungeonLevel => PlayerPrefs.GetInt(CurActiveSlot + "_slot_MaxDungeonLevel");
    public static int CheckIsLevelUpDungeonLevel()
    {
        if (!DungeonStats.Instance.IsCompleteDungeon)
        {
            if (GetDungeonLevel == MaxDungeonLevel)
            {
                Debug.Log("Level up MaxDungeon level from " + GetDungeonLevel + " to " + GetDungeonLevel + 1);
                SetMaxDungeonLevel(GetDungeonLevel + 1);
                return 0;
            }
            else { return 1; };
        }
        else { Debug.Log("Dungeon is already completed"); return 2; }
    }
    public static void SetMaxDungeonLevel(int value)
    {
        if (value < CONSTmaxDungeonLevel && value >= 1) PlayerPrefs.SetInt(CurActiveSlot + "_slot_MaxDungeonLevel", value); else return;
    }
    public static int CurActiveSlot => PlayerPrefs.GetInt("activeSlot");
    public static int GetDungeonLevel => PlayerPrefs.GetInt("dungeonLevel");
    public static void SetDungeonLevel(int value)
    {
        if (value >= 1 && value <= MaxDungeonLevel) PlayerPrefs.SetInt("dungeonLevel", value); else Debug.Log("set " + value + " / dungeonLevel " + GetDungeonLevel + " of max " + MaxDungeonLevel);
    }
    public static bool CheckTagInGameObject(string tagGameObject, Tags[] tagForCheck)
    {
        for (int i = 0; i < tagForCheck.Length; i++)
        {
            if (tagForCheck[i].ToString() == tagGameObject) { return true; }
        }
        return false;
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
            craftChance = Mathf.FloorToInt(100 - (1 + Mathf.Sqrt((craftDifficult * craftDifficult) / craftSkill) * 10));
        }
        return craftChance;
    }
    public static void SwithStatusInCollection(bool status, List<GameObject> listObjects)
    {
        foreach (GameObject obj in listObjects)
        {
            obj.SetActive(status);
        }
    }
    public static void SetParent(Transform changeObj, Transform parantObj)
    {
        changeObj.SetParent(parantObj);
    }
    public static IEnumerator DestroyGameObjectDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        {
            Destroy(gameObject);
        }
    }
    public static IEnumerator DoActionDelay(Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        {
            action.Invoke();
        }
    }
    public static IEnumerator DestroyGameObjectDelay(Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        {
            action.Invoke();
        }
    }
}
