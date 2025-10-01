using UnityEngine;
using Unity.Netcode; // namespace pour utiliser Netcode
using UnityEngine.SceneManagement; // namespace pour la gestion des scènes

// Gère l’ensemble de la logique du jeu : joueurs, balles, blocs, et transitions entre états (menu, partie, fin, etc.)
public class GameManager : NetworkBehaviour
{
    // Références aux deux joueurs (leurs prefabs)
    public GameObject Player1;
    public GameObject Player2;

    // Références aux éléments UI
    public GameObject menu;              // Menu principal à cacher quand la partie commence
    public GameObject panelDeConnexion;  // Panel pour choisir entre Hôte ou Client

    [SerializeField] private GameObject ballPrefab; // Prefab de la balle (assigné dans l’inspecteur)

    // === PARAMÈTRES POUR LA GÉNÉRATION DES BLOCS ===

    [SerializeField] private GameObject blocFeuPrefab;   // Assigné dans l'inspecteur
    [SerializeField] private GameObject blocGlacePrefab; // Assigné dans l'inspecteur

    [SerializeField] private int lignes = 6;      // Nombre de rangées de blocs
    [SerializeField] private int colonnes = 8;    // Nombre de blocs par rangée

    [SerializeField] private float espacementX = 1.2f; // Distance horizontale entre les blocs
    [SerializeField] private float espacementY = 0.6f; // Distance verticale entre les rangées

    [SerializeField] private Vector2 positionDepart = new Vector2(-8f, 3.5f); // Position du premier bloc en haut à gauche


    // Références aux boutons réseau
    public NetworkObject boutonHote;
    public NetworkObject boutonClient;

    // Singleton : permet d’accéder au GameManager depuis n’importe quel script via GameManager.instance
    public static GameManager instance;

    // États du jeu
    public bool partieEnCours { private set; get; } //permet de savoir si une partie est en cours
    public bool partieTerminee { private set; get; } // permet de savoir si une partie est terminée

    // Référence à la balle en jeu
    public BalleRigid balleInstance;

    // Sons
    public AudioClip BreakingGlass;
    private AudioSource audioSource;


    void Awake()
    {
        // Singleton : s’il n’existe pas encore, on en fait ce script l’instance unique
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this.gameObject); => pour garder ce GameManager entre les scènes
        }
        else
        {
            Destroy(gameObject); // Empêche d’avoir deux GameManagers dans la scène
        }

        // Dès qu’un joueur se connecte, on appelle OnNouveauClientConnecte()
        // Abonnement au callback OnClientConnectedCallback qui lancera la fonction OnNouveauClientConnecte.
        NetworkManager.Singleton.OnClientConnectedCallback += OnNouveauClientConnecte;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Permet de jouer des sons plus tard
    }

    public void OnNouveauClientConnecte(ulong obj)
    {
        if (!IsServer)
        {

            // Si je suis client j'affiche le panneau d'attente 
            //NavigationManager.singleton.AfficheAttenteClient();

            // Si je ne suis pas le serveur, je ne fais rien de plus
            return;
        }

        // Si c’est le 1er joueur connecté, on instancie Player1
        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            GameObject nouveauJoueur = Instantiate(Player1);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);

            // Si je suis client j'affiche le panneua d'attente 
            //NavigationManager.singleton.AfficheAttenteServeur();

            panelDeConnexion.SetActive(false); // On cache le panneau de connexion

        }

        // Si c’est le 2ème joueur connecté, on instancie Player2
        else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            GameObject nouveauJoueur = Instantiate(Player2);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);

            //NavigationManager.singleton.AffichePanelServeurPartie();

        }

    }

    void Update()
    {
        //if (!IsHost) return;
        if (partieEnCours) return; // Ne fait rien si la partie est déjà commencée

        // Si deux joueurs sont connectés, on démarre la partie
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            NouvellePartie();
            menu.SetActive(false); // On désactive le menu du début

        }
    }




    public void NouvellePartie()
    {
        if (!IsServer) return;  // Seul le serveur peut lancer une nouvelle partie


        partieEnCours = true; // Marque que la partie est en cours

        // S’il n’y a pas déjà une balle en jeu, on en instancie une
        if (BalleRigid.instance == null)
        {
            GameObject balle = Instantiate(ballPrefab, new Vector2(0f, 0.5f), Quaternion.identity);
            balle.GetComponent<NetworkObject>().Spawn(true); // Le serveur possède la balle
            balleInstance = balle.GetComponent<BalleRigid>();
        }

        // === GÉNÉRATION DYNAMIQUE DES BLOCS ===
        for (int i = 0; i < lignes; i++)
        {
            for (int j = 0; j < colonnes; j++)
            {
                // Position de chaque bloc en fonction de sa ligne et de sa colonne
                Vector2 position = new Vector2(
                    positionDepart.x + j * espacementX,
                    positionDepart.y - i * espacementY
                );

                // Alterne entre feu et glace selon la ligne (tu peux adapter ça à ton besoin)
                GameObject prefab = (i % 2 == 0) ? blocFeuPrefab : blocGlacePrefab;

                // Instanciation du bloc
                GameObject bloc = Instantiate(prefab, position, Quaternion.identity);

                // Assure-toi que le prefab contient un NetworkObject
                bloc.GetComponent<NetworkObject>().Spawn();
            }
        }


        // Donne le coup d’envoi à la balle
        BalleRigid.instance.LanceBalleMilieu();
    }

    // Fonction appelée par le ScoreManager pour terminer la partie (nous l'utilserons plus tard)
    public void FinPartie()
    {
        partieTerminee = true; // Peut être utilisé pour afficher un écran de fin ou stopper les interactions
    }



    // Fonction appelée pour le bouton qui permet de se connecter comme hôte
    public void LanceCommeHote() // Public pour être appeler de l'extérieur (par le bouton Hôte)
    {

        audioSource.PlayOneShot(BreakingGlass); // Le son du brise glasse joue
        NetworkManager.Singleton.StartHost();   // Le joueur devient Hôte (Serveur + Client)
        panelDeConnexion.SetActive(false);      // Cache le panneau de choix

    }

    // Fonction appelée pour le bouton qui permet de se connecter comme client
    public void LanceCommeClient() // Public pour être appeler de l'extérieur (par le bouton Client)
    {

        audioSource.PlayOneShot(BreakingGlass); // Le son du brise glasse joue
        NetworkManager.Singleton.StartClient(); // Le joueur devient un simple client
        panelDeConnexion.SetActive(false);      // Cache le panneau de choix

    }
}