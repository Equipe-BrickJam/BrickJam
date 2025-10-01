using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components; // Pour accéder aux propriétés du NetworkTransform

public class CollisionBlockGlace : NetworkBehaviour
{
    public GameObject balle;

    public Sprite blocInitialGlace;
    public Sprite blockDamage1Glace;
    public Sprite blockDamage2Glace;

    private SpriteRenderer spriteRenderer;

    // Variable pour le son de casse
    public AudioClip BreakingGlass;
    private AudioSource audioSource;

    // Compteur de collisions avec la balle (synchronisé en réseau)
    private NetworkVariable<int> nombreCollisionsBalle = new NetworkVariable<int>(0);

    // Start est appelé avant la première frame Update
    void Start()
    {
        // Récupère le SpriteRenderer une seule fois
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Récupère le composant AudioSource attaché à ce bloc
        audioSource = GetComponent<AudioSource>();

        // Initialise le sprite avec l'apparence de départ
        spriteRenderer.sprite = blocInitialGlace;

        // Met à jour le sprite à chaque changement de valeur du compteur (client et serveur)
        nombreCollisionsBalle.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateSprite(newValue);
        };
    }

    // Met à jour le sprite en fonction du nombre de collisions
    private void UpdateSprite(int state)
    {
        switch (state)
        {
            case 0: spriteRenderer.sprite = blocInitialGlace; break;
            case 1: spriteRenderer.sprite = blockDamage1Glace; break;
            case 2: spriteRenderer.sprite = blockDamage2Glace; break;
        }
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        // Si la balle du joueur 1 touche le bloc
        if (infoCollision.gameObject.CompareTag("BalleJoueur1"))
        {
            // Incrémente le compteur de collisions
            nombreCollisionsBalle.Value++;

            // Joue le son de verre cassé
            audioSource.PlayOneShot(BreakingGlass);

            // Si le bloc a été touché 3 fois, il est détruit sur le réseau
            if (nombreCollisionsBalle.Value >= 3)
            {
                GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }
}
