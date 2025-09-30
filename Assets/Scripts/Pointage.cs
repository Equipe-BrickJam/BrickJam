using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Pointage : MonoBehaviour
{
    //Itinialisation du pointage
    public int pointageServeur = 0;
    public int pointageClient = 0;

    public static Pointage instance;
    public BalleRigid scriptBalle;

    public int scoreVictoire = 3;

    public TextMeshProUGUI textePointageBleu;
    public TextMeshProUGUI textePointageRouge;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    //M�thode pour ajouter des points
    public void AjouterPointage(bool isServer, int points)
    {
        if (GameManager.instance.partieEnCours && !GameManager.instance.partieTerminee)
        {
            if (isServer)
            {
                pointageServeur += points;
                scriptBalle.blocsFeuDetruits = false;
            }
            else
            {
                pointageClient += points;
                scriptBalle.blocsGlaceDetruits = false;
            }

            Debug.Log($"Score Serveur: {pointageServeur} & Score Client: {pointageClient}");

            VerifierVictoire();
        }
    }

    private void VerifierVictoire()
    {
        if (pointageClient >= scoreVictoire)
        {
            GameManager.instance.FinPartie();
            SceneManager.LoadScene("BleuGagne");
        }
        else if (pointageServeur >= scoreVictoire)
        {
            GameManager.instance.FinPartie();
            SceneManager.LoadScene("RougeGagne");
        }
    }
}
