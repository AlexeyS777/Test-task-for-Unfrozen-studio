

/*
 * Класс отвечающий за управление игрой (так же является "Точкой входа в программу").
 * 
 * Начинает и завершает игровой раунд, решает кто ходит первым
 * Определяет действия персонажей
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    private CharacterManager charMngr;
    private UIController uiCntrl;
    private MinerAi[] characters = new MinerAi[8]; // ----------------------------------- Массив для персонажей во всех отрядах.  
    private MinerAi atkChar; // --------------------------------------------------------- Переменные для определения атакующего и защищающегося персонажей.
    private MinerAi defChar;

    private int[] charTurn = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }; // ------------------- Массив определяющий порядок хода персонажей.
    private int indexsOfTurn = 0;// ----------------------------------------------------- Индикатор для массива хода персонажей.

    public static bool playerTurn = false; // ------------------------------------------- Индикаторы для настройки отображения еффектов выбора.
    public static bool turnStart = false;

    public static int roundCounter = 0; // ---------------------------------------------- Номер ранда.

    public MinerAi DefendingChar { get { return defChar; } set { defChar = value; } } //  Свойство для настройки персонажа защитника.
    public MinerAi[] Character { get { return characters; } } // ------------------------ Свойство для заполнения массива персонажей.


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

    public void NextMove() // ------------------------------------------------- Следующий ход  
    {
        indexsOfTurn++;
        atkChar.ChoiseEffect().SetActive(false); // -   -   -   -   -   -   -   Отключение эффекта выбора для исправления бага с этим эффектом
        uiCntrl.endTurn(atkChar.PosIndex); //   -   -   -   -   -   -   -   -   Включение индикатора окончания хода

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

    private void nextRound() // ------------------------------------------------ Начало нового раунда
    {
        roundCounter++;
        uiCntrl.newRound(roundCounter);
        CharRandomTurns(); 
        ChoiseCharacter();        
    }

    public void ChoiseCharacter() // ------------------------------------------- Определение атакующего и защищающегося персонажей.
    {
        atkChar = characters[charTurn[indexsOfTurn]];

        if (atkChar.PosIndex < 4) // -   -   -   -   -   -   -   -   -   -   -   Ход игрока.
        {
            playerTurn = true; 
            atkChar.ChoiseEffect().SetActive(true);
            uiCntrl.HideButtons(true);
        }
        else // -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -    Ход противника.
        {
            playerTurn = false;
            defChar = characters[Random.Range(0, 4)]; // -   -   -   -   -   -   Враг выбирает кого атаковать
            StartCoroutine(StartBattle());
        }        
    }
    
    private void CharRandomTurns() // ------------------------------------------ Задаем случайный порядок хода персонажам.
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

    public IEnumerator StartBattle() // ---------------------------------------- Сопрограмма отвечающая за ход игры
    {
        turnStart = false;
        uiCntrl.HideButtons(false);

        float atcDistance = 0; // -   -   -   -   -   -   -   -   -   -   -   -  Определяем направление движения персонажей
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

        atkChar.myMesh.sortingOrder = 9; // -   -   -   -   -   -   -   -   -   Настраиваем  sorting layer order для корректного отображения персонажей
        defChar.myMesh.sortingOrder = 9;
        StartCoroutine(uiCntrl.ShowShadow(true));// -   -   -   -   -   -   -   Отображаем тень битвы
        yield return new WaitForSeconds(1f);

        StartCoroutine(atkChar.Move(atcDistance)); // -   -   -   -   -   -     Команда на движение к боевой позиции
        StartCoroutine(defChar.Move(defDistance));

        while (!atkChar.ready && !defChar.ready) // -   -   -   -   -   -   -   Проверка готовноси персонажей к атаки
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
                
        atkChar.RandomAttack(); // -   -   -   -   -   -   -   -   -   -   -    Команда на атаку

        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        StartCoroutine(uiCntrl.ShowShadow(false)); // -   -   -   -   -   -     Скрытие тени битвы, эффектов крови, возврат на стартовые позиции
        defChar.HideBlood();
        StartCoroutine(atkChar.Move(atkChar.startPos.x));
        StartCoroutine(defChar.Move(defChar.startPos.x));
        
        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(2f); 
        atkChar.myMesh.sortingOrder = aCharSortOrder; // -   -   -   -   -     Восстанавливаем sorting layer order для корректного отображения персонажей
        defChar.myMesh.sortingOrder = defCharSortOrder;

        NextMove();
        
    }
}
