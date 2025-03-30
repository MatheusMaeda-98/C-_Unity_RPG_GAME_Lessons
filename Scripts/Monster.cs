using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class Monster : MonoBehaviour
{
    [Header("Controller")]
    public Entity entity;

    public GameManager manager;

    [Header("Patrol")]
    public List<Transform> waypointList;
    public float arrivalDistance = 0.5f;
    public float waitTime = 5;
    public int waypointID;
    //privates
    Transform targetWapoint;
    int currentWaypoint = 0;
    float lastDistanceToTarget = 0f;
    float currentWaitTime = 0f;

    [Header("Experience Reward")]

    public int rewardExperience = 10;
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;

    [Header("Respawn")]

    public GameObject prefab;
    public bool respawn = true;
    public float respawnTime = 10f;

    [Header("UI")]
    public Slider healthSlider;
    
    Rigidbody2D rb2D;
    Animator animator;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint")) //loop do waypoint salvo apos monstro morrer 
        {   
            int ID = obj.GetComponent<WaypointID>().ID;
            if(ID == waypointID)
            {
                waypointList.Add(obj.transform); 
            }
        }

        currentWaitTime = waitTime;

        if(waypointList.Count > 0)
        {
            targetWapoint = waypointList[currentWaypoint];
            lastDistanceToTarget = Vector2.Distance(transform.position, targetWapoint.position);
        }
    }

    private void Update()
    {   
        if(entity.dead)
            return;

        if(entity.currentHealth <= 0) // se o monstro morreu
        {
            entity.currentHealth = 0;
            Die();
        }

        healthSlider.value = entity.currentHealth;

        if(!entity.inCombat)
        {
            if(waypointList.Count > 0)
            {
                Patrol(); //se nao estiver em combate o moster vai se mover
            }
            else
            {
                animator.SetBool("isWalking", false); // se nao, vai ficar parado.

            }
        }
        else
        {
            if(entity.attackTime > 0)
                entity.attackTime -= Time.deltaTime; // definição do tempo de attack

            if(entity.attackTime < 0)
                entity.attackTime = 0; // pra nao ficar negativo

            if(entity.target != null && entity.inCombat)//se estiver em combate e alvo
            {
                //atacar
                if(!entity.combatCoroutine)
                    StartCoroutine(Attack());
            }
            else
            {
                entity.combatCoroutine = false;
                StopCoroutine(Attack());

            }

        }
    }

//--------------------------------------SISTEMA DE VERIFICAÇÃO DE INIMIGO ---------------------------------------------
    private void OnTriggerStay2D (Collider2D collider)
    {
        if(collider.tag == "Player" && !entity.dead) //se monstro colidir c/ player e monster nao estiver morto.
        {
            entity.inCombat = true;
            entity.target = collider.gameObject;
            entity.target.GetComponent<BoxCollider2D>().isTrigger = true;
        }

    }

    private void OnTriggerExit2D (Collider2D collider)
    {
        if(collider.tag == "Player") 
        {
            entity.inCombat = false;
            if (entity.target)
            {
                entity.target.GetComponent<BoxCollider2D>().isTrigger = false;
                entity.target = null; // a gente nao tem alvo tambem

            } // se o player nao estiver perto a gente nao está em combate

        }
    }
//------------------------------------movimentação do monstro-----------------------------
    void Patrol()
    {
        if(entity.dead)
            return;
        
        //calcular a distancia do waypoint.
        float distanceToTarget = Vector2.Distance(transform.position, targetWapoint.position);
         // posição que a gente está para o nosso alvo
        if (distanceToTarget <= arrivalDistance || distanceToTarget >= lastDistanceToTarget) // se já chegou ao target
        {
            animator.SetBool("isWalking", false);

            if(currentWaitTime<= 0)
            { 
                currentWaypoint ++;

                if(currentWaypoint >= waypointList.Count) // não ultrapassar 
                    currentWaypoint = 0;

                targetWapoint = waypointList[currentWaypoint];
                lastDistanceToTarget = Vector2.Distance(transform.position,targetWapoint.position);
                
                currentWaitTime = waitTime;
            }

            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            animator.SetBool("isWalking", true); //animação para andar até o waypoint
            lastDistanceToTarget = distanceToTarget; // o quanto o monster deve andar.
        }

        Vector2 direction = (targetWapoint.position - transform.position).normalized;
        animator.SetFloat("input_x", direction.x);
        animator.SetFloat("input_y", direction.y);

        rb2D.MovePosition(rb2D.position + direction * (entity.speed * Time.fixedDeltaTime)); // o que faz mover nosso personagem

    }

    IEnumerator Attack() //ilenumerator acontece apos x segundos.
    {
        entity.combatCoroutine = true;
        while(true) 
        {
            yield return new WaitForSeconds(entity.cooldown);

            if(entity.target != null && !entity.target.GetComponent<Player>().entity.dead) //se o monstro tiver o alvo e se player nao estiver morto.
            {
                animator.SetBool("attack", true); // animar para attack
                
                float distance = Vector2.Distance(entity.target.transform.position, transform.position);

                if (distance <= entity.attackDistance)
                {
                    int dmg = manager.CalculateDamage(entity, entity.damage);
                    int targetDef = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense); //pegar a defesa do alvo
                    int dmgResult = dmg - targetDef;

                    if (dmgResult < 0)
                    {
                        dmgResult = 0; 
                    }

                    Debug.Log("Inimigo atacou o player, Dmg: " + dmgResult);
                    entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult; //subtrai a vida do usuario   
                }

            
            }
        
        
        }
    }


    void Die() /// se monstro morrer.
    {
        entity.dead = true;
        entity.inCombat = false;
        entity.target = null;

        animator.SetBool("isWalking", false);

        //add exp no player
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.GainExp(rewardExperience);

        Debug.Log("O inimigo morreu" + entity.name);

        StopAllCoroutines(); //criação do sistema de respawn do monstro
        StartCoroutine(Respawn());

    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation, null); //vai criar novo monstro na mesma posição
        newMonster.name = prefab.name;
        newMonster.GetComponent<Monster>().entity.dead = false;
        newMonster.GetComponent<Monster>().entity.combatCoroutine = false; //monster apos renascer nao esta em modo combate

        Destroy(this.gameObject);
    }

}
