using UnityEngine;
using Unity.Netcode; // namespace pour utiliser Netcode
using UnityEngine.SceneManagement; // namespace pour la gestion des scènes

public class GameManager : NetworkBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    public static GameManager instance;// Singleton pour parler au GameManager de n'importe où
    public bool partieEnCours{ private set; get; } //permet de savoir si une partie est en cours
    public bool partieTerminee{ private set; get; } // permet de savoir si une partie est terminée
    [SerializeField] private GameObject ballPrefab; // assign prefab in inspector


// Création du singleton si nécessaire
    void Awake()
   {
       if (instance == null)
       {
           instance = this;
       }
       else
       {
           Destroy(gameObject);
       }
        // Abonnement au callback OnClientConnectedCallback qui lancera la fonction OnNouveauClientConnecte.
       NetworkManager.Singleton.OnClientConnectedCallback += OnNouveauClientConnecte;
   }
    public void OnNouveauClientConnecte(ulong obj)
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
       NouvellePartie();
       }

    // void Update()
    // {
    //     if (!IsHost) return;
    //    if (partieEnCours) return;

    //    if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
    //    {
    //        NouvellePartie();
    //    }
    // }

    
   

    public void NouvellePartie()
    {
        partieEnCours = true;
        BalleRigid.instance.LanceBalleMilieu();
   }

    // Fonction appelée par le ScoreManager pour terminer la partie (nous l'utilserons plus tard)
   public void FinPartie()
   {
       partieTerminee = true;
   }

    

   // Fonction appelée pour le bouton qui permet de se connecter comme hôte
   public void LanceCommeHote() // Public pour être appeler de l'extérieur (par le bouton Hôte)
   {
       NetworkManager.Singleton.StartHost(); // Fonction du NetworkManager pour démarrer une partie comme hôte
   }

   // Fonction appelée pour le bouton qui permet de se connecter comme client
   public void LanceCommeClient() // Public pour être appeler de l'extérieur (par le bouton Client)
   {
       NetworkManager.Singleton.StartClient(); // Fonction du NetworkManager pour démarrer une partie comme client
   }
}