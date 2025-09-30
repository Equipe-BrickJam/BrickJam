using UnityEngine;
using Unity.Netcode; // namespace pour utiliser Netcode
using UnityEngine.SceneManagement; // namespace pour la gestion des scènes

public class GameManager : NetworkBehaviour
{
    public GameObject Player1;
    public GameObject Player2;
    public GameObject menu;
    public GameObject panelDeConnexion;

    [SerializeField] private GameObject ballPrefab; // assign prefab in inspector

    public GameObject[] blocGlacePrefab;
    public GameObject[] blocFeuPrefab;  

    public NetworkObject boutonHote;
    public NetworkObject boutonClient;

    public static GameManager instance;// Singleton pour parler au GameManager de n'importe où
    public bool partieEnCours { private set; get; } //permet de savoir si une partie est en cours
    public bool partieTerminee { private set; get; } // permet de savoir si une partie est terminée

    public BalleRigid balleInstance;

    //Variable du son
    public AudioClip BreakingGlass;
    private AudioSource audioSource;


    // Création du singleton si nécessaire
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //dont destroyonload
        }
        else
        {
            Destroy(gameObject);
        }
        // Abonnement au callback OnClientConnectedCallback qui lancera la fonction OnNouveauClientConnecte.
        NetworkManager.Singleton.OnClientConnectedCallback += OnNouveauClientConnecte;
    }

    private void Start()
    {
        // l'audioSource viens chercher le component du block 
        audioSource = GetComponent<AudioSource>();
    }
    public void OnNouveauClientConnecte(ulong obj)
    {
        if (!IsServer)
        {

            // Si je suis client j'affiche le panneua d'attente 
            //NavigationManager.singleton.AfficheAttenteClient();

            return;
        }

        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            GameObject nouveauJoueur = Instantiate(Player1);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
            // Si je suis client j'affiche le panneua d'attente 
            //NavigationManager.singleton.AfficheAttenteServeur();
            //if (IsServer)
            //{
            //    boutonHote.Despawn(true);
            //}
            panelDeConnexion.SetActive(false);

        }
        else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            GameObject nouveauJoueur = Instantiate(Player2);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
            //NavigationManager.singleton.AffichePanelServeurPartie();
            //if (IsServer)
            //{
            //    boutonClient.Despawn(true);
            //}


        }

    }

    void Update()
    {
        //if (!IsHost) return;
        if (partieEnCours) return;

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            NouvellePartie();
            menu.SetActive(false); // On désactive le canvas ( menu du début ) pour ne plus voir le fond et le titre

        }
    }




    public void NouvellePartie()
    {
        if (!IsServer) return;

        partieEnCours = true;

        // If the ball doesn’t exist yet, spawn it
        if (BalleRigid.instance == null)
        {
            GameObject balle = Instantiate(ballPrefab, new Vector2(0f, 0.5f), Quaternion.identity);
            balle.GetComponent<NetworkObject>().Spawn(true); // true = server owns it
            balleInstance = balle.GetComponent<BalleRigid>();
        }

        foreach (var blocGlaceSpawn in blocGlacePrefab)
        {
            var blocGlaceCopie = Instantiate(blocGlaceSpawn);
            blocGlaceCopie.GetComponent<NetworkObject>().Spawn();
            blocGlaceCopie.SetActive(true);
        }

        foreach (var blocFeuSpawn in blocFeuPrefab)
        {
            var blocFeuCopie = Instantiate(blocFeuSpawn);
            blocFeuCopie.GetComponent<NetworkObject>().Spawn();
            blocFeuCopie.SetActive(true);
        }

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
        // Le son du brise glasse joue
        audioSource.PlayOneShot(BreakingGlass);
        NetworkManager.Singleton.StartHost(); // Fonction du NetworkManager pour démarrer une partie comme hôte
        panelDeConnexion.SetActive(false);

    }

    // Fonction appelée pour le bouton qui permet de se connecter comme client
    public void LanceCommeClient() // Public pour être appeler de l'extérieur (par le bouton Client)
    {
        // Le son du brise glasse joue
        audioSource.PlayOneShot(BreakingGlass);
        NetworkManager.Singleton.StartClient(); // Fonction du NetworkManager pour démarrer une partie comme client
        panelDeConnexion.SetActive(false);

    }


    //changement de scene
    //NetworkManager Singleton.SceneManager Loadscene("Jeu")
}