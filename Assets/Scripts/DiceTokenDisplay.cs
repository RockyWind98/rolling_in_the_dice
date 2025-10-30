using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTokenDisplay : MonoBehaviour
{
    public Text diceTokenText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Roll()
    {
        int roll = Random.Range(1, 7);
        diceTokenText.text = roll.ToString();
    }
}
