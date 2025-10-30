using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceActCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickDiceAct()
    {
        GameObject[] prefabInstances = GameObject.FindGameObjectsWithTag("DiceToken");

        if (prefabInstances.Length <= 0 )
        {
            Debug.Log("No DiceToken prefab instances found in the scene.");
        }
        else
        {
            foreach (GameObject prefabInstance in prefabInstances)
            {
                DiceTokenDisplay diceTokenDisplay = prefabInstance.GetComponent<DiceTokenDisplay>();
                if (diceTokenDisplay != null)
                {
                    diceTokenDisplay.Roll();
                }
                else
                {
                    Debug.LogWarning("DiceTokenDisplay component not found on prefab instance.");
                }
            }
        }
    }
}
