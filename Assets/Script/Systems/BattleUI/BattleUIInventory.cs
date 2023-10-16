using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BattleUIInventory : MonoBehaviour
{
    public Entity entity;
    public List<BattleUIItem> items = new List<BattleUIItem>();

    private void Start()
    {
        foreach(var item in items)
        {
            item.InitUIItem(this);
        }
    }
    public void InitInventory(Entity entity)
    {
        this.entity = entity;
    }
    public int GetIndexOfUIItem(BattleUIItem uiItem)
    {
        return items.IndexOf(uiItem);
    }
    public void ItemUIChange(int inventoryIndex, int itemIndex)
    {
        if(inventoryIndex == -1)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].GetItem() == -1)
                {
                    items[i].SetItem(itemIndex);
                    break;
                }
            }
        }
        else if(items.Count > inventoryIndex)
        {
            items[inventoryIndex].SetItem(itemIndex);
        }
    }
}
