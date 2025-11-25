using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinView : MonoBehaviour
{
    [SerializeField] private TMP_Text txt;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetCoinValue(PlayerDataManager.Instance.GetPlayerCoins());
    }

    public void SetCoinValue(int value)
    {
        if (txt == null)
        {
            Debug.LogError("CoinView: txt is null.");
            return;
        }
        txt.text = value.ToString();
    }
}
