

/*
 * The class responsible for character health bar.
*/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Tooltip("Add red health bar object from hierarchy.")]
    [SerializeField] private Transform healthRed = null;

    [Tooltip("Add yellow health bar object from hierarchy.")]
    [SerializeField] private Transform healthYellow = null;

    [Tooltip("Add health bar light object from hierarchy.")]
    [SerializeField] private GameObject barLight = null; // -------------- Move of character indicator.

    [Tooltip("Add all health bar element from hierarchy (background must have index - 0).")]
    [SerializeField] private SpriteRenderer[] healthBarelements = null;

    
    public void EditLayerOrder(int value) // ----------------------------- 
    {
        for (int i = 0; i < healthBarelements.Length; i++)
        {
            healthBarelements[i].sortingOrder = i + value;
        }
    }

    public void ShowLight(bool on) //------------------------------------- Show move of character indicator.
    {
        barLight.SetActive(on);
    }

    public IEnumerator Damage(float value) // ---------------------------- Transforming scale for health bar sprites.
    {  
        Vector3 hlth = healthRed.localScale;
        hlth.x -= value;
        if (hlth.x < 0) hlth.x = 0;

        
        while (healthRed.localScale.x > 0 && healthRed.localScale.x > hlth.x)
        {
            healthRed.localScale = Vector3.MoveTowards(healthRed.localScale, hlth, 2 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (healthYellow.localScale.x > 0 && healthYellow.localScale.x > hlth.x)
        {
            healthYellow.localScale = Vector3.MoveTowards(healthYellow.localScale, hlth, 1 * Time.deltaTime);
            yield return null;
        }
    }
}
