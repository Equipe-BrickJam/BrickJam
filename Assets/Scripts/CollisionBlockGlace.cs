using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;// pour accéder aux propriétés du NetworkTransform

public class CollisionBlockGlace : NetworkBehaviour
{
    public GameObject balle;

    public Sprite blocInitialGlace;
    public Sprite blockDamage1Glace;
    public Sprite blockDamage2Glace;

    private SpriteRenderer spriteRenderer;


    //Variable du son
    public AudioClip BreakingGlass;
    private AudioSource audioSource;

    //Compteur collision de la balle: NetworkVariable initialisé à 0
    private NetworkVariable<int> nombreCollisionsBalle = new NetworkVariable<int>(0);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Chercher le sprite renderer une seule fois dans le start au lieu du update
        spriteRenderer = GetComponent<SpriteRenderer>();


        // l'audioSource viens chercher le component du block 
        audioSource = GetComponent<AudioSource>();

        //S'assurer que la sprite initiale est charg�e en premier
        spriteRenderer.sprite = blocInitialGlace;

        // Quand la valeur des collisions change, on met à jour le sprite localement (clients et serveur)
        nombreCollisionsBalle.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue == 1)
            {
                spriteRenderer.sprite = blockDamage1Glace;
            }
            else if (newValue == 2)
            {
                spriteRenderer.sprite = blockDamage2Glace;
            }
        };
    }


    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        if (!IsServer) return;

        if (infoCollision.gameObject.CompareTag("BalleJoueur1"))
        {
            //Le nombre de fois augmente de 1
            nombreCollisionsBalle.Value++;
            // Le son du brise glasse joue
            audioSource.PlayOneShot(BreakingGlass);

            if (nombreCollisionsBalle.Value >= 3)
            {
                GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }
}