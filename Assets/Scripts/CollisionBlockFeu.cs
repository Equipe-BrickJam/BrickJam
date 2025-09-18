using UnityEngine;

public class CollisionBlockFeu : MonoBehaviour
{
    public GameObject balle;

    private float compteur = 0; // Compter le nombre de fois que le block est touché

    private float NbFois; //Le nombre de fois que la balle touche le block

    public Sprite blocInitialFeu;
    public Sprite blockDamage1Feu;
    public Sprite blockDamage2Feu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NbFois = compteur;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onCollisonEnter2D(Collision2D infoCollision)
    {
        if (infoCollision.gameObject.name == "Balle")
        {
            //Le nombre de fois augmente de 1
            NbFois++;

            //La balle fait un rebond

        }
        if (NbFois == 1f)
        {
            //On change le sprite du blockInitial à blockDamage1
            GetComponent<SpriteRenderer>().sprite = blockDamage1Feu;

        }
        if (NbFois == 2f)
        {
            //On change le sprite du blockInitial à blockDamage1
            GetComponent<SpriteRenderer>().sprite = blockDamage2Feu;

        }
        if (NbFois == 3f)
        {
            //On change le sprite du blockInitial à blockDamage1
            Destroy(gameObject);

        }

    }
}
