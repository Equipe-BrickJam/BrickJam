using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Joueur : NetworkBehaviour
{
    public float vitesseDeplacement;   // Vitesse de déplacement du joueur
    public float vitesseMaximale;      // Vitesse maximale (non utilisée ici, pourrait servir à limiter la vitesse)

    private Vector2 velocitePerso;     // Vitesse actuelle appliquée au Rigidbody2D

    // Fonction appelée lors de l’apparition de l’objet réseau (avant Start)
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Position initiale différente selon si c’est le serveur ou le client
        if (IsServer)
        {
            transform.position = new Vector3(0f, -4.49f, 0f); // Position du joueur serveur
        }
        else
        {
            transform.position = new Vector3(0f, 4.49f, 0f);  // Position du joueur client
        }
    }

    // Update est appelé une fois par frame
    void Update()
    {
        // Seul le joueur local (propriétaire) peut se déplacer
        if (!IsOwner) return;

        Mouvement();
    }

    // Gère le mouvement horizontal du joueur
    private void Mouvement()
    {
        if (Input.GetKey(KeyCode.D)) // Déplacement vers la droite
        {
            velocitePerso.x = vitesseDeplacement;
        }
        else if (Input.GetKey(KeyCode.A)) // Déplacement vers la gauche
        {
            velocitePerso.x = -vitesseDeplacement;
        }
        else
        {
            velocitePerso.x = 0f; // Arrêt horizontal
        }

        // Applique la vitesse au Rigidbody2D
        GetComponent<Rigidbody2D>().linearVelocity = velocitePerso;
    }
}
