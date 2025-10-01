using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Pointage : MonoBehaviour
{
    // Initialisation des scores
    public int pointageServeur = 0;
    public int pointageClient = 0;

    public static Pointage instance;
    public BalleRigid scriptBalle;

    public int scoreVictoire = 3; // Score nécessaire pour gagner

    public TextMeshProUGUI textePointageBleu;
    public TextMeshProUGUI textePointageRouge;

    void Awake()
    {
        // Implémentation du pattern Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde ce GameObject entre les scènes
        }
        else
        {
            Destroy(gameObject); // Empêche les doublons
        }
    }

    // Méthode pour ajouter des points à un joueur
    public void AjouterPointage(bool isServer, int points)
    {
        // Vérifie que la partie est en cours et non terminée
        if (GameManager.instance.partieEnCours && !GameManager.instance.partieTerminee)
        {
            if (isServer)
            {
                pointageServeur += points;
                scriptBalle.blocsFeuDetruits = false; // Réinitialise l'état des blocs feu
            }
            else
            {
                pointageClient += points;
                scriptBalle.blocsGlaceDetruits = false; // Réinitialise l'état des blocs glace
            }

            Debug.Log($"Score Serveur: {pointageServeur} & Score Client: {pointageClient}");

            VerifierVictoire();
        }
    }

    // Vérifie si un joueur a atteint le score de victoire
    private void VerifierVictoire()
    {
        if (pointageClient >= scoreVictoire)
        {
            GameManager.instance.FinPartie();
            SceneManager.LoadScene("BleuGagne"); // Le client (joueur bleu) gagne
        }
        else if (pointageServeur >= scoreVictoire)
        {
            GameManager.instance.FinPartie();
            SceneManager.LoadScene("RougeGagne"); // Le serveur (joueur rouge) gagne
        }
    }
}
