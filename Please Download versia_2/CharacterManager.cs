
/*
 * The class responsible for creating characters in squads.
 * 
*/


using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Tooltip("Add character prefub.")]
    [SerializeField] private GameObject characterPrefub = null;

    private GameManager gmMngr; 

    //----------------------------------------------------------------------------- Start creating characters for all squads
    public void InitializingTeams(GameManager gm)
    {
        gmMngr = gm;

        for (int i = 0; i < 8; i++)                          
        {
            gmMngr.Character[i] = CreatCharacter(i);                             
        }
    }


    // ---------------------------------------------------------------------------  The method of create character, numOfPos - number of position character.
    private MinerAi CreatCharacter(int numOfPos)
    {
        GameObject character = Instantiate(characterPrefub) as GameObject;
        SkeletonMecanim skComponent = character.GetComponent<SkeletonMecanim>(); // The component required to change the spine animation.
        MinerAi charAi = character.GetComponent<MinerAi>();                                                                                   

        //------------------------------------------------------------------------------------------------------------------------------
        int skinIndex = Random.Range(0, 2);
        string skinName = (skinIndex < 1) ? "base" : "elite";                    // Random change skin for character.
        skComponent.initialSkinName = skinName;
        //------------------------------------------------------------------------------------------------------------------------------


        MeshRenderer charMesh = character.GetComponent<MeshRenderer>();
        charMesh.sortingOrder = numOfPos; //--------------------------------------- Sorting the characters by layers, for proper display.      

        Vector3 pos = Vector3.zero;

        switch (numOfPos) //------------------------------------------------------- Select character position.
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
            skComponent.initialFlipX = true;   //---------------------------------- Rotate character if he in enemy squad.            
        }        

        character.transform.position = pos;    // --------------------------------- Change character position.
        charAi.StartPosition = pos;



        //------------------------------------------------------------------------- Sorting the characters in the editor, for clarity
        #if (UNITY_EDITOR)                                                                                                              //
                                                                                                                                        //
        character.transform.parent = (numOfPos < 4) ? GameObject.Find("Character_team_1").transform :                                   //
                                                       GameObject.Find("Character_team_2").transform;                                   //
        #endif                                                                                                                          //
        //--------------------------------------------------------------------------------------------------------------------------------

        charAi.PosIndex = numOfPos;
        skComponent.Initialize(true);  // ----------------------------------------- Confirm settings for spine animation;
        charAi.BloodEffectInitialize((skinName == "elite") ? true: false); //------ Initializing blood effect and character parameters.
        return charAi;
    }
}

