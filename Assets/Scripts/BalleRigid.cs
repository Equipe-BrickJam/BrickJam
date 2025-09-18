using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Components;// pour accéder aux propriétés du NetworkTransform

public class BalleRigid : NetworkBehaviour
{
    public static BalleRigid instance; // Singleton
    float maxDistanceX = 25f; // moitié de la largeur de la table, pour savoir si un but est compté
    [SerializeField] private float nombreDeBonds; //compte du nombre de bonds de la balle // Servira plus tard
    [SerializeField] private float maxSpeed; // si on veut limiter la vitesse max de la balle (inutilisé)

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

       System.Random random = new System.Random();
       float aleaX = random.Next(0, 2) == 0 ? -10 : 10; //  opérateur ternaire
       float aleaZ = random.Next(0, 2) == 0 ? -10 : 10;

       GetComponent<Rigidbody2D>().AddForce(new Vector2(aleaX, aleaZ), ForceMode2D.Impulse);

   }

   private void onCollisonEnter2D(Collision2D infoCollision) 
   {
       //Si elle rentre en collision avec un blouclier OU un block
       if(infoCollision.gameObject.tag == "Bouclier" || infoCollision.gameObject.tag == "Block")  
       {
           nombreDeBonds++;

       System.Random random = new System.Random();
       float aleaX = random.Next(0, 2) == 0 ? -10 : 10; //  opérateur ternaire
       float aleaZ = random.Next(0, 2) == 0 ? -10 : 10;

       GetComponent<Rigidbody2D>().AddForce(new Vector2(-aleaX, -aleaZ), ForceMode2D.Impulse);
       }

       

   }
}
