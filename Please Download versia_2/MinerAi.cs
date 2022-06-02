

/*
 * The class responsible for the behavior of the characters in the game.
 * Starts animations and registers interaction with the cursor.
 * Transmitting information about character.
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;




public class MinerAi : MonoBehaviour
{
    [Tooltip("Add player choise effect.")] 
    [SerializeField] private GameObject playerChoiceEffect = null;
    
    [Tooltip("Add enemy choise effect.")]
    [SerializeField] private GameObject enemyChoiceEffect = null;
    
    [Tooltip("Add blood effect object.")] 
    [SerializeField] private Animator blood = null;
    
    [Tooltip("Add health bar object.")] 
    [SerializeField] private HealthBar myHealthBar = null;    

    [Tooltip("Add damage text prefub.")]
    [SerializeField] private DamageText myDamage_txt = null;

    [Tooltip("Add icon sprite.")]
    [SerializeField] private Sprite[] charIcons = null;    

    private Sprite myIcon;
    private MeshRenderer myMesh;
    private Transform myPos;
    private Animator anim;
    private GameManager gmMngr;
    private UIController uiCntrl;
    private AudioSource myVoice;
    private CharParam myParam;
    private SkeletonMecanim mySkComponent;

    private Vector3 startPos; // ----------------------------------------------------------- Character start position (for motion)
    private int posIndex; // --------------------------------------------------------------- Index of character position.
    public bool ready = true; //------------------------------------------------------------ Character readinees for action.

    public MeshRenderer MyMesh { get { return myMesh; } }
    public int PosIndex { get { return posIndex; } set { posIndex = value; } } // ---------- Edit property for index of character position.
    public Sprite MyIcon { get { return myIcon; } }
    public Vector3 StartPosition { get { return startPos; } set { startPos = value; } }


    //=====================================================================================================================================

    //                                                  Inicializing settings

    //=====================================================================================================================================

    void Awake()
    {

        myVoice = GetComponent<AudioSource>();
        myMesh = GetComponent<MeshRenderer>();
        mySkComponent = GetComponent<SkeletonMecanim>();
        gmMngr = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiCntrl = GameObject.Find("GameManager").GetComponent<UIController>();
        myPos = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    public void BloodEffectInitialize(bool elite)
    {
        blood.GetComponent<MeshRenderer>().sortingOrder = this.GetComponent<MeshRenderer>().sortingOrder + 1;
        blood.GetComponent<SkeletonMecanim>().initialFlipX = this.GetComponent<SkeletonMecanim>().initialFlipX;
        blood.GetComponent<SkeletonMecanim>().Initialize(true);
        myParam = GetComponent<CharParam>();
        myParam.Inicializing(elite);
        myIcon = (elite) ? charIcons[0] : charIcons[1];
    }

    //=======================================================================================================================================

    //                                                       Cursor settings

    //=======================================================================================================================================

    // Enable enemy choice effect, when cursor on character collider.
    private void OnMouseEnter()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart)
        {
            enemyChoiceEffect.SetActive(true);
            uiCntrl.EditCharacterUI(false, myIcon,myParam.MyParameters); // ------------------ Show enemy parametrs
        }
    }


    // Disable enemy choice effect, when cursor out of character collider.
    private void OnMouseExit()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart)
        {
            enemyChoiceEffect.SetActive(false);
            uiCntrl.HideEnemyCharacterUI();
        }
    }


    // Choice enemy whith cursor.
    private void OnMouseDown()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart)
        {
            gmMngr.DefendingChar = this; // ------------------------------------------- Transmit data about the choice of the defending character
            StartCoroutine(gmMngr.StartBattle()); //----------------------------------- Attack start. 
        }        
    }

    // -------------------------------------------------------------------------------- Blood effect setting.
    

    

    //=============================================================================================================================

    //                                              Attack settings

    //=============================================================================================================================

    public void RandomAttack() // ---------------------------------------------------- Random selection of the type of attack.
    {
        ready = false;

        int variantOfAttack;
        string attackName = "Miner_1";

        variantOfAttack = Random.Range(0, 3);

        switch (variantOfAttack)
        {
            case 0:
                attackName = "Miner_1";
                break;
            case 1:
                attackName = "DoubleShift";
                break;
            case 2:
                attackName = "PickaxeCharge";
                break;
            default:
                attackName = "DoubleShift";
                break;

        }

        anim.Play(attackName);
    }

    public void Hit() // ----------------------------------------------------------------- Method - label in attack animation, enabling damage animation for enemy character.
    {
        myVoice.Play();
        gmMngr.DefendingChar.Damage();
    }

    public void EndAttack() // ----------------------------------------------------------- Method - label in attack animation
    {
        gmMngr.DefendingChar.HideBlood();

        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   -   -   -    Reset the animator to fix a bug with restarting the animation.
        anim.enabled = true;
        ready = true;

    }

    public void Damage() // -------------------------------------------------------------- Get damage, enable damage animation.
    {
        ready = false;
        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   -   -   -    Reset the animator to fix a bug with restarting the animation.
        anim.enabled = true;
        this.anim.Play("Damage");
        int dmg = Random.Range(myParam.MyParameters["Attack_Min"],myParam.MyParameters["Attack_Max"]);
        float barDmg = 1f / myParam.MyParameters["Health_Max"] * (float)dmg;

        DamageText dmgTxt = Instantiate(myDamage_txt, myPos.position,transform.rotation);
        StartCoroutine(dmgTxt.Move(dmg));

        StartCoroutine(this.myHealthBar.Damage(barDmg));
        myParam.MyParameters["Health"] -= dmg;
        if(myParam.MyParameters["Health"] < 0) myParam.MyParameters["Health"] = 0;

        if(!mySkComponent.initialFlipX)
        {
            uiCntrl.EditCharacterUI(true, myIcon, myParam.MyParameters);
        }
        else
        {
            uiCntrl.EditCharacterUI(false,myIcon, myParam.MyParameters);
        }

        this.blood.gameObject.SetActive(true);
        this.blood.GetComponent<MeshRenderer>().sortingOrder = myMesh.sortingOrder + 1; // Setting the blood Sorting Layer Order is greater than that of the character
                                                                                        // for correct display.
        this.blood.Play("Damage");
    }

    //==================================================================================================================================================

    //                                                              Effects settings

    //==================================================================================================================================================

    public GameObject ChoiceEffect() // -------------------------------------------------- Enable / Disable player choice effect from enother class. 
    {                              
        return playerChoiceEffect;
    }
    
    

    public void HideBlood()// ------------------------------------------------------------ Hide blood effect.
    {
        blood.gameObject.SetActive(false);
    }

    public void EditLayerOrder(int value)
    {
        myMesh.sortingOrder = value;
        myHealthBar.EditLayerOrder(myMesh.sortingOrder);
    }

    public void ShowMoveIndicator(bool on)
    {
        myHealthBar.ShowLight(on);
    }

    public Dictionary<string, int> MyParameters()
    {
        return myParam.MyParameters;
    }

    //==================================================================================================================================================

    //                                                              Another settings

    //==================================================================================================================================================

    public void Death()
    {
        gmMngr.Character[posIndex] = null;
        Destroy(this.gameObject);
    }

    public IEnumerator Move(float pos) // ------------------------------------------------ Coroutine responsible for character moving.
    {        
        this.ready = false;
        if (playerChoiceEffect.activeSelf) playerChoiceEffect.SetActive(false); //         Disable choice effects.
        if (enemyChoiceEffect.activeSelf) enemyChoiceEffect.SetActive(false); 

        if (PosIndex != 0 && PosIndex != 4) //-   -   -   -   -   -   -   -   -   -   -    The closest characters of the player and the opponent 
        { //                                                                               should not move due to the short distance.
            Vector3 finpos = myPos.position;
            finpos.x = pos;

            anim.Play("Pull"); // -  -   -   -   -   -   -   -   -   -   -   -   -   -     Enable moving animation.

            while (myPos.localPosition.x != pos)
            {
                myPos.position = Vector3.MoveTowards(myPos.position, finpos, 4 * Time.deltaTime);
                yield return null;
            }            
        }
        
        this.ready = true;
    }
}
