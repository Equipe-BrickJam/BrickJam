using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class Joueur : NetworkBehaviour
{
    public float vitesseDeplacement;
    public float vitesseMaximale;

    private Vector2 velocitePerso;

    public GameObject Player1;
    public GameObject Player2;

    // Fontion semblale au start, mais pour les objets réseaux et s'éxécute avant
   public override void OnNetworkSpawn()
   {
       base.OnNetworkSpawn();

       // Abonnement au callback OnClientConnectedCallback qui lancera la fonction OnNouveauClientConnecte.
       NetworkManager.Singleton.OnClientConnectedCallback += OnNouveauClientConnecte;

       if (IsServer)
       {
           transform.position = new Vector3(0f, -3.86f, 0f); //position à ajuster selon votre jeu
       }
       else
       {
           transform.position = new Vector3(0f, 3.86f, 0f); //position à ajuster selon votre jeu
       }
   }


    private void OnNouveauClientConnecte(ulong obj)
   {
       if (!IsServer) return;

       if (NetworkManager.Singleton.ConnectedClients.Count == 1) 
       {
           GameObject nouveauJoueur = Instantiate(Player1);
           nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
       }
       else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
       {
           GameObject nouveauJoueur = Instantiate(Player2);
           nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
           
       }
       //Mettre une fonction qui commence la partie
       GetComponent<GameManager>().NouvellePartie();
   }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;
       
        Mouvement();
    }

    private void Mouvement() 
    {
        if (Input.GetKey(KeyCode.D)) //Déplacement vers la droite
        {
            velocitePerso.x = vitesseDeplacement;
        }

        else if (Input.GetKey(KeyCode.A)) //Déplacement vers la gauche
        {
            velocitePerso.x = -vitesseDeplacement;
        }

        else
        {
            velocitePerso.x = 0f;
        }

         GetComponent<Rigidbody2D>().linearVelocity = velocitePerso;
    }
}
