

/*
 * ����� ���������� �� �������� ���������� � �������
 * 
 * ������� � ����������� ���������� � ������ ������.
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    /// <summary>
    /// �������� ������� ����������.
    /// </summary>
    [SerializeField] private GameObject characterPrefub = null; //----------------- ������ ���������

    private GameManager gmMngr; 

    // ��������� �������� �������� ���������� ��� ������� ������.
    public void InitializingTeams(GameManager gm)
    {
        gmMngr = gm;

        //-----------------------------------------------------------------------------------------------------------------------------
        for (int i = 0; i < 8; i++)                          
        {
            gmMngr.Character[i] = CreatCharacter(i);                             // ������� ���������� ��� ������� ������ � ������� � ���.
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }


    // ����� �������� ��������� numOfPos - ����� ������� ���������.
    private MinerAi CreatCharacter(int numOfPos)
    {
        GameObject character = Instantiate(characterPrefub) as GameObject;
        SkeletonMecanim skComponent = character.GetComponent<SkeletonMecanim>(); // ���������, ����������� ��� ��������� spine ��������.
        MinerAi charAi = character.GetComponent<MinerAi>();                                                                                  // 

        //------------------------------------------------------------------------------------------------------------------------------
        int skinIndex = Random.Range(0, 2);
        string skinName = (skinIndex < 1) ? "base" : "elite";                    // �������� ���� ��� ��������� ��������� �������.
        skComponent.initialSkinName = skinName;
        //------------------------------------------------------------------------------------------------------------------------------


        MeshRenderer charMesh = character.GetComponent<MeshRenderer>();
        charMesh.sortingOrder = (numOfPos < 4) ? numOfPos + 3 : numOfPos - 1 ; //-- ��������� ���������� �� �����, ��� ����������� �����������.        

        Vector3 pos = Vector3.zero;

        switch (numOfPos) //------------------------------------------------------- ��������� ������� ���������.
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
            skComponent.initialFlipX = true;   //------------------ ������������ ���������, ���� �� � ������ ����������.            
        }        

        character.transform.position = pos;    // --------------------------------�������� ������� ���������.
        charAi.startPos =pos;



        //------------------------------------------------------------------------ ��������� ���������� � ���������, ��� �����������
        #if (UNITY_EDITOR)                                                                                                              //
                                                                                                                                        //
        character.transform.parent = (numOfPos < 4) ? GameObject.Find("Character_team_1").transform :                                   //
                                                       GameObject.Find("Character_team_2").transform;                                   //
        #endif                                                                                                                          //
        //--------------------------------------------------------------------------------------------------------------------------------

        charAi.PosIndex = numOfPos;
        skComponent.Initialize(true);  // ----------------------------------------��������� ���������, ��������� ��� spine ��������
        charAi.BloodEffectInitialize();
        return charAi;
    }
}

