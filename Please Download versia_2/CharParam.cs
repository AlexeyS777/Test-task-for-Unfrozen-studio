

/*
 * The class responsible for character params.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharParam : MonoBehaviour
{
    private Dictionary<string, int> myParam = new Dictionary<string, int>();

    public Dictionary<string,int> MyParameters { get { return myParam; } }

    public void Inicializing(bool elite)
    {
        int maxHealth = (elite) ? Random.Range(55, 80) : Random.Range(30, 54);

        myParam.Add("Health_Max", maxHealth);
        myParam.Add("Health", myParam["Health_Max"]);

        myParam.Add("Attack_Min", myParam["Health_Max"] / 5 - 3);
        myParam.Add("Attack_Max", myParam["Health_Max"] / 5 + 3);
    }

    
}
