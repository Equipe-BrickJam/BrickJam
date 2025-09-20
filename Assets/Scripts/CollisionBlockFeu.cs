using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;// pour accéder aux propriétés du NetworkTransform

public class CollisionBlockFeu : NetworkBehaviour
{
    public GameObject balle;

    private int NbFois = 0; //Le nombre de fois que la balle touche le block

    public Sprite blocInitialFeu;
    public Sprite blockDamage1Feu;
    public Sprite blockDamage2Feu;

    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Chercher le sprite renderer une seule fois dans le start au lieu du update
        spriteRenderer = GetComponent<SpriteRenderer>();

        //S'assurer que la sprite initiale est charg�e en premier
        spriteRenderer.sprite = blocInitialFeu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        if (infoCollision.gameObject.CompareTag("BalleJoueur2"))
        {
            //Le nombre de fois augmente de 1
            NbFois++;

            if (NbFois == 1)
            {
                //On change le sprite du blockInitial � blockDamage1
                spriteRenderer.sprite = blockDamage1Feu;
            }
            else if (NbFois == 2)
            {
                //On change le sprite du blockInitial � blockDamage1
                spriteRenderer.sprite = blockDamage2Feu;
            }
            else if (NbFois == 3)
            {
                //Détruit l'objet
                if (IsServer)
                {
                    NetworkObject.Despawn(true);
                }

            }
        }
    }
}
