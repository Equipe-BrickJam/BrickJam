using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;// pour accéder aux propriétés du NetworkTransform
using UnityEngine.SceneManagement;

public class BalleRigid : NetworkBehaviour
{

    public GameObject pointage;

    public GameObject Game;

    public static BalleRigid instance; // Singleton
    float maxDistanceY = 4.7f; // moitié de la largeur de la table, pour savoir si un but est compté
    [SerializeField] private float nombreDeBonds; //compte du nombre de bonds de la balle // Servira plus tard
    public string tagJoueur1 = "BalleJoueur1";
    public string tagJoueur2 = "BalleJoueur2";
    public float vitesseDepart = 10f;

    public Sprite balleInitial;
    public Sprite balleRouge;
    public Sprite balleBleu;
    private SpriteRenderer spriteRenderer;

    public bool blocsGlaceDetruits = false;
    public bool blocsFeuDetruits = false;

    public float pointageServeur; // pointage Bleu

    public float pointageClient; // pointage Rouge

    //Variable du son
    public AudioClip sonRebond;
    public AudioClip sonSwitchCouleur;
    private AudioSource audioSource;

    public TextMeshProUGUI textePointageBleu;
    public TextMeshProUGUI textePointageRouge;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Chercher le sprite renderer une seule fois dans le start au lieu du update
        spriteRenderer = GetComponent<SpriteRenderer>();

        // l'audioSource viens chercher le component du block 
        audioSource = GetComponent<AudioSource>();

        //Initie le sprite de la balle en premier
        spriteRenderer.sprite = balleInitial;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (!GameManager.instance.partieEnCours) return;

        //but client
        if (transform.position.y < -maxDistanceY)
        {
            //LanceBalleMilieu();
        }

        //but serveur (hôte)
        if (transform.position.y > maxDistanceY)
        {
            //LanceBalleMilieu();
        }

        ScoreRpc();
    }
    [Rpc(SendTo.Everyone)]
    public void ScoreRpc()
    {
        //****************TEST****************//
        if (Input.GetKeyDown(KeyCode.G) || GameObject.FindGameObjectsWithTag("BlocGlace").Length == 0)
        {
            blocsGlaceDetruits = true;
            Pointage.instance.AjouterPointage(false, 1);
            pointageServeur += 1;
            Game.GetComponent<GameManager>().NouvellePartie();
        }

        //****************TEST****************//
        if (Input.GetKeyDown(KeyCode.F) || GameObject.FindGameObjectsWithTag("BlocFeu").Length == 0)
        {
            blocsFeuDetruits = true;
            Pointage.instance.AjouterPointage(true, 1);
            pointageClient += 1;
            Game.GetComponent<GameManager>().NouvellePartie();
        }

        textePointageBleu.text = pointageServeur.ToString();
        textePointageRouge.text = pointageClient.ToString();
    }

    public void LanceBalleMilieu()
    {
        //nombreDeBonds = 0;
        GetComponent<NetworkTransform>().Interpolate = false;
        transform.position = new Vector2(0f, 0.5f);
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (GameManager.instance.partieTerminee) return;
        StartCoroutine(NouvelleBalle());
    }

    IEnumerator NouvelleBalle()
    {
        yield return new WaitForSecondsRealtime(1f);
        GetComponent<NetworkTransform>().Interpolate = true;

        //La balle peut partir dans tout les angles
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        GetComponent<Rigidbody2D>().AddForce(direction * vitesseDepart, ForceMode2D.Impulse);
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int spriteType)
    {
        switch (spriteType)
        {
            case 0: spriteRenderer.sprite = balleInitial; break;
            case 1: spriteRenderer.sprite = balleBleu; break;
            case 2: spriteRenderer.sprite = balleRouge; break;
        }
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        //Si elle rentre en collision avec un block
        if(infoCollision.gameObject.tag == "BlocFeu" || infoCollision.gameObject.tag == "BlocGlace")
        {
            // On fait jouer le son du rebond
            audioSource.PlayOneShot(sonRebond);

        }


        //Si elle rentre en collision avec un blouclier OU un block
        if (infoCollision.gameObject.tag == "Joueur1")
        {
            gameObject.tag = tagJoueur1;
            ChangeSpriteClientRpc(1); // Change la couleur de la balle en bleu pour le joueur 1
            
            // Le son de la balle qui change de couleur
            audioSource.PlayOneShot(sonSwitchCouleur);
        }

        else if (infoCollision.gameObject.tag == "Joueur2")
        {
            gameObject.tag = tagJoueur2;
            ChangeSpriteClientRpc(2); // Change la couleur de la balle en rouge pour le joueur 2

            // Le son de la balle qui change de couleur
            audioSource.PlayOneShot(sonSwitchCouleur);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Si la balle rentre en collision avec les extrémités du bas et du haut, elle reviens au millieu
        if (other.gameObject.tag == "Limite")
        {
            // Stop la balle
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Remet à la position de départ
            transform.position = new Vector2(0f, 0.5f);

            // On projeter de facon random

            // Attend un petit délai avant de relancer la balle (optionnel)
            //StartCoroutine(LancerBalleApresDelai(0.5f));
            
            StartCoroutine(NouvelleBalle());
        }
    }

    private IEnumerator LancerBalleApresDelai(float delai)
    {
        yield return new WaitForSeconds(delai);

        // Crée une direction aléatoire X et Y (évite les 0)
        float x = UnityEngine.Random.Range(-1f, 1f);
        float y = UnityEngine.Random.Range(0.5f, 1f); // vers le haut seulement

        Vector2 direction = new Vector2(x, y).normalized;

        float force = 5f; // Ajuste la vitesse ici

        // Applique la vitesse
        GetComponent<Rigidbody2D>().linearVelocity = direction * force;
    }

    [Rpc(SendTo.Everyone)]
    public void Joueur1GagneRpc()
    {
        SceneManager.LoadScene("BleuGagne");
    }

    public void Joueur2GagneRpc()
    {
        SceneManager.LoadScene("RougeGagne");
    }
}