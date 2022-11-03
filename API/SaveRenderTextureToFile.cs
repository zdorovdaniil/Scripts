using UnityEngine;
using UnityEditor;

public class SaveRenderTextureToFile
{
    public static void SaveRTToFile(int slotId, RenderTexture inputTexture = null)
    {
        //RenderTexture rt = Selection.activeObject as RenderTexture;
        RenderTexture rt = inputTexture;


        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        string path = Application.persistentDataPath + slotId + ".png";
#if UNITY_EDITOR
        path = "Assets/Resources/Textures/CameraRendering/ScrImage_" + slotId + ".png";
#endif
        // string path = "Assets/Resources/Textures/CameraRendering/ScrImage_" + slotId + ".png";
        System.IO.File.WriteAllBytes(path, bytes);
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path);
#endif
        Debug.Log("Saved to " + path);
    }
    public static void SaveT2DToFile(int slotId, Texture2D inputTexture = null)
    {

        byte[] bytes;
        bytes = inputTexture.EncodeToPNG();

        string path = Application.persistentDataPath + slotId + ".png";
#if UNITY_EDITOR
        path = "Assets/Resources/Textures/CameraRendering/ScrImage_" + slotId + ".png";
#endif
        System.IO.File.WriteAllBytes(path, bytes);
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path);
#endif
        Debug.Log("Saved to " + path);
    }
}