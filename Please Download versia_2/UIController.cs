

/*
 * The class responsible for UI.
 * Changes the round number indicator.
 * Hides and shows the shadow of the battle and the attack and skip move buttons
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("       Add this elements from hierarchy")]

    [SerializeField] private Animator camera_Anim = null;
 
    [SerializeField] private Image playerImage = null;
    [SerializeField] private GameObject playerAtckIcon = null;
    [SerializeField] private GameObject playerHealthIcon = null;

    [SerializeField] private TextMeshProUGUI playerName_txt = null;
    [SerializeField] private TextMeshProUGUI playerAttack_txt = null;
    [SerializeField] private TextMeshProUGUI playerHealth_txt = null;



    [SerializeField] private Image enemyImage = null;
    [SerializeField] private GameObject enemyAtckIcon = null;
    [SerializeField] private GameObject enemyHealthIcon = null;

    [SerializeField] private TextMeshProUGUI enemyName_txt = null;
    [SerializeField] private TextMeshProUGUI enemyAttack_txt = null;
    [SerializeField] private TextMeshProUGUI enemyHealth_txt = null;


    [SerializeField] private TextMeshProUGUI roundText = null;
    [SerializeField] private SpriteRenderer Shadow = null;
    [SerializeField] private GameObject[] UiButtons = null;




    private void Awake()
    {
        GameManager.Camera_Damage_Effect += this.CameraDamageEffect;
    }

    private void OnDestroy()
    {
        GameManager.Camera_Damage_Effect -= this.CameraDamageEffect;
    }


    //=============================================================================================

    //                              Character UI settings

    //=============================================================================================

    public void EditCharacterUI(bool player, Sprite icon, Dictionary<string,int> charParam) 
    {
        if (player)
        {
            playerImage.sprite = icon;
            playerImage.gameObject.SetActive(true);
            playerAtckIcon.SetActive(true);
            playerHealthIcon.SetActive(true);
            playerName_txt.text = (charParam["Name"] < 1) ? "Miner" : "Elite Miner";
            playerAttack_txt.text = charParam["Attack_Min"] + "-" + charParam["Attack_Max"];
            playerHealth_txt.text = charParam["Health"] + "/" + charParam["Health_Max"];
        }
        else
        {
            enemyImage.sprite = icon;
            enemyImage.gameObject.SetActive(true);
            enemyAtckIcon.SetActive(true);
            enemyHealthIcon.SetActive(true);
            enemyName_txt.text = (charParam["Name"] < 1) ? "Miner" : "Elite Miner";
            enemyAttack_txt.text = charParam["Attack_Min"] + "-" + charParam["Attack_Max"];
            enemyHealth_txt.text = charParam["Health"] + "/" + charParam["Health_Max"];
        }        
    }

    public void HideEnemyCharacterUI()
    {
        enemyImage.gameObject.SetActive(false);
        enemyAtckIcon.SetActive(false);
        enemyHealthIcon.SetActive(false);
        enemyImage.sprite = null;
        enemyName_txt.text = "";
        enemyAttack_txt.text = "";
        enemyHealth_txt.text = "";
    }

    public void HideCharUI() 
    {        
        playerImage.gameObject.SetActive(false);
        playerAtckIcon.SetActive(false);
        playerHealthIcon.SetActive(false);
        playerImage.sprite = null;        

        playerName_txt.text = "";
        playerAttack_txt.text = "";
        playerHealth_txt.text = "";



        enemyImage.gameObject.SetActive(false);
        enemyAtckIcon.SetActive(false);
        enemyHealthIcon.SetActive(false);
        enemyImage.sprite = null;

        enemyName_txt.text = "";
        enemyAttack_txt.text = "";
        enemyHealth_txt.text = "";
    }


    //=============================================================================================

    //                                  Button settings

    //=============================================================================================


    public void AttackButton() // ------------------------------------------- The method for the attack button, enable indicator of the start of the move,
    {                          //                                             and hide skip moove button.
        GameManager.turnStart = true;
        UiButtons[0].SetActive(false);
        UiButtons[1].SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void HideButtons(bool on) // ------------------------------------- The method for hide attack and skip moove buttons.
    {
        foreach (GameObject b in UiButtons) b.SetActive(on);
    }

    //=============================================================================================

    //                                  Another UI Settings

    //=============================================================================================

    private void CameraDamageEffect()
    {
        camera_Anim.Play("Camera_Damage");
    }

    public void SetRoundCounter(int value)
    {
        roundText.text = "" + value;
    }

    public IEnumerator ShowShadow(bool on) // ------------------------------- The coroutine of starting and disabling the shadow of battle.
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
