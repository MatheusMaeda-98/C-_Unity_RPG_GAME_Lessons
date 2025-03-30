using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler //possibilita clicar na interface
{   
    Image itemIcon;
    public CanvasGroup canvasGroup {get; private set; } //para o inventário conseguir mostrar img do item.
    public Item myItem {get; set; }
    public InventorySlot activeSlot {get; set; } 
    void Awake() // é o comando que inicia antes de void start
    {
        canvasGroup = GetComponent<CanvasGroup>(); // o script sempre vai chamar os componentes do canvas.
        itemIcon = GetComponent<Image>();
    }

    public void Initialize(Item item, InventorySlot parent) // item vai se anexar a um slot
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;
    }


    public void OnPointerClick(PointerEventData eventData) // requisito para o ipointerclick
    {
        if(eventData.button == PointerEventData.InputButton.Left) // ao clicar botao esquerd do mouse inicia essa função;
        {
            Inventory.Singleton.SetCarriedItem(this);     //quero usar o set carrieditem neste scpript           
        }
        else if(eventData.button == PointerEventData.InputButton.Right) //ao clickar c/ btn direito
        {
            Inventory.Singleton.DropItem(this); //inicia a função drop item

        } 
    }
}
