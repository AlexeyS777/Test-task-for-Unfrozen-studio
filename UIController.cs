

/*
 * ����� ���������� �� ��������� � ����. (�� �������� �� ����� ��������� ��������)
 * 
 * ��������� ���������� ���������� ����, 
 * �������� ��������� ������ ������,
 * �������� � ���������� ���� ����� � ������ ����� � �������� ����
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    /// <summary>
    /// ��������� ������.
    /// </summary>
    [SerializeField] private TextMeshProUGUI roundText;
    /// <summary>
    /// ��������� ��������� ���� ���������.
    /// </summary>
    [SerializeField] private GameObject[] charMoveIndicator = null;

    /// <summary>
    /// ���� ����� ������ ����������.
    /// </summary>
    [SerializeField] private SpriteRenderer Shadow = null;

    /// <summary>
    /// ������ "�����" � "������� ����".
    /// </summary>
    [SerializeField] private GameObject[] UiButtons = null;
        
    
    public void newRound(int value) // -------------------------------------- ������ ��������� ��������� ���� ��� ����������.
    {
        this.roundText.text = "Round: " + value;
        foreach(GameObject o in charMoveIndicator)
        {
            o.SetActive(false);
        }
    }

    public void endTurn(int value) // --------------------------------------- �������� ��������� ��������� ����.
    {
        Debug.Log("indictor:" + value);
        charMoveIndicator[value].SetActive(true);
    }

    public void AttackButton() // ------------------------------------------- ����� ��� ������ �����, �������� ��������� ������ ����,
    {                          //                                             � ������� ������ �������� ����.
        GameManager.turnStart = true;
        UiButtons[0].SetActive(false);
        UiButtons[1].SetActive(false);
    }

    public void HideButtons(bool on) // ------------------------------------- ����� ���������� ������ ����� � �������� ����.
    {
        foreach (GameObject b in UiButtons) b.SetActive(on);
    }

    public IEnumerator ShowShadow(bool on) // ------------------------------ ����������� ������� � ���������� ����, ����� ������ ����������.
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
