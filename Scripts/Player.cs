using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{   
    public Entity entity;
    [Header("Player Regen System")]
    public bool regenHPEnabled = true;
    public float regenHPTime = 5f;
    public int regenHPValue = 5;
    public bool regenMPEnabled = true;
    public float regenMPTime = 10f;
    public int regenMPValue = 5;

    [Header("Game Manager")]
    public GameManager manager;

    [Header("Player Shortcuts")]
    public KeyCode attributesKey = KeyCode.C;
    public KeyCode inventoryKey = KeyCode.I; // key para abrir ou fechar a janela de attributos

    [Header("Player UI Panels")]
    public GameObject attributesPanel;
    public GameObject inventoryPanel;

    [Header("Player UI")]
    public Slider health;
    public Slider mana;
    public Slider stamina;
    public Slider exp;

    public Text expText;

    public Text levelText;

    public Text strTxt;
    public Text resTxt;
    public Text intTxt;
    public Text wilTxt;
    public Text pointsTxt;
    public Button strPositiveBtn;
    public Button resPositiveBtn;
    public Button intPositiveBtn;
    public Button wilPositiveBtn;
    public Button strNegativeBtn;
    public Button resNegativeBtn;
    public Button intNegativeBtn;
    public Button wilNegativeBtn;

    [Header("Exp")]
    public int currentExp;
    public int expBase;
    public int expLeft;
    public float expMod;//um modificar para exp, tipo evento com mais exp
    public int givePoints = 5;
    public GameObject levelUpFX;
    public AudioClip levelUpSound;

    [Header("Respawn")]
    public float respawnTime = 5;
    

    public GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (manager == null)
        {
            Debug.LogError("Você precisa anexar o GameManager aqui no player");
            return;
        }
        
        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;
        //teste
        //int dmg = manager.CalculateDamage(entity, 10);
        //int def = manager.CalculateDefense(entity, 5);

        health.maxValue = entity.maxHealth;
        health.value = health.maxValue;

        mana.maxValue = entity.maxMana;
        mana.value = mana.maxValue;

        stamina.maxValue = entity.maxStamina;
        stamina.value = stamina.maxValue;

        exp.value = currentExp;
        exp.maxValue = expLeft;
        expText.text = String.Format("Exp: {0}/{1}", currentExp, expLeft); // barra de info de exp
        levelText.text = entity.level.ToString();

        // iniciar a regeneração
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
        
        UpdatePoints();
        SetupUIButtons();

    }
    //iniciar o regen health e mana
    


    private void Update()
    {
        if(entity.dead)
            return;
        
        if(entity.currentHealth <= 0)
        {
            Die();
        }

        if(Input.GetKeyDown(attributesKey)) //abrir o painel de atributos
        {
            attributesPanel.SetActive(!attributesPanel.activeSelf);
        }

        if(Input.GetKeyDown(inventoryKey)) //abrir o painel de equipamentos
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        if(attributesPanel.activeSelf) //quando o painel de attribute estiver ativo
        {
            strTxt.text = entity.strength.ToString();  
            resTxt.text = entity.resistence.ToString();  
        }
        
        health.value = entity.currentHealth;
        mana.value = entity.currentMana;
        stamina.value = entity.currentStamina;
        exp.value = currentExp;
        exp.maxValue = expLeft;

        expText.text = String.Format("Exp: {0}/{1}", currentExp, expLeft); // barra de info de exp
        levelText.text = entity.level.ToString();
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    entity.currentHealth -= 5;
        //    entity.currentMana -= 5;                
        //} //teste regens


    }
    // Update is called once per frame

    IEnumerator RegenHealth()
    {
        while(true) //loop
        {   
            if(regenHPEnabled)
            {
                if(entity.currentHealth < entity.maxHealth)
                {   
                    Debug.LogFormat("Recuperando HP");
                    entity.currentHealth += regenHPValue;
                    yield return new  WaitForSeconds(regenHPTime);
                }
                else
                {
                    yield return null;
                }	
            }

            else
            {
                yield return null;
            }
        }
    }
    IEnumerator RegenMana()
    {
        while(true) //loop
        {   
            if(regenHPEnabled)
            {
                if(entity.currentMana< entity.maxMana)
                {   
                    Debug.LogFormat("Recuperando MP");
                    entity.currentMana += regenMPValue;
                    yield return new  WaitForSeconds(regenMPTime);
                }
                else
                {
                    yield return null;
                }
            }

            else
            {
                yield return null;
            }
                         
        }

    }

    void Die ()
    {
        entity.currentHealth = 0;
        entity.dead = true;
        entity.target = null;
        StopAllCoroutines();
        StartCoroutine(Respawn());

    }
    IEnumerator Respawn()
    {   
        GetComponent<PlayerController>().enabled = false; //nao mover o player apos morto

        yield return new WaitForSeconds(respawnTime);

        GameObject newPlayer = Instantiate(prefab, transform.position, transform.rotation, null); //vai criar novo monstro na mesma posição
        newPlayer.name = prefab.name;
        newPlayer.GetComponent<Player>().entity.dead = false;
        newPlayer.GetComponent<Player>().entity.combatCoroutine = true; //monster apos renascer nao esta em modo combate
        newPlayer.GetComponent<PlayerController>().enabled = true;
        Destroy(this.gameObject);
    }
    // add exp no player
    
    public void GainExp(int amount) // amount é a qtd de exp q vamos receber
    {
        currentExp += amount;
        if(currentExp >= expLeft)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentExp -= expLeft; //exp atual vai subtrair do exp left para up
        entity.level++;
        entity.points += givePoints;
        UpdatePoints();

        entity.currentHealth = entity.maxHealth; //vida vai ser restaurado ao upar
        


        float newExp = Mathf.Pow((float)expMod, entity.level); //novo calculo para o xp
        expLeft = (int)Mathf.Floor((float)expBase * newExp); // novo calculo para upar 

        entity.entityAudio.PlayOneShot(levelUpSound);//play na musica apenas uma vez.
        Instantiate(levelUpFX, this.gameObject.transform); // coloca o efeito de levelup no personagem
    }

    public void UpdatePoints()
    {
        strTxt.text = entity.strength.ToString();
        resTxt.text = entity.resistence.ToString();
        intTxt.text = entity.inteligence.ToString();
        wilTxt.text = entity.willpower.ToString();
        pointsTxt.text = entity.points.ToString(); 
    }

    public void SetupUIButtons()
    {
        strPositiveBtn.onClick.AddListener(() => AddPoints(1)); // ao clicar no botao vai add pontos
        resPositiveBtn.onClick.AddListener(() => AddPoints(2));
        intPositiveBtn.onClick.AddListener(() => AddPoints(3));
        wilPositiveBtn.onClick.AddListener(() => AddPoints(4));

        strNegativeBtn.onClick.AddListener(() => RemovePoints(1)); // ao clicar no botao vai add pontos
        resNegativeBtn.onClick.AddListener(() => RemovePoints(2));
        intNegativeBtn.onClick.AddListener(() => RemovePoints(3));
        wilNegativeBtn.onClick.AddListener(() => RemovePoints(4));
    }

    public void AddPoints(int index)  //botoes de add atrib ao valor atributo
    {
        
        if(entity.points > 0)
        {
            if(index == 1 )//str
                entity.strength++;
            else if (index == 2)
                entity.resistence++;
            else if (index == 3)
                entity.inteligence++;
            else if (index == 4)
                entity.willpower++;
            
            entity.points--; //estamos usando os pontos disponíveis, portanto negativo
            UpdatePoints();
        }
    }

    public void RemovePoints(int index) // botoes de diminuir atrib ao valor atrib
    {
        if(entity.points > 0)
        {
            if(index == 1 && entity.strength> 0 )//str
                entity.strength--;
            else if (index == 2 && entity.resistence> 0 )
                entity.resistence--;
            else if (index == 3 && entity.inteligence> 0 )
                entity.inteligence--;
            else if (index == 4 && entity.willpower> 0 )
                entity.willpower--;

            entity.points++; //estamos oferecendo pontos disponiveis, portanto acrescenta
            UpdatePoints();
        }
    } 

 
}
