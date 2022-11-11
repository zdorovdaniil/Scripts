using System.Collections.Generic;
using UnityEngine;

public class BasePrefs : MonoBehaviour
{
    public static BasePrefs instance; void Awake() { instance = this; _itemDatabase.Initialize(); }
    [SerializeField] private TextLanguage[] _avaibleLanguages; public TextLanguage GetTextLanguage(int id) => _avaibleLanguages[id];
    public List<Skill> AvaibleSkills = new List<Skill>();
    public List<Attribut> AvaibleAttributes = new List<Attribut>();
    [SerializeField] private ItemRequirement _emptyRequirement; public ItemRequirement GetEmptyRequirement => _emptyRequirement;

    // Получение доступа к базе данных предметов
    [SerializeField] private ItemDatabase _itemDatabase; public ItemDatabase GetItemDatabase => _itemDatabase;
    [SerializeField] private BuffDatabase _buffDatabase; public BuffDatabase GetBuffDatabase => _buffDatabase;
    [SerializeField] private ImproveDatabase _improveDatabase; public ImproveDatabase GetImproveDatabase => _improveDatabase;

    // Получение иконки
    [SerializeField] private List<Sprite> _levelIcons = new List<Sprite>(); public Sprite GetIcon(int num) => _levelIcons[num];

    // Получение материала для комнат где побывал игрок
    [SerializeField] private Material _matGrey; public Material GetGreyMaterial => _matGrey;
    [SerializeField] private Mesh _meshOpenedChest; public Mesh GetMeshOpenedChest => _meshOpenedChest;
    [SerializeField] private Mesh _meshClosedChest; public Mesh GetMeshClosedChest => _meshClosedChest;
    [SerializeField] private Texture2D _nullImage; public Texture2D GetNullImage => _nullImage;
    [SerializeField] private Texture2D[] _crsCharacterTexture2D;
    public Texture2D GetTexture2DCrsCharacter(int idProfileSlot)
    {
        string path = Application.persistentDataPath + idProfileSlot + ".png";
#if UNITY_EDITOR
        path = "Assets/Resources/Textures/CameraRendering/ScrImage_" + idProfileSlot + ".png";
#endif
        var rawData = System.IO.File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(300, 540, TextureFormat.ARGB32, false); // Create an empty Texture; size doesn't matter (she said)
        tex.filterMode = FilterMode.Point;
        tex.LoadImage(rawData);
        return tex;
    }
    public void SetImageCharacter(RenderTexture tex, int idSlot)
    { SaveRenderTextureToFile.SaveRTToFile(idSlot, tex); }


}
