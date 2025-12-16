using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/GameItemDataBase")]
public class GameItemDataBase : ScriptableObject
{
    public List<GameItem> allItems;
    private Dictionary<string, GameItem> lookUpTable;

    public void Init()
    {
        lookUpTable = new Dictionary<string, GameItem>();
        foreach (var item in allItems)
        {
            if (item != null && !lookUpTable.ContainsKey(item.id))
            {
                lookUpTable.Add(item.id, item);
            }
            else
            {
                Debug.LogWarning($"重复的ID或空物品: {item?.id}");
            }
        }
    }

    public GameItem GetItemById(string id)
    {
        if (lookUpTable == null) Init();

        if (lookUpTable.TryGetValue(id, out GameItem item))
        {
            return item;
        }

        Debug.LogError($"找不到ID为 {id} 的物品！");
        return null;
    }

    [ContextMenu("自动加载所有物品")]
    public void LoadAllItems()
    {
        allItems = new List<GameItem>();

        // 1. 查找所有类型为 GameItem 的资源 GUID
        string[] guids = AssetDatabase.FindAssets("t:GameItem");

        foreach (string guid in guids)
        {
            // 2. 将 GUID 转换为实际的路径
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // 3. 加载资源
            GameItem item = AssetDatabase.LoadAssetAtPath<GameItem>(path);

            if (item != null)
            {
                allItems.Add(item);
            }
        }
        Debug.Log($"数据库更新完毕，共加载 {allItems.Count} 个物品。");
    }
}
