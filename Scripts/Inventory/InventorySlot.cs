using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;



public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem {  get; set; }
    public SlotTag myTag;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) //classif. o item como carried item ou nao
        {
            if(Inventory.carriedItem == null) //verif. se item ja tem no inventario
            {
                return;
            }
        }

        if(myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag) //verif se tem uma tag igual ao que ja temos no inventario.
        {
            return;
        }

    SetItem(Inventory.carriedItem);   
    }
    
    public void SetItem(InventoryItem item)
    {
        Inventory.carriedItem = null;

        item.activeSlot.myItem = null;

        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true; //bloqueia a interação para nao dar probleema

        if(myTag != SlotTag.None) // tudo que tem tag none ele vai pro inventário, se tag equipment ele vai poder equipar.
        {
            Inventory.Singleton.EquipEquipment(myTag, myItem);
        }
    }
}
