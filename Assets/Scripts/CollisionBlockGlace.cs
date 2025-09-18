using UnityEngine;

public class CollisionBlockGlace : MonoBehaviour
{
    public GameObject balle;

    private int NbFois = 0; //Le nombre de fois que la balle touche le block

    public Sprite blocInitialGlace;
    public Sprite blockDamage1Glace;
    public Sprite blockDamage2Glace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
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
            GetComponent<SpriteRenderer>().sprite = blockDamage1Glace;

        }
        if (NbFois == 2f)
        {
            //On change le sprite du blockInitial à blockDamage1
            GetComponent<SpriteRenderer>().sprite = blockDamage2Glace;

        }
        if (NbFois == 3f)
        {
            //On change le sprite du blockInitial à blockDamage1
            Destroy(gameObject);

        }

    }
}
