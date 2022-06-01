

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

    [SerializeField] private TextMeshProUGUI roundText = null;

    [SerializeField] private Image playerImage = null;
    [SerializeField] private Image enemyImage = null;

    [SerializeField] private TextMeshProUGUI playerAttack_txt = null;
    [SerializeField] private TextMeshProUGUI playerHealth_txt = null;
    [SerializeField] private TextMeshProUGUI enemyAttack_txt = null;
    [SerializeField] private TextMeshProUGUI enemyHealth_txt = null;

    [SerializeField] private SpriteRenderer Shadow = null;
    [SerializeField] private GameObject[] UiButtons = null;
    

    //=============================================================================================

    //                              Character UI settings

    //=============================================================================================

    public void EditCharacterUI(bool player, Sprite icon, Dictionary<string,int> charParam) 
    {
        if (player)
        {
            playerImage.sprite = icon;
            playerImage.gameObject.SetActive(true);
            playerAttack_txt.text = charParam["Attack_Min"] + "-" + charParam["Attack_Max"];
            playerHealth_txt.text = charParam["Health"] + "/" + charParam["Health_Max"];
        }
        else
        {
            enemyImage.sprite = icon;
            enemyImage.gameObject.SetActive(true);
            enemyAttack_txt.text = charParam["Attack_Min"] + "-" + charParam["Attack_Max"];
            enemyHealth_txt.text = charParam["Health"] + "/" + charParam["Health_Max"];
        }        
    }

    public void HideEnemyCharacterUI(Sprite icon, Dictionary<string, int> charParam)
    {
        enemyImage.gameObject.SetActive(false);
        enemyImage.sprite = null;
        enemyAttack_txt.text = "";
        enemyHealth_txt.text = "";
    }

    public void HideCharUI() 
    {
        enemyImage.gameObject.SetActive(false);
        playerImage.gameObject.SetActive(false);
        playerImage.sprite = null;
        enemyImage.sprite = null;

        playerAttack_txt.text = "";
        playerHealth_txt.text = "";

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
