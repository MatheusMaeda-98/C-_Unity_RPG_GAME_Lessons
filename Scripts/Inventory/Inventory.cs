using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] equipmentSlots; 

    [SerializeField] Transform draggablesTransform; //estara armazenando os valores das posições dos objs do inventario.
    [SerializeField] InventoryItem itemPrefab;

    [SerializeField] Item[] items;
    [SerializeField] Button giveItemBtn; //o item ao cair de um monstro vira um botar para pegar //teste

    public Player player;

    private Item helmetTempItem;
    private Item chestTempItem;
    private Item legsTempItem;
    private Item feetTempItem;

    


    private void Awake()
    {   
        player = GameObject.Find("Player").GetComponent<Player>();
        Singleton = this;
        giveItemBtn.onClick.AddListener(delegate {SpawnInventoryItem(); } );

    }
    public void Update()
    {
        if(carriedItem == null)
        {
            return;
        }

        carriedItem.transform.position = Input.mousePosition; //vai obter a posição do item pelo cursor do mouse.
    }	

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null) //vai classificar o tipo de item obtido.
        {
            if (item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) //verifica se o item ja tem no inventario
            {
                return;
            }
            item.activeSlot.SetItem(carriedItem); //se falso, ele cria um novo slot para o novo item
        }

        if (item.activeSlot.myTag != SlotTag.None)
        {
            EquipEquipment(item.activeSlot.myTag,null); //se tag diferente de nulo, o item passara a ser equipado
        }

        carriedItem = item; 
        carriedItem.canvasGroup.blocksRaycasts = false; //atrib. do canvas para nao dar problema no click
        item.transform.SetParent(draggablesTransform); //set o item, o valor será transferido para o dragg transform.
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null) //metodo para equipar item
    {
        switch (tag)
        {
            case SlotTag.Head:
                if (item == null)
                {   
                    player.entity.strength -= helmetTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence -= helmetTempItem.res;
                    helmetTempItem = null;
                    Debug.Log("Removeu um item da tag Head"); //poderia aqui colocar buffs do equipamento
                }
                else
                {   
                    helmetTempItem = item.myItem;
                    player.entity.strength += helmetTempItem.str; //ao equipar, coloca o buff do equip
                    player.entity.resistence += helmetTempItem.res;
                    Debug.Log("Equipou um item da tag head");
                }
                break; // sai do case

            case SlotTag.Chest:
                if (item == null)
                {   
                    player.entity.strength -= chestTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence -= chestTempItem.res;
                    chestTempItem = null;
                    Debug.Log("Removeu um item da tag chest");
                }
                else
                {   chestTempItem = item.myItem;
                    player.entity.strength += chestTempItem.str; //ao equipar, coloca o buff do equip
                    player.entity.resistence += chestTempItem.res;
                    
                    Debug.Log("Equipou um item da tag chest");
                }
                break; // sai do case

            case SlotTag.Legs:
                if (item == null)
                {   player.entity.strength -= legsTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence -= legsTempItem.res;
                    legsTempItem = null;
                    Debug.Log("Removeu um item da tag Legs");
                }
                else
                {   legsTempItem = item.myItem;
                    player.entity.strength += legsTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence += legsTempItem.res;
                    Debug.Log("Equipou um item da tag Legs");
                }
                break; // sai do case

            case SlotTag.Feet:
                if (item == null)
                {   player.entity.strength -= feetTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence -= feetTempItem.res;   
                    feetTempItem = null;
                    Debug.Log("Removeu um item da tag Feet");
                }
                else
                {   feetTempItem = item.myItem;
                    player.entity.strength += feetTempItem.str; //ao desequipar, tira o buff do equip
                    player.entity.resistence += feetTempItem.res;
                    Debug.Log("Equipou um item da tag Feet");
                }
                break; // sai do case
        }
    }
    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item; 
        //cria um item temporario para teste
        if(_item == null) //verif se item é vazio
        {
            _item = PickRandomItem(); //vms colocar um item aleatorio no inventario
        }
        for (int i = 0; i < inventorySlots.Length; i++) //loop no inventario
        {
            if(inventorySlots[i].myItem == null) // se o slot do inventario for vazio
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]); //o item temporario "_item" é criado
                break;
            }
        }

    }	

    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length); //função para gerar um item aleatorio
        return items[random];
    }

    Item PickItem(Item pickItem) //vms pegar um item especific na nossa database
    {
        Item selectItem = null; 

        foreach (var item in items) //loop em todos os itens em nossa db
        {
            if(item.name == pickItem.name) //se o nome do item é igual ao que a gente quer
            {
                selectItem = item; //selecione o item e atribui como select item
                break;
            }
        }
        return selectItem;

    }

    public void PickupItem(Item item)
    {
        Item _item = item; //cria um item temporario para teste
        if(_item == null) //verif se item é vazio
        {
            _item = PickItem(item); //vms criar um item temporario da base de dados
        }
        for (int i = 0; i < inventorySlots.Length; i++) //loop no inventario
        {
            if(inventorySlots[i].myItem == null) // se o slot do inventario for vazio
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]); //o item temporario "_item" é criado
                break;
            }
        }  
    }

    public void DropItem(InventoryItem item)
    {
        Debug.Log($"Drop item: {item.name}");
        SpawnObjectNearPlayer(item);
        Destroy(item.gameObject); //ao dropar vai destruir o item do inventário
    }

    public void SpawnObjectNearPlayer(InventoryItem item)
    {
        Transform player = GameObject.Find("Player").transform; //localização pelo gameobj player
        Vector2 randomDirection = Random.insideUnitCircle.normalized; //vai pegar uma outra posição random de um circulo
        float randomDistance = Random.Range(0.1f, 0.5f); //distancia random max e min.
        Vector2 spawnPosition = (Vector2)player.position + randomDirection * randomDistance; //formula para pawn

        GameObject dropItemPrefab = Instantiate(item.myItem.prefab, spawnPosition, Quaternion.identity); //quarteniom é a rotacao do item.
        dropItemPrefab.GetComponent<SpriteRenderer>().sprite = item.myItem.sprite; //vai pegar o sprite do myitem a imagem do item dropado.
        dropItemPrefab.GetComponent<PickupItem>().item = item.myItem; // coloca dentro da pickupitem qual item foi dropado (para permitir catar denovo esse item)
        

    }   
}
