using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components; // Pour accéder aux propriétés du NetworkTransform
using UnityEngine.SceneManagement;

public class BalleRigid : NetworkBehaviour
{
    public GameObject pointage;

    public GameObject Game;

    public static BalleRigid instance; // Singleton

    float maxDistanceY = 4.7f; // Moitié de la hauteur du terrain, utilisé pour détecter les buts

    public string tagJoueur1 = "BalleJoueur1";
    public string tagJoueur2 = "BalleJoueur2";
    public float vitesseDepart = 10f;

    public Sprite balleInitial;
    public Sprite balleRouge;
    public Sprite balleBleu;
    private SpriteRenderer spriteRenderer;

    public bool blocsGlaceDetruits = false;
    public bool blocsFeuDetruits = false;

    public float pointageServeur; // Pointage pour le joueur Bleu (serveur)
    public float pointageClient;  // Pointage pour le joueur Rouge (client)

    // Variables audio
    public AudioClip sonRebond;
    public AudioClip sonSwitchCouleur;
    private AudioSource audioSource;

    public TextMeshProUGUI textePointageBleu;
    public TextMeshProUGUI textePointageRouge;

    private void Awake()
    {
        // Mise en place du singleton
        if (instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }

    // Start est appelé avant la première frame Update
    void Start()
    {
        // Référence au SpriteRenderer de la balle (à faire une seule fois)
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Récupère le composant AudioSource sur la balle
        audioSource = GetComponent<AudioSource>();

        // Initialise le sprite de la balle
        spriteRenderer.sprite = balleInitial;
    }

    // Update est appelé une fois par frame
    void Update()
    {
        // Exécute uniquement sur le serveur
        if (!IsServer)
        {
            return;
        }

        if (!GameManager.instance.partieEnCours) return;

        // But pour le joueur 1 (la balle sort en bas)
        if (transform.position.y < -maxDistanceY)
        {
            //LanceBalleMilieu();
        }

        // But pour le joueur 2 (la balle sort en haut)
        if (transform.position.y > maxDistanceY)
        {
            //LanceBalleMilieu();
        }

    }

    [Rpc(SendTo.Everyone)]
    public void ScoreRpc()
    {
        // Si tous les blocs de glace (équipe bleue) sont détruits
        if (GameObject.FindGameObjectsWithTag("BlocGlace").Length == 0 || Input.GetKey(KeyCode.G))
        {
            blocsGlaceDetruits = true;
            Pointage.instance.AjouterPointage(false, 1);
            pointageServeur += 1;
            Game.GetComponent<GameManager>().NouvellePartie();
            // Vérifie si un camp a perdu tous ses blocs
            ScoreRpc();
        }

        // Si tous les blocs de feu (équipe rouge) sont détruits
        if (GameObject.FindGameObjectsWithTag("BlocFeu").Length == 0 || Input.GetKey(KeyCode.F))
        {
            blocsFeuDetruits = true;
            Pointage.instance.AjouterPointage(true, 1);
            pointageClient += 1;
            Game.GetComponent<GameManager>().NouvellePartie();
            // Vérifie si un camp a perdu tous ses blocs
            ScoreRpc();
        }

        // Met à jour les textes de pointage
        textePointageBleu.text = pointageServeur.ToString();
        textePointageRouge.text = pointageClient.ToString();
    }

    public void LanceBalleMilieu()
    {
        // Désactive l’interpolation réseau temporairement pour repositionner proprement
        GetComponent<NetworkTransform>().Interpolate = false;

        // Replace la balle au centre du terrain
        transform.position = new Vector2(0.12f, -1.81f);
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (GameManager.instance.partieTerminee) return;

        // Relance la balle après un délai
        StartCoroutine(NouvelleBalle());
    }

    [Rpc(SendTo.Everyone)]
    private void SynchroniserPositionClientRpc(Vector2 nouvellePosition)
    {
        // Synchronise la position de la balle côté client
        transform.position = nouvellePosition;

        // Par sécurité, stoppe aussi le mouvement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator NouvelleBalle()
    {
        yield return new WaitForSecondsRealtime(1f);

        // Calcule une direction aléatoire (angle complet)
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 5f);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        // Applique une impulsion dans cette direction
        GetComponent<Rigidbody2D>().AddForce(direction * vitesseDepart, ForceMode2D.Impulse);
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int spriteType)
    {
        // Change la couleur de la balle selon le joueur
        switch (spriteType)
        {
            case 0: spriteRenderer.sprite = balleInitial; break;
            case 1: spriteRenderer.sprite = balleBleu; break;
            case 2: spriteRenderer.sprite = balleRouge; break;
        }
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        // Collision avec un bloc feu ou glace
        if (infoCollision.gameObject.tag == "BlocFeu" || infoCollision.gameObject.tag == "BlocGlace")
        {
            // Joue le son du rebond
            audioSource.PlayOneShot(sonRebond);
        }

        // Collision avec un joueur
        if (infoCollision.gameObject.tag == "Joueur1")
        {
            gameObject.tag = tagJoueur1;
            ChangeSpriteClientRpc(1); // Balle devient bleue

            // Son du changement de couleur
            audioSource.PlayOneShot(sonSwitchCouleur);
        }
        else if (infoCollision.gameObject.tag == "Joueur2")
        {
            gameObject.tag = tagJoueur2;
            ChangeSpriteClientRpc(2); // Balle devient rouge

            // Son du changement de couleur
            audioSource.PlayOneShot(sonSwitchCouleur);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si la balle touche une limite (haut/bas du terrain)
        if (other.gameObject.tag == "Limite")
        {
            // Stoppe le mouvement
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Replace la balle au centre
            transform.position = new Vector2(0f, 0.5f);

            // Relance la balle après un petit délai
            StartCoroutine(NouvelleBalle());
        }
    }

    private IEnumerator LancerBalleApresDelai(float delai)
    {
        yield return new WaitForSeconds(delai);

        // Crée une direction aléatoire (évite les valeurs nulles)
        float x = UnityEngine.Random.Range(-1f, 1f);
        float y = UnityEngine.Random.Range(0.5f, 1f); // Vers le haut uniquement

        Vector2 direction = new Vector2(x, y).normalized;
        float force = 5f; // Force de lancement

        // Applique la vitesse à la balle
        GetComponent<Rigidbody2D>().linearVelocity = direction * force;
    }

    [Rpc(SendTo.Everyone)]
    public void Joueur1GagneRpc()
    {
        // Charge la scène de victoire du joueur 1
        SceneManager.LoadScene("BleuGagne");
    }

    public void Joueur2GagneRpc()
    {
        // Charge la scène de victoire du joueur 2
        SceneManager.LoadScene("RougeGagne");
    }
}
