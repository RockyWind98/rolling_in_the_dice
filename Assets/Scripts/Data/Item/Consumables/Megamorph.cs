using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Consumables/Megamorph")]
public class Megamorph : ConsumableItem
{
    public override bool Use(GameObject targetObj)
    {
        if(targetObj == null)
        {
            Debug.LogError("targetObj is null!");
            return false;
        }

        var dv = targetObj.GetComponent<DiceView>();
        if (dv != null)
        {
            var dice = dv.dice;
            if (dice is CommonDice)
            {
                dice.num = 6;
                return true;
            }
        }

        NoticeSystem.Instance.ShowNotice("只能对普通骰子使用该道具！");
        return false;
    }
}
