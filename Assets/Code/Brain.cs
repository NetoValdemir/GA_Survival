using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    // Características do Agente Inteligente
    public int DNALength = 1;                           // Comprimento do DNA
    public float timeAlive;                             // Ciclo de vida do agente
    public float distanceTravelled;                     // Distância percorrida pelo agente na plataforma
    Vector3 startPosition;                              // Posição incial do agente
    public DNA dna;                                     // Criação de variável atribuída a classe DNA

    // Atributos do script ThirdPersonCharacter
    private ThirdPersonCharacter m_Character;
    private Vector3 m_Move;
    private bool m_Jump;
    bool alive = true;

    // Método de identificação de colisão do agente
    void OnCollisionEnter(Collision obj)
    {
        if(obj.gameObject.tag == "dead")
        {
            alive = false;
        }
    }

    // Método de Inicialização
    public void Init()
    {
        // Inicializa o DNA
        // 0 Frente
        // 1 Trás
        // 2 Esquerda
        // 3 Direita
        // 4 Pulo
        // 5 Agachar

        dna = new DNA(DNALength, 6);                                // Determina o tamanho do DNA
        m_Character = GetComponent<ThirdPersonCharacter>();         // Atribuição do script ThirdPersonCharacter ao agente
        timeAlive = 0;                                              // Início do ciclo de vida
        alive = true;                                               // Condição de vivo ou morto
        startPosition = this.transform.position;                    // Posição inicial
    }

    private void FixedUpdate()
    {
        // Executa o DNA
        float h = 0;
        float v = 0;
        bool crouch = false;

        // Executa os comandos relacionados a movimentação do agente
        if (dna.GetGene(0) == 0) v = 1;
        else if (dna.GetGene(0) == 1) v = -1;
        else if (dna.GetGene(0) == 2) h = -1;
        else if (dna.GetGene(0) == 3) h = 1;
        else if (dna.GetGene(0) == 4) m_Jump = true;
        else if(dna.GetGene(0) == 5) crouch = true;

        // Movimentação geral do agente, baseada na combinação de todas as possíveis ações
        m_Move = v * Vector3.forward + h * Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;

        if (alive)
        {
            timeAlive += Time.deltaTime;                                                            // Contagem do ciclo de vida do agente
            distanceTravelled = Vector3.Distance(this.transform.position, startPosition);           // Contagem da distância percorrida pelo agente
        }
    }
}
