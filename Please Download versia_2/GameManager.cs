

/*
 * The class responsible for game management (the entry point to the program).
 * Starts and finishes the game round, decides who goes first
 * Defines the actions of the characters
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public delegate void GUIEffects(); // ----------------------------------------------- How much benefit I got working on this assignment! I remembered the delegates! ^_^ 
    public static event GUIEffects Camera_Damage_Effect; //                               True, I didn't use them in the best way here...

    [SerializeField] private AudioSource iratusVoice = null;
    [SerializeField] private AudioClip[] voice = null;

    private CharacterManager charMngr;
    private UIController uiCntrl;
    private MinerAi[] characters = new MinerAi[8]; // ----------------------------------- Array for all characters in all squads.  
    private MinerAi atkChar; // --------------------------------------------------------- Variables for defining attacking and defending characters.
    private MinerAi defChar;

    private int[] charTurn = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }; // ------------------- Array for defining the order of the characters moves.
    private int indexsOfTurn = 0;// ----------------------------------------------------- Indicator for array of character moves.

    public static bool playerTurn = false; // ------------------------------------------- Indicators for configuring the display of selection effects.
    public static bool turnStart = false;

    public static int roundCounter = 0; // ---------------------------------------------- Round number.

    public MinerAi DefendingChar { get { return defChar; } set { defChar = value; } } //  Property for configuring the defender character.

    public MinerAi[] Character { get { return characters; } } // ------------------------ Property for filling in an array of characters.



    //=====================================================================================================================================

    //                                                  Inicializing settings

    //=====================================================================================================================================


    private void Awake()
    {
        uiCntrl = GetComponent<UIController>();
        charMngr = GetComponent<CharacterManager>();
        charMngr.InitializingTeams(this);
    }
    private void Start()
    {
        roundCounter = 0;
        nextRound();
    }


    //=====================================================================================================================================

    //                                                  Round and move settings

    //=====================================================================================================================================



    public void NextMove() // ----------------------------------------------------------- Next character move;  
    {
        if(atkChar != null)
        {
            atkChar.ChoiceEffect().SetActive(false); // -   -   -   -   -   -   -   -     Disabling the  player choice effect to fix a bug with this effect.
            atkChar.ShowMoveIndicator(false);
            atkChar = null;
        }

        indexsOfTurn++;

        //-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   Check characters in all sqads.
        bool enemyAquad = false;
        bool playerSquad = false;

        for(int i = 0; i < characters.Length; i++)
        {
            if (i < 4 && characters[i] != null)
            {
                playerSquad = true;
            }
            else if (i > 3 && characters[i] != null)
            {
                enemyAquad = true;
            } 
            
        }

        if(enemyAquad && playerSquad)
        {
            if (indexsOfTurn == 8)
            {
                nextRound();
            }
            else
            {
                ChoiseCharacter();
            }
        }
        else
        {
            SceneManager.LoadScene("Test Battle");
        }
        
    }

    private void nextRound() // -------------------------------------------------------- Start new round
    {  
        foreach(MinerAi ch in characters)
        {
            if(ch != null)ch.ShowMoveIndicator(true);
        }

        roundCounter++;
        uiCntrl.SetRoundCounter(roundCounter);
        CharRandomTurns(); 
        ChoiseCharacter();        
    }

    private void CharRandomTurns() // -------------------------------------------------- Determining the random order of the characters' moves.
    {
        int r;

        for (int i = 0; i < charTurn.Length; i++)
        {
            int index = Random.Range(i, charTurn.Length);
            r = charTurn[index];

            charTurn[index] = charTurn[i];
            charTurn[i] = r;
        }

        indexsOfTurn = 0;
    }


    //=====================================================================================================================================

    //                                                  Characters selection settings

    //=====================================================================================================================================


    public void ChoiseCharacter() // --------------------------------------------------- Definition of attacking and defending characters.
    {        
        atkChar = characters[charTurn[indexsOfTurn]];

        if(atkChar == null)
        {
            NextMove();
        }
        else
        {
            if (atkChar.PosIndex < 4) // -   -   -   -   -   -   -   -   -   -   -   -   -   Player's move.
            {
                playerTurn = true;
                atkChar.ChoiceEffect().SetActive(true);
                uiCntrl.HideButtons(true);
                uiCntrl.EditCharacterUI(true,atkChar.MyIcon, atkChar.MyParameters());
            }
            else // -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -    Enemy's move.
            {
                playerTurn = false;

                while (defChar == null)
                {
                    defChar = characters[Random.Range(0, 4)]; // -   -    The enemy chooses who to attack
                }

                uiCntrl.EditCharacterUI(false,atkChar.MyIcon, atkChar.MyParameters());
                uiCntrl.EditCharacterUI(true,defChar.MyIcon, defChar.MyParameters());
                StartCoroutine(StartBattle());
            }
        }
    }



    //=====================================================================================================================================

    //                                                  Battle settings

    //=====================================================================================================================================

    public IEnumerator StartBattle() // ---------------------------------------- The coroutine responsible for the course of the game
    {
        turnStart = false;
        uiCntrl.HideButtons(false);

        float atcDistance = 0; // -   -   -   -   -   -   -   -   -   -   -   -  Determining the direction of movement of the characters
        float defDistance = 0;
        int aCharSortOrder = atkChar.MyMesh.sortingOrder;
        int defCharSortOrder = defChar.MyMesh.sortingOrder;

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

        atkChar.EditLayerOrder(16); // -   -   -   -   -   -   -   -   -   -   - Setting up the sorting layer order for the correct display of characters
        defChar.EditLayerOrder(15);
        StartCoroutine(uiCntrl.ShowShadow(true));// -   -   -   -   -   -   -    Display shadow of battle.
        yield return new WaitForSeconds(1f);

        StartCoroutine(atkChar.Move(atcDistance)); // -   -   -   -   -   -   -  Moving to the battle position.
        StartCoroutine(defChar.Move(defDistance));

        while (!atkChar.ready && !defChar.ready) // -   -   -   -   -   -   -    Checking the readiness of the characters to attack
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        int i = Random.Range(0, 2);
        if(!iratusVoice.isPlaying && atkChar.PosIndex < 4 && i > 0) iratusVoice.PlayOneShot(voice[Random.Range(0, 7)]);

        if (Camera_Damage_Effect != null) Camera_Damage_Effect();
        
        atkChar.RandomAttack(); // -   -   -   -   -   -   -   -   -   -   -     Attack

        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        StartCoroutine(uiCntrl.ShowShadow(false)); // -   -   -   -   -   -      Hide shadow of battle, blood effect, return to the start position.
        defChar.HideBlood();
        StartCoroutine(atkChar.Move(atkChar.StartPosition.x));
        StartCoroutine(defChar.Move(defChar.StartPosition.x));
        
        while (!atkChar.ready && !defChar.ready)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(2f); 
        atkChar.EditLayerOrder(aCharSortOrder); // -   -   -   -   -   -   -     Restore sorting layer order for characters currect display.
        defChar.EditLayerOrder(defCharSortOrder);

        uiCntrl.HideCharUI();

        if (defChar.MyParameters()["Health"] < 1)
        {
            if (defChar.PosIndex < 4 && !iratusVoice.isPlaying) //-   -   -   -   -   -   -   -   -   -   Not work, because Iratus voice files is very quiet
            {
                iratusVoice.PlayOneShot(voice[Random.Range(13, voice.Length)]);
            }
            else if(!iratusVoice.isPlaying)
            {
                iratusVoice.PlayOneShot(voice[Random.Range(7, 13)]);
            }

            defChar.Death();
        }

        defChar = null;        

        NextMove();        
    }
}
