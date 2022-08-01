using UnityEngine;

public class GlobalSounds : MonoBehaviour
{
    static public GlobalSounds Instance; private void Awake() { Instance = this; }
    [SerializeField] private AudioSource _sButtonClick; public void SButtonClick() { _sButtonClick.Play(); }
    [SerializeField] private AudioSource _sClearRoom; public void SClearRoom() { _sClearRoom.Play(); }
    [SerializeField] private AudioSource _sLevelUp; public void SLevelUp() { _sLevelUp.Play(); }
    [SerializeField] private AudioSource _sCompleteQuest; public void SCompleteQuest() { _sCompleteQuest.Play(); }
    [SerializeField] private AudioSource _sGetQuest; public void SGetQuest() { _sGetQuest.Play(); }
    [SerializeField] private AudioSource _sCrafting; public void SCraftQuest() { _sCrafting.Play(); }
    [SerializeField] private AudioSource _sOpenWindow; public void SOpenWindow() { _sOpenWindow.Play(); }
    [SerializeField] private AudioSource _sCloseWindow; public void SCloseWindow() { _sCloseWindow.Play(); }
    [SerializeField] private AudioSource _sPlaceItem; public void SPlaceItem() { _sPlaceItem.Play(); }
    [SerializeField] private AudioSource _sClickItem; public void SClickItem() { _sClickItem.Play(); }
    [SerializeField] private AudioSource _sAttention; public void SAttention() { _sAttention.Play(); }
    [SerializeField] private AudioSource _sBuySell; public void SBuySell() { _sBuySell.Play(); }
    [SerializeField] private AudioSource _sEquipArmor; public void SEquipArmor() { _sEquipArmor.Play(); }
}
