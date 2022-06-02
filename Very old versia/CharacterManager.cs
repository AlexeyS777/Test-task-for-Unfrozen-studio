

/*
 * Класс отвечающий за создание персонажей в отрядах
 * 
 * Создает и настраивает персонажей в каждом отряде.
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    /// <summary>
    /// Добавьте префабы персонажей.
    /// </summary>
    [SerializeField] private GameObject characterPrefub = null; //----------------- Префаб персонажа

    private GameManager gmMngr; 

    // Запускаем механизм создания персонажей для каждого отряда.
    public void InitializingTeams(GameManager gm)
    {
        gmMngr = gm;

        //-----------------------------------------------------------------------------------------------------------------------------
        for (int i = 0; i < 8; i++)                          
        {
            gmMngr.Character[i] = CreatCharacter(i);                             // Создаем персонажей для каждого отряда и позицыы в нем.
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }


    // Метод создания персонажа numOfPos - номер позиции персонажа.
    private MinerAi CreatCharacter(int numOfPos)
    {
        GameObject character = Instantiate(characterPrefub) as GameObject;
        SkeletonMecanim skComponent = character.GetComponent<SkeletonMecanim>(); // Компонент, необходимый для изменения spine анимации.
        MinerAi charAi = character.GetComponent<MinerAi>();                                                                                  // 

        //------------------------------------------------------------------------------------------------------------------------------
        int skinIndex = Random.Range(0, 2);
        string skinName = (skinIndex < 1) ? "base" : "elite";                    // Выбираем скин для персонажа случайным образом.
        skComponent.initialSkinName = skinName;
        //------------------------------------------------------------------------------------------------------------------------------


        MeshRenderer charMesh = character.GetComponent<MeshRenderer>();
        charMesh.sortingOrder = (numOfPos < 4) ? numOfPos + 3 : numOfPos - 1 ; //-- Сортируем персонажей по слоям, для правильного отображения.        

        Vector3 pos = Vector3.zero;

        switch (numOfPos) //------------------------------------------------------- Назначаем позицию персонажу.
        {
            case 0:
                pos = new Vector3(-2f, -2.6f, 0f);
                break;
            case 1:
                pos = new Vector3(-4f, -2.6f, 0f);
                break;
            case 2:
                pos = new Vector3(-6f, -2.6f, 0f);
                break;
            case 3:
                pos = new Vector3(-8f, -2.6f, 0f);
                break;
            case 4:
                pos = new Vector3(2f, -2.6f, 0f);
                break;
            case 5:
                pos = new Vector3(4f, -2.6f, 0f);
                break;
            case 6:
                pos = new Vector3(6f, -2.6f, 0f);
                break;
            case 7:
                pos = new Vector3(8f, -2.6f, 0f);
                break;
        }

        if (numOfPos > 3)
        {
            skComponent.initialFlipX = true;   //------------------ Поворачиваем персонажа, если он в отряде противника.            
        }        

        character.transform.position = pos;    // --------------------------------Изменяем позицию персонажа.
        charAi.startPos =pos;



        //------------------------------------------------------------------------ Сортируем персонажей в редакторе, для наглядности
        #if (UNITY_EDITOR)                                                                                                              //
                                                                                                                                        //
        character.transform.parent = (numOfPos < 4) ? GameObject.Find("Character_team_1").transform :                                   //
                                                       GameObject.Find("Character_team_2").transform;                                   //
        #endif                                                                                                                          //
        //--------------------------------------------------------------------------------------------------------------------------------

        charAi.PosIndex = numOfPos;
        skComponent.Initialize(true);  // ----------------------------------------Применяем изменения, сделанные для spine анимации
        charAi.BloodEffectInitialize();
        return charAi;
    }
}

