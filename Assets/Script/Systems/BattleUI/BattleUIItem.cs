using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUIItem : MonoBehaviour
    , IDragHandler
    , IPointerDownHandler
    , IPointerUpHandler
{
    private BattleUIInventory inventory;

    public Image itemShadowIcon;
    public Image itemIcon;

    private int itemIndex = -1;

    private bool isDragging = false;

    private void Start()
    {
        gameObject.SetActive(false);

        if (inventory != null)
            gameObject.SetActive(true);
    }
    public void InitUIItem(BattleUIInventory inventory)
    {
        this.inventory = inventory;
        gameObject.SetActive(true);
    }
    public bool IsSuitable(int setItemIndex)
    {
        // 적합한 아이템이 들어가는지 확인.
        return true;
    }
    public bool IsEmpty()
    {
        return itemIndex == -1;
    }
    public void SetItem(int setItemIndex)
    {
        if (setItemIndex == -1)
        {
            itemShadowIcon.gameObject.SetActive(false);
            itemIcon.gameObject.SetActive(false);

            itemIndex = setItemIndex;
            return;
        }

        itemShadowIcon.gameObject.SetActive(true);

        itemIcon.transform.position = transform.position;
        itemIcon.gameObject.SetActive(true);

        itemIcon.overrideSprite = ResourceManager.Instance.GetItemImage(setItemIndex);

        itemIndex = setItemIndex;
    }
    public int GetItem()
    {
        return itemIndex;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemIndex == -1)
            return;

        isDragging = true;

        itemIcon.transform.SetParent(BattleUIManager.Instance.transform, false);
        itemIcon.transform.position = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (itemIndex == -1)
            return;

        if (isDragging)
        {
            itemIcon.transform.position = eventData.position;

            if (ResourceManager.Instance.IsBuildingType(itemIndex))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    itemIcon.gameObject.SetActive(false);
                    BattleUIManager.Instance.DraggingBuildingItem(eventData.position, itemIndex);
                }
                else
                {
                    itemIcon.gameObject.SetActive(true);
                    BattleUIManager.Instance.EndDragBuildingItem();
                }
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (itemIndex == -1)
            return;

        if (isDragging)
        {
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);

            foreach (RaycastResult ray in hits)
            {
                BattleUIItem overItem = null;
                if (ray.gameObject.TryGetComponent(out overItem))
                {
                    if (!overItem.IsSuitable(itemIndex)) // 아이템이 들어갈 수 있는지 확인.
                    {
                        itemIcon.transform.position = transform.position;
                        break;
                    }

                    if (overItem.IsEmpty())
                    {
                        // 해당 아이템 슬롯이 비어있을 경우.
                        BattleUIManager.Instance.MoveItemByUI(inventory.entity, inventory.GetIndexOfUIItem(this), overItem.inventory.entity, overItem.inventory.GetIndexOfUIItem(overItem), itemIndex);
                    }
                }
            }

            BattleUIManager.Instance.EndDragBuildingItem();

            itemIcon.gameObject.SetActive(true);
            itemIcon.transform.SetParent(transform, false);
            itemIcon.transform.position = transform.position;

            isDragging = false;
        }
    }
}
