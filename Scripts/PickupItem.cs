using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    bool alreadyPickup = false; //para nao duplicar item ao cair.

    public void OnTriggerStay2D(Collider2D collision) //ao player chegar prox. do item caido
    { 
        if(collision.transform.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.E) && !alreadyPickup) //ao press E vai acionar e se item nao foi pego:
            {
                Inventory.Singleton.PickupItem(item); //pegar o item
                alreadyPickup = true; //item ja foi pego

                Destroy(this.gameObject); //destroi o item caido da cena
            }
        }
    }
}
