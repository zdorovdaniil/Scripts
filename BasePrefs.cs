﻿using System.Collections.Generic;
using UnityEngine;

public class BasePrefs : MonoBehaviour
{
    public static BasePrefs instance; void Awake() { instance = this; }
    public List<Skill> AvaibleSkills = new List<Skill>();

    // Получение доспута к базе данных
    [SerializeField] private ItemDatabase _itemDatabase; public ItemDatabase GetItemDatabase => _itemDatabase;

    // Получение иконки
    [SerializeField] private List<Sprite> _levelIcons = new List<Sprite>(); public Sprite GetIcon(int num) => _levelIcons[num];

    // Получение материала для комнат где побывал игрок
    [SerializeField] private Material _matGrey; public Material GetGreyMaterial => _matGrey;


}