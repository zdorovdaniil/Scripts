using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image ImClicked; // изображение подсветки
    [SerializeField] private PlayerController movePlayer;

    [SerializeField] private int ComboNum = 0; //Счетчик для Атаки
    [SerializeField] private float Reset = 1.5f; //Таймер сброса атаки
    [SerializeField] private float ResetTime; // Время для сброса атаки

    [SerializeField] private bool isDownButtonAttack = false;
    [SerializeField] private float timer;
    [SerializeField] private int NumComboAttack; // кол-во комбо ударов, которые персонаж может выполнить

    private void Start()
    {
        ImClicked.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {

        // если зажата клавиша атаки
        if (isDownButtonAttack == true)
        {
            // идет таймер
            timer += Time.deltaTime;
            // если таймер доходт до 0.7, то происходит длинная атака
            if (timer > 0.5f)
            {
                movePlayer.PlayerComboAttack();
                timer = 0f;
                isDownButtonAttack = false;
            }
        }
        else timer = 0f;


        // проверка на атаку
        if (ComboNum > 0)
        {
            // идет таймер
            Reset += Time.deltaTime;
            // если таймер больше 0.8f, то пропадает возможность сделать комбо атаку
            if (Reset > ResetTime)
            {
                movePlayer.PlayerStopAttack();
                ComboNum = 0;
            }
        }
        ResetTime = 1f;  // reset time доступный интервал для нанесения атаки

        // временный код для ПК
        if (Input.GetKeyDown(KeyCode.E)) DownPressAttack();
        if (Input.GetKeyUp(KeyCode.E)) UpPressAttack();
        //
    }
    public void ClickButton()
    {
        DownPressAttack(); UpPressAttack();
    }
    // нажатие джостика
    public virtual void OnPointerDown(PointerEventData ped)
    {
        DownPressAttack();
        OnDrag(ped);
    }
    // отжатие джостика
    public virtual void OnPointerUp(PointerEventData ped)
    {
        UpPressAttack();
    }
    // удержание нажатой кнопки
    public virtual void OnDrag(PointerEventData ped)
    {

    }
    public void DownPressAttack()
    {
        isDownButtonAttack = true;
        // 1 удар комбо
        if (ComboNum == 0)
        {
            ComboNum = 1;
            movePlayer.PlayerAttack(ComboNum);
            Reset = 0f;
        }
        // 2 удар для комбо возможен с диапозоном от 0.4 до 1 сек
        else if ((ComboNum == 1) && (Reset > 0.4f) && (Reset < 1f))
        {
            ComboNum += 1; movePlayer.PlayerAttack(ComboNum); Reset = 0f;
        }
        // 3 удар для комбо возможен с диапозоном от 1 до 1.5 сек
        else if ((ComboNum == 2) && (Reset > 0.55f) && (Reset < 1f))
        {
            ComboNum += 1;
            movePlayer.PlayerAttack(ComboNum); Reset = 0f;
            Debug.Log("Attack on " + Reset);

        }
        // меняется изображение иконки атаки
        ImClicked.gameObject.SetActive(true);
    }


    public void UpPressAttack()
    {
        isDownButtonAttack = false;
        // меняется изображение иконки атаки
        ImClicked.gameObject.SetActive(false);
    }


}
