using UnityEngine;
using Unity.Netcode; // namespace pour utiliser Netcode
using UnityEngine.SceneManagement; // namespace pour la gestion des scènes

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;// Singleton pour parler au GameManager de n'importe où
    
    public bool partieEnCours;
    public bool partieTerminee;

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
   }

   // Fonction appelée pour le bouton qui permet de se connecter comme hôte
   public void LanceCommeHote()
   {
        NetworkManager.Singleton.StartHost();
   }
    // Fonction appelée pour le bouton qui permet de se connecter comme client
    public void LanceCommeClient()
   {
        NetworkManager.Singleton.StartClient(); 
   }

    void Update()
    {
        if (!IsHost) return;
        if (partieEnCours) return;

        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            NouvellePartie();
        }
    }
    public void NouvellePartie()
    {
        partieEnCours = true;
    }
    public void FinPartie()
    {
        partieTerminee = true;
    }
    public void Recommencer()
    {
        NetworkManager.Singleton.Shutdown(); // On arrête le NetworkManager pour réinitialiser la partie
        Destroy(NetworkManager.gameObject);
        partieEnCours = false; // On remet la partie en cours à false
        SceneManager.LoadScene(0);// On recharge la scène de jeu
    }
}