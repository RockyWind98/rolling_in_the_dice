using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemShowSystem : Singleton<ItemShowSystem>
{
    public TMP_Text itemName;
    public TMP_Text itemDes;
    public GameObject showObj;

    // Start is called before the first frame update
    void Start()
    {
        showObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowItemInfo(string iName, string iDes, bool isUp, Vector3 screenPosition, float offset)
    {
        // 设置显示的文本内容
        itemName.text = iName;
        itemDes.text = iDes;

        // 计算显示位置
        showObj.transform.position = isUp? new Vector3(screenPosition.x, screenPosition.y + offset, showObj.transform.position.z) :
            new Vector3(screenPosition.x, screenPosition.y - offset, showObj.transform.position.z);

        // 激活显示对象
        showObj.SetActive(true);
    }
}
