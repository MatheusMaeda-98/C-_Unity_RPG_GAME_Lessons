using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Pet : MonoBehaviour
{
    public GameObject player;
    public Animator petAnimator;

    public float speed = 1.8f;
    public float keepDistance = +0.3f;

    bool isWalking = false;
    float input_x;
    float input_y;
    float lastDirectionX;
    float lastDirectionY;

    Vector2 petPos;
    Vector2 playerPos;

    private void Start()
    {
        petAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        petPos = transform.position;

        playerPos = SetDirection(1, 1, player.transform.position);
         // definindo como inicio, x=1, y=1 como last position do player
        transform.position = Vector2.MoveTowards(petPos, playerPos, speed * Time.deltaTime); //pet ja vai mover para prox ao jogador ao iniciar
        

    }

    void Update()
    {
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");
        isWalking = (input_x != 0 || input_y != 0); //verif. se pet está andando ou nao

        if(isWalking)
        {

            petAnimator.SetFloat("input_x",input_x);
            petAnimator.SetFloat("input_y",input_y);
        }

        if(input_x > 0 || input_x < 0)
        lastDirectionX = input_x;

        if(input_y >0 || input_y <0)
        lastDirectionY = input_y;

        petAnimator.SetBool("isWalking",isWalking); // se pet estiver parado

        petPos = transform.position;
        playerPos = SetDirection(lastDirectionX, lastDirectionY, player.transform.position); //ultima posição do nosso player

        transform.position = Vector2.MoveTowards(petPos, playerPos, speed * Time.deltaTime); //o pet vai seguir o player na velocidade correta
   

   
    }

    Vector2 SetDirection(float input_x, float input_y, Vector2 playerPos)
    {
        if (input_x < 0) // se player andar para esquerda
        {
            playerPos.x += keepDistance; //pet vai esquerda com a distancia do jogador, ficando a direita do player para ficar impressão do pet estar correndo atras do player
        }
        else if (input_x > 0) // se pla andar p/ direita
        {
            playerPos.x -= keepDistance; // pet vai para direita, ficando a esquerda do jogador 
        }

        if (input_y < 0) // se player andar para baixo
        {
            playerPos.y += keepDistance; //pet vai baixo com a distancia do jogador, ficando acima do player para ficar impressão do pet estar correndo atras do player
        }
        else if (input_y > 0) // se pla andar p/ cima
        {
            playerPos.y -= keepDistance; // pet vai para cima, ficando a abaixo do jogador 
        }

        return playerPos;

    }	
}   
