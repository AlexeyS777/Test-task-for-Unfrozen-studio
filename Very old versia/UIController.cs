

/*
 * Класс отвечающий за интерфейс в игре. (не отвечает за выбор персонажа курсором)
 * 
 * Запускает индикаторы завершения хода, 
 * изменяет индикатор номера раунда,
 * скрывает и показывает тень битвы и кнопки атаки и пропуска хода
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    /// <summary>
    /// Индикатор раунда.
    /// </summary>
    [SerializeField] private TextMeshProUGUI roundText;
    /// <summary>
    /// Индикатор окончания хода персонажа.
    /// </summary>
    [SerializeField] private GameObject[] charMoveIndicator = null;

    /// <summary>
    /// Тень перед ударом персонажей.
    /// </summary>
    [SerializeField] private SpriteRenderer Shadow = null;

    /// <summary>
    /// Кнопки "Атака" и "Пропуск хода".
    /// </summary>
    [SerializeField] private GameObject[] UiButtons = null;
        
    
    public void newRound(int value) // -------------------------------------- Скрыть идикаторы окончания хода для персонажей.
    {
        this.roundText.text = "Round: " + value;
        foreach(GameObject o in charMoveIndicator)
        {
            o.SetActive(false);
        }
    }

    public void endTurn(int value) // --------------------------------------- Показать индикатор окончания хода.
    {
        Debug.Log("indictor:" + value);
        charMoveIndicator[value].SetActive(true);
    }

    public void AttackButton() // ------------------------------------------- Метод для кнопки атаки, включает индикатор начала хода,
    {                          //                                             и крывает кнопку пропуска хода.
        GameManager.turnStart = true;
        UiButtons[0].SetActive(false);
        UiButtons[1].SetActive(false);
    }

    public void HideButtons(bool on) // ------------------------------------- Метод скрывающий кнопки атаки и пропуска хода.
    {
        foreach (GameObject b in UiButtons) b.SetActive(on);
    }

    public IEnumerator ShowShadow(bool on) // ------------------------------ Сопрограмма запуска и отключения тени, перед ударом персонажей.
    {
        float direction = (on) ? 0.7f : 0; 

        while(Shadow.color.a != direction)
        {
            Vector4 c = Shadow.color;
            c.w = direction;
            Shadow.color = Vector4.MoveTowards(Shadow.color, c, 1 * Time.deltaTime);
            yield return null;
        }
    }
}
