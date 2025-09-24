using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;// pour accéder aux propriétés du NetworkTransform
using UnityEngine.SceneManagement;

public class BalleRigid : NetworkBehaviour
{
    public static BalleRigid instance; // Singleton
    float maxDistanceX = 25f; // moitié de la largeur de la table, pour savoir si un but est compté
    [SerializeField] private float nombreDeBonds; //compte du nombre de bonds de la balle // Servira plus tard
    public string tagJoueur1 = "BalleJoueur1";
    public string tagJoueur2 = "BalleJoueur2";
    public float vitesseDepart = 10f;

    public Sprite balleInitial;
    public Sprite balleRouge;
    public Sprite balleBleu;
    private SpriteRenderer spriteRenderer;

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
    if (!GameManager.instance.partieEnCours) return; // Il faudra créer cette variable dans le GameManager

    //but client
    if (transform.position.x < -maxDistanceX)
    {
      // Ici, il faudra aussi augmenter le score du joueur
      LanceBalleMilieu();
    }

    //but serveur (hôte)
    if (transform.position.x > maxDistanceX)
    {
      // Ici, il faudra aussi augmenter le score du joueur
      LanceBalleMilieu();
    }

    if (GameObject.FindGameObjectsWithTag("BlocGlace").Length == 0)
    {
      Joueur1Gagne();
    }

    else if (GameObject.FindGameObjectsWithTag("BlocFeu").Length == 0) 
    {
      Joueur2Gagne();
    }
  }

  public void LanceBalleMilieu()
    {
        nombreDeBonds = 0;
        GetComponent<NetworkTransform>().Interpolate = false;
        transform.position = new Vector2(0f, 0.5f);
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if (GameManager.instance.partieTerminee) return; // Il faudra créer cette variable dans le GameManager
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

    private void OnCollisionEnter2D(Collision2D infoCollision) 
   {
      //Si elle rentre en collision avec un blouclier OU un block
      if(infoCollision.gameObject.tag == "Joueur1")
      {
        gameObject.tag = tagJoueur1;
        Debug.Log("Tag changed to: " + gameObject.tag);
        spriteRenderer.sprite = balleBleu; // Change la couleur de la balle en bleu pour le joueur 1
       }

      else if (infoCollision.gameObject.tag == "Joueur2") 
      {
        gameObject.tag = tagJoueur2;
        Debug.Log("Tag changed to: " + gameObject.tag);
        spriteRenderer.sprite = balleRouge; // Change la couleur de la balle en rouge pour le joueur 2
       }
   }

   public void Joueur1Gagne() 
   {
    SceneManager.LoadScene("BleuGagne");
   }

    public void Joueur2Gagne() 
   {
    SceneManager.LoadScene("RougeGagne");
   }
}
