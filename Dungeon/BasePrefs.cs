using System.Collections.Generic;
using UnityEngine;

public class BasePrefs : MonoBehaviour
{
    public static BasePrefs instance; void Awake() { instance = this; }
    public List<Skill> AvaibleSkills = new List<Skill>();
    public List<Attribut> AvaibleAttributes = new List<Attribut>();
    [SerializeField] private ItemRequirement _emptyRequirement; public ItemRequirement GetEmptyRequirement => _emptyRequirement;

    // Получение доступа к базе данных предметов
    [SerializeField] private ItemDatabase _itemDatabase; public ItemDatabase GetItemDatabase => _itemDatabase;
    [SerializeField] private BuffDatabase _buffDatabase; public BuffDatabase GetBuffDatabase => _buffDatabase;

    // Получение иконки
    [SerializeField] private List<Sprite> _levelIcons = new List<Sprite>(); public Sprite GetIcon(int num) => _levelIcons[num];

    // Получение материала для комнат где побывал игрок
    [SerializeField] private Material _matGrey; public Material GetGreyMaterial => _matGrey;
    [SerializeField] private Mesh _meshOpenedChest; public Mesh GetMeshOpenedChest => _meshOpenedChest;
    [SerializeField] private Mesh _meshClosedChest; public Mesh GetMeshClosedChest => _meshClosedChest;


}
