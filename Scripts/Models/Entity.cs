using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class Entity //identidade do player
{
    [Header("Name")]
    public string name;
    
    [Header("Health")]
    public int currentHealth;
    public int level;
    public int maxHealth;

    [Header("Mana")]
    public int currentMana;
    public int maxMana;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;

    [Header("Stats")]
    public int strength = 1;
    public int resistence = 1;
    public int willpower = 1;
    public int damage = 1;
    public int defense = 1;
    public int inteligence = 1;
    public float speed = 2f;
    public int points = 0;

    [Header("Combat")]
    public float attackDistance = 0.5f;
    public float attackTime = 1;
    public float cooldown = 2;
    public bool inCombat = false;
    public GameObject target;
    public bool combatCoroutine = false;
    public bool dead = false;
    
    [Header("Component")]
    public AudioSource entityAudio;


}
