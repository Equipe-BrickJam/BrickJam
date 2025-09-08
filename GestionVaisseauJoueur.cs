using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GestionVaisseauJoueur : MonoBehaviour
{

    public float vitesseDeplacement;
    public float vitesseMaximale;

    public bool peutAppuyer = true;
    public bool powerUp = false;
    public bool viePerdu = false;
    public bool viePerdu2 = false;

    public GameObject laserOriginal;
    public GameObject etoileOriginal;
    public GameObject cercleLaser;
    public GameObject coeur1;
    public GameObject coeur2;
    public GameObject coeur3;
    public GameObject etoileVerif;

    private Vector2 velocitePerso;

    public AudioClip sonExplosion;
    public AudioClip sonShipShot;
    public AudioClip sonShipLaser;

    public TextMeshProUGUI textePointage;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DupliquerEtoile", 5, 15);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.D)) && peutAppuyer)  //Déplacement vers la droite
        {
            velocitePerso.x = vitesseDeplacement;
            GetComponent<Animator>().SetBool("Bouge", true);
        }

        else if ((Input.GetKey(KeyCode.A)) && peutAppuyer) //Déplacement vers la gauche
        {
            velocitePerso.x = -vitesseDeplacement;
            GetComponent<Animator>().SetBool("Bouge", true);
        }

        else
        {
            velocitePerso.x = GetComponent<Rigidbody2D>().velocity.x; //Vitesse actuelle X
            GetComponent<Animator>().SetBool("Bouge", false);
        }

        if ((Input.GetKey(KeyCode.W)) && peutAppuyer) //Déplacement vers le haut
        {
            velocitePerso.y = vitesseDeplacement;
        }

        else if ((Input.GetKey(KeyCode.S)) && peutAppuyer) //Déplacement vers le bas
        {
            velocitePerso.y = -vitesseDeplacement;
        }

        else
        {
            velocitePerso.y = GetComponent<Rigidbody2D>().velocity.y; //Vitesse actuelle en Y
        }

        GetComponent<Rigidbody2D>().velocity = velocitePerso;

        if ((Input.GetKeyDown(KeyCode.Space)) && peutAppuyer) //Vaisseau personnage tire des projectiles
        {
            GameObject laserCopie = Instantiate(laserOriginal);
            laserCopie.SetActive(true);
            laserCopie.transform.position = new Vector3(transform.position.x, transform.position.y);
            laserCopie.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 15);
            GetComponent<AudioSource>().PlayOneShot(sonShipShot);

        }

        else if ((Input.GetKeyDown(KeyCode.Return)) && powerUp) //Vaisseau personnage active une laser de cercle
        {
            cercleLaser.SetActive(true);
            Invoke("AnnulePowerUp", 2);
            GetComponent<AudioSource>().PlayOneShot(sonShipLaser);
            etoileVerif.SetActive(false);
        }


       

    }

    private void OnCollisionEnter2D(Collision2D infoCollision) //Collision entre le joueur et les obstacles
    {
        if (infoCollision.gameObject.tag == "Ennemis")
        {
            GetComponent<Animator>().SetBool("Explosion", true);
            GetComponent<AudioSource>().PlayOneShot(sonExplosion);
            coeur1.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            Invoke("Disparait", 1.5f);
            Laser.pointage -= 20;

            if(viePerdu == false)
            {
                viePerdu = true;
            }

            else
            {
                coeur2.SetActive(false);
                Invoke("Disparait", 1.5f);

                if (viePerdu2 == false)
                {
                    viePerdu2 = true;
                }

                else
                {
                    coeur3.SetActive(false);
                    Invoke("SceneDefaite", 0.5f);
                    peutAppuyer = false;
                }
            }

        }

        else
        {
            GetComponent<Animator>().SetBool("Explosion", false);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Etoile(Clone)")
        {
            powerUp = true;
            Destroy(collision.gameObject);
            etoileVerif.SetActive(true);
        }

        else if (collision.gameObject.name == "Etoile(Clone)" && powerUp)
        {
            Laser.pointage += 25;
        }

    }

    void DupliquerEtoile() //Duplique l'étoile power up
    {
        GameObject etoileCopie = Instantiate(etoileOriginal);
        etoileCopie.SetActive(true);
        etoileCopie.transform.position = new Vector2(Random.Range(transform.position.x - 8f, transform.position.x + 8f), Random.Range(transform.position.y + 10f, transform.position.y + 20f));
    }

    void AnnulePowerUp() //Désactive le laser
    {
        powerUp = false;
        cercleLaser.SetActive(false);
    }

    void Disparait() //Empêcher le vaisseau de se faire toucher une autre fois alors qu'il devrait être détruit
    {
        gameObject.SetActive(false);
        Invoke("Respawn", 1);
    }

    void Respawn() //Vaisseau revient à la normale
    {
        gameObject.SetActive(true);
        gameObject.transform.position = new Vector3(-0.37f, -3.47f);
        GetComponent<Collider2D>().enabled = true;
    }

    void SceneDefaite() //Changement de scène
    {
        SceneManager.LoadScene("Defeat");
        SceneManager.UnloadSceneAsync("MainGame");
    }
}
