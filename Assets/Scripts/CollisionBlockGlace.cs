using UnityEngine;

public class CollisionBlockGlace : MonoBehaviour
{
    public GameObject balle;

    private int NbFois = 0; //Le nombre de fois que la balle touche le block

    public Sprite blocInitialGlace;
    public Sprite blockDamage1Glace;
    public Sprite blockDamage2Glace;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Chercher le sprite renderer une seule fois dans le start au lieu du update
        spriteRenderer = GetComponent<SpriteRenderer>();

        //S'assurer que la sprite initiale est chargée en premier
        spriteRenderer.sprite = blocInitialGlace;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D infoCollision)
    {
        if (infoCollision.gameObject.CompareTag("Balle"))
        {
            //Le nombre de fois augmente de 1
            NbFois++;

            if (NbFois == 1)
            {
                //On change le sprite du blockInitial à blockDamage1
                spriteRenderer.sprite = blockDamage1Glace;
            }
            else if (NbFois == 2)
            {
                //On change le sprite du blockInitial à blockDamage1
                spriteRenderer.sprite = blockDamage2Glace;

            }
            else if (NbFois == 3)
            {
                //On change le sprite du blockInitial à blockDamage1
                Destroy(gameObject);

            }
        }
    }
}
