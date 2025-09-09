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
        Mouvement();
    }

    private void Mouvement() 
    {
        if (Input.GetKey(KeyCode.D)) //Déplacement vers la droite
        {
            velocitePerso.x = vitesseDeplacement;
        }

        else if (Input.GetKey(KeyCode.A)) //Déplacement vers la gauche
        {
            velocitePerso.x = -vitesseDeplacement;
        }

        else
        {
            velocitePerso.x = 0f;
        }
    }
}
