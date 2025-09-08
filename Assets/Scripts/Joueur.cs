using UnityEngine;
using UnityEngine.InputSystem;

public class Joueur : MonoBehaviour
{
    public float vitesseDeplacement;
    public float vitesseMaximale;

    private Vector2 velocitePerso;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().linearVelocity = velocitePerso;
        
        if (Input.GetKeyDown(KeyCode.D)) //Déplacement vers la droite
        {
            velocitePerso.x = vitesseDeplacement;
        }

        else if (Input.GetKeyDown(KeyCode.A)) //Déplacement vers la gauche
        {
            velocitePerso.x = -vitesseDeplacement;
        }

        else
        {
            velocitePerso.x = GetComponent<Rigidbody2D>().linearVelocity.x; //Vitesse actuelle X
        }
    }
}
