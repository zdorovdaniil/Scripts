using UnityEngine;
using System.Collections;
using TMPro;

public class SceneLoadingUI : MonoBehaviour
{
    public static SceneLoadingUI Instance;
    [SerializeField] private TMP_Text _loadingMainText;
    [SerializeField] private Transform _loadingPanel;
    private Animator _anim;

    private void Awake()
    {
        Instance = this;
        _anim = GetComponent<Animator>();
    }
    public void OpenLoadingUI(string mainText, bool isNewLoading = false)
	{
        _loadingMainText.text = mainText;
        _anim.SetBool("IsActive", true);
        _loadingPanel.gameObject.SetActive(true);
    }
    public void CloseLoadingUI()
	{
        _anim.SetBool("IsActive", false);
        _loadingPanel.gameObject.SetActive(false);
    }
}
