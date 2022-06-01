


/*
 * The class responsible for damage moving text.
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro myText;
    [SerializeField] private Transform myPos;

    
    public IEnumerator Move(int value)
    {
        myText.text = "" + value;

        Vector3 pos = myPos.transform.position;
        pos.y += 4;
        myPos.position = pos;
        pos.y += 5;

        Color c = myText.color;
        c.a = 0;

        while(myText.color.a > 0)
        {
            myText.color = Vector4.MoveTowards(myText.color, c, 1 * Time.deltaTime);
            myPos.position = Vector3.MoveTowards(myPos.position, pos, 2 * Time.deltaTime);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
