

/*
 * ����� ���������� �� ���������� ����� (��� �� �������� "������ ����� � ���������").
 * 
 * �������� � ��������� ������� �����, ������ ��� ����� ������
 * ���������� �������� ����������
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    private CharacterManager charMngr;
    private UIController uiCntrl;
    private MinerAi[] characters = new MinerAi[8]; // ----------------------------------- ������ ��� ���������� �� ���� �������.  
    private MinerAi atkChar; // --------------------------------------------------------- ���������� ��� ����������� ���������� � ������������� ����������.
    private MinerAi defChar;

    private int[] charTurn = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }; // ------------------- ������ ������������ ������� ���� ����������.
    private int indexsOfTurn = 0;// ----------------------------------------------------- ��������� ��� ������� ���� ����������.

    public static bool playerTurn = false; // ------------------------------------------- ���������� ��� ��������� ����������� �������� ������.
    public static bool turnStart = false;

    public static int roundCounter = 0; // ---------------------------------------------- ����� �����.

    public MinerAi DefendingChar { get { return defChar; } set { defChar = value; } } //  �������� ��� ��������� ��������� ���������.
    public MinerAi[] Character { get { return characters; } } // ------------------------ �������� ��� ���������� ������� ����������.


    private void Awake()
    {
        uiCntrl = GetComponent<UIController>();
        charMngr = GetComponent<CharacterManager>();
        charMngr.InitializingTeams(this);
    }
    private void Start()
    {        
        nextRound();
    }

    public void NextMove() // ------------------------------------------------- ��������� ���  
    {
        indexsOfTurn++;
        atkChar.ChoiseEffect().SetActive(false); // -   -   -   -   -   -   -   ���������� ������� ������ ��� ����������� ���� � ���� ��������
        uiCntrl.endTurn(atkChar.PosIndex); //   -   -   -   -   -   -   -   -   ��������� ���������� ��������� ����

        atkChar = null;
        defChar = null;

        if (indexsOfTurn == 8)
        {
            nextRound();
        }
        else
        {
            ChoiseCharacter();
        }
    }

    private void nextRound() // ------------------------------------------------ ������ ������ ������
    {
        roundCounter++;
        uiCntrl.newRound(roundCounter);
        CharRandomTurns(); 
        ChoiseCharacter();        
    }

    public void ChoiseCharacter() // ------------------------------------------- ����������� ���������� � ������������� ����������.
    {
        atkChar = characters[charTurn[indexsOfTurn]];

        if (atkChar.PosIndex < 4) // -   -   -   -   -   -   -   -   -   -   -   ��� ������.
        {
            playerTurn = true; 
            atkChar.ChoiseEffect().SetActive(true);
            uiCntrl.HideButtons(true);
        }
        else // -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -    ��� ����������.
        {
            playerTurn = false;
            defChar = characters[Random.Range(0, 4)]; // -   -   -   -   -   -   ���� �������� ���� ���������
            StartCoroutine(StartBattle());
        }        
    }
    
    private void CharRandomTurns() // ------------------------------------------ ������ ��������� ������� ���� ����������.
    {        
        int r;

        for(int i = 0; i < charTurn.Length; i++)
        {            
            int index = Random.Range(i, charTurn.Length);
            r = charTurn[index];

            charTurn[index] = charTurn[i];
            charTurn[i] = r;
        }

        indexsOfTurn = 0;
    }       

    public IEnumerator StartBattle() // ---------------------------------------- ����������� ���������� �� ��� ����
    {
        turnStart = false;
        uiCntrl.HideButtons(false);

        float atcDistance = 0; // -   -   -   -   -   -   -   -   -   -   -   -  ���������� ����������� �������� ����������
        float defDistance = 0;
        int aCharSortOrder = atkChar.myMesh.sortingOrder;
        int defCharSortOrder = defChar.myMesh.sortingOrder;

        if(atkChar.PosIndex < 4)
        {
            atcDistance = -2.5f;
            defDistance = 2.5f;
        }
        else
        {
            atcDistance = 2.5f;
            defDistance = -2.5f;
        }

        atkChar.myMesh.sortingOrder = 9; // -   -   -   -   -   -   -   -   -   �����������  sorting layer order ��� ����������� ����������� ����������
        defChar.myMesh.sortingOrder = 9;
        StartCoroutine(uiCntrl.ShowShadow(true));// -   -   -   -   -   -   -   ���������� ���� �����
        yield return new WaitForSeconds(1f);

        StartCoroutine(atkChar.Move(atcDistance)); // -   -   -   -   -   -     ������� �� �������� � ������ �������
        StartCoroutine(defChar.Move(defDistance));

        while (!atkChar.ready && !defChar.ready) // -   -   -   -   -   -   -   �������� ��������� ���������� � �����
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
                
        atkChar.RandomAttack(); // -   -   -   -   -   -   -   -   -   -   -    ������� �� �����

        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        StartCoroutine(uiCntrl.ShowShadow(false)); // -   -   -   -   -   -     ������� ���� �����, �������� �����, ������� �� ��������� �������
        defChar.HideBlood();
        StartCoroutine(atkChar.Move(atkChar.startPos.x));
        StartCoroutine(defChar.Move(defChar.startPos.x));
        
        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(2f); 
        atkChar.myMesh.sortingOrder = aCharSortOrder; // -   -   -   -   -     ��������������� sorting layer order ��� ����������� ����������� ����������
        defChar.myMesh.sortingOrder = defCharSortOrder;

        NextMove();
        
    }
}
