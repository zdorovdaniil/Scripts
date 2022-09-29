using UnityEngine;
public enum SoundMaterials
{
    None, Wood, Stone, Glass, DestroyWood, DestroyStone, DestroyGlass,
}
public class GlobalSounds : MonoBehaviour
{
    static public GlobalSounds Instance; private void Awake() { Instance = this; }
    [SerializeField] private AudioSource _sButtonClick; public void SButtonClick() { _sButtonClick.Play(); }
    [SerializeField] private AudioSource _sAwaibleLvlUp; public void SAwaibleLvlUp() { _sAwaibleLvlUp.Play(); }
    [SerializeField] private AudioSource _sHeartBeat; public void SHeartBeat() { _sHeartBeat.Play(); }
    [SerializeField] private AudioSource _sLevelUp; public void SLevelUp() { _sLevelUp.Play(); }
    [SerializeField] private AudioSource _sCompleteQuest; public void SCompleteQuest() { _sCompleteQuest.Play(); }
    [SerializeField] private AudioSource _sGetQuest; public void SGetQuest() { _sGetQuest.Play(); }
    [SerializeField] private AudioSource _sGetRewardQuest; public void SGetRewardQuest() { _sGetRewardQuest.Play(); }
    [SerializeField] private AudioSource _sCraftItem; public void SCraftItem() { _sCraftItem.Play(); }
    [SerializeField] private AudioSource _sCraftPoison; public void SCraftPoison() { _sCraftPoison.Play(); }
    [SerializeField] private AudioSource _sCraftFail; public void SCraftFail() { _sCraftFail.Play(); }
    [SerializeField] private AudioSource _sOpenWindow; public void SOpenWindow() { Playing(_sOpenWindow, true); }
    [SerializeField] private AudioSource _sCloseWindow; public void SCloseWindow() { Playing(_sCloseWindow, true); }
    [SerializeField] private AudioSource _sPlaceItem; public void SPlaceItem() { _sPlaceItem.Play(); }
    [SerializeField] private AudioSource _sTakeAll; public void STakeAll() { _sTakeAll.Play(); }
    [SerializeField] private AudioSource _sClickItem; public void SClickItem() { Playing(_sClickItem, true); }
    [SerializeField] private AudioSource _sClickGlass; public void SClickGlass() { _sClickGlass.Play(); }
    [SerializeField] private AudioSource _sAttention; public void SAttention() { _sAttention.Play(); }
    [SerializeField] private AudioSource _sBuySell; public void SBuySell() { _sBuySell.Play(); }
    [SerializeField] private AudioSource _sEquipArmor; public void SEquipArmor() { _sEquipArmor.Play(); }
    [SerializeField] private AudioSource _sEquipAmulet; public void SEquipAmulet() { _sEquipAmulet.Play(); }
    [SerializeField] private AudioSource _sEquipRing; public void SEquipRing() { _sEquipRing.Play(); }
    [SerializeField] private AudioSource _sEquipWeapon; public void SEquipWeapon() { _sEquipWeapon.Play(); }
    [SerializeField] private AudioSource _sUsePoison; public void SUsePoison() { _sUsePoison.Play(); }
    [SerializeField] private AudioSource _sUseFood; public void SUseFood() { _sUseFood.Play(); }
    [SerializeField] private AudioSource _sLadder; public void SLadder() { _sLadder.Play(); }
    [SerializeField] private AudioSource _sSkillUp; public void SSkillUp() { _sSkillUp.Play(); }
    [SerializeField] private AudioSource _sAttributeUp; public void SAttributeUp() { _sAttributeUp.Play(); }
    [SerializeField] private AudioSource _sOpenChest; public void SOpenChest() { _sOpenChest.Play(); }
    [SerializeField] private AudioSource _sCloseChest; public void SCloseChest() { _sCloseChest.Play(); }
    [SerializeField] private GameObject _getBuff; public void PGetBuff(Transform trf) { Instantiate(_getBuff, trf); }
    [SerializeField] private GameObject _crit; public void PCrit(Transform trf) { Instantiate(_crit, trf); }
    [SerializeField] private GameObject _wood;
    [SerializeField] private GameObject _stone;
    [SerializeField] private GameObject _glass;
    [SerializeField] private GameObject _destroyWood;
    [SerializeField] private GameObject _destroyStone;
    [SerializeField] private GameObject _destroyGlass;
    [SerializeField] private GameObject _pBoom; public void PBoom(Transform trf) { Instantiate(_pBoom, trf); }
    [SerializeField] private GameObject _pOpenChest; public void POpenChest(Transform trf) { Instantiate(_pOpenChest, trf); }
    [SerializeField] private GameObject _pCloseChest; public void PCloseChest(Transform trf) { Instantiate(_pCloseChest, trf); }

    private void Playing(AudioSource source, bool pitching = false, bool voluming = false)
    {
        if (pitching) source.pitch = Random.Range(0.7f, 1.1f);
        if (voluming) source.volume = Random.Range(0.5f, 0.7f);
        source.Play();
    }
    public void CreateSoundMaterial(Transform trf, SoundMaterials material)
    {
        GameObject newObject = null;
        switch (material)
        {
            case SoundMaterials.Wood: newObject = Instantiate(_wood, trf); break;
            case SoundMaterials.Stone: newObject = Instantiate(_stone, trf); break;
            case SoundMaterials.Glass: newObject = Instantiate(_glass, trf); break;
            case SoundMaterials.DestroyWood: newObject = Instantiate(_destroyWood, trf); break;
            case SoundMaterials.DestroyStone: newObject = Instantiate(_destroyStone, trf); break;
            case SoundMaterials.DestroyGlass: newObject = Instantiate(_destroyGlass, trf); break;
        }
        newObject.transform.SetParent(null);

    }
    public void CreateDestroySound(Transform trf, SoundMaterials material)
    {
        GameObject newObject = null;
        switch (material)
        {
            case SoundMaterials.Wood: newObject = Instantiate(_destroyWood, trf); break;
            case SoundMaterials.Stone: newObject = Instantiate(_destroyStone, trf); break;
            case SoundMaterials.Glass: newObject = Instantiate(_destroyGlass, trf); break;
        }
        newObject.transform.SetParent(null);

    }
}