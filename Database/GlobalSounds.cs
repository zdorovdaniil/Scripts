using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public enum SoundMaterials
{
    None, Wood, Stone, Metal, DestroyStandartMat
}
public class GlobalSounds : MonoBehaviour
{
    static public GlobalSounds Instance; private void Awake() { Instance = this; }
    [SerializeField] private AudioSource _sButtonClick; public void SButtonClick() { _sButtonClick.Play(); }
    [SerializeField] private AudioSource _sAwaibleLvlUp; public void SAwaibleLvlUp() { _sAwaibleLvlUp.Play(); }
    [SerializeField] private AudioSource _sLevelUp; public void SLevelUp() { _sLevelUp.Play(); }
    [SerializeField] private AudioSource _sCompleteQuest; public void SCompleteQuest() { _sCompleteQuest.Play(); }
    [SerializeField] private AudioSource _sGetQuest; public void SGetQuest() { _sGetQuest.Play(); }
    [SerializeField] private AudioSource _sCrafting; public void SCrafting() { _sCrafting.Play(); }
    [SerializeField] private AudioSource _sOpenWindow; public void SOpenWindow() { _sOpenWindow.Play(); }
    [SerializeField] private AudioSource _sCloseWindow; public void SCloseWindow() { _sCloseWindow.Play(); }
    [SerializeField] private AudioSource _sPlaceItem; public void SPlaceItem() { _sPlaceItem.Play(); }
    [SerializeField] private AudioSource _sTakeAll; public void STakeAll() { _sTakeAll.Play(); }
    [SerializeField] private AudioSource _sClickItem; public void SClickItem() { _sClickItem.Play(); }
    [SerializeField] private AudioSource _sAttention; public void SAttention() { _sAttention.Play(); }
    [SerializeField] private AudioSource _sBuySell; public void SBuySell() { _sBuySell.Play(); }
    [SerializeField] private AudioSource _sEquipArmor; public void SEquipArmor() { _sEquipArmor.Play(); }
    [SerializeField] private AudioSource _sLadder; public void SLadder() { _sLadder.Play(); }
    [SerializeField] private AudioSource _sSkillUp; public void SSkillUp() { _sSkillUp.Play(); }
    [SerializeField] private AudioSource _sAttributeUp; public void SAttributeUp() { _sAttributeUp.Play(); }
    [SerializeField] private AudioSource _sOpenChest; public void SOpenChest() { _sOpenChest.Play(); }
    [SerializeField] private AudioSource _sCloseChest; public void SCloseChest() { _sCloseChest.Play(); }
    [SerializeField] private GameObject _Wood;
    [SerializeField] private GameObject _Stone;
    [SerializeField] private GameObject _Metal;
    [SerializeField] private GameObject _DestroyStandartMat;
    [SerializeField] private GameObject _OpenChest; public void POpenChest(Transform trf) { Instantiate(_OpenChest, trf); }
    [SerializeField] private GameObject _CloseChest; public void PCloseChest(Transform trf) { Instantiate(_CloseChest, trf); }

    public void CreateSoundMaterial(Transform trf, SoundMaterials material)
    {
        GameObject newObject = null;
        switch (material)
        {
            case SoundMaterials.Wood: newObject = Instantiate(_Wood, trf); break;
            case SoundMaterials.Stone: newObject = Instantiate(_Stone, trf); break;
            case SoundMaterials.Metal: newObject = Instantiate(_Metal, trf); break;
            case SoundMaterials.DestroyStandartMat: newObject = Instantiate(_DestroyStandartMat, trf); break;
        }
        newObject.transform.SetParent(null);
    }
}