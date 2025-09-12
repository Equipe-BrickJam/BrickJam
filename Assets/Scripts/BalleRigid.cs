using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;

public class BalleRigid : NetworkBehaviour
{
    public static BalleRigid instance;
    float maxDistanceX = 5f;
    private void Awake()
    {
        // Cr�ation d'un singleton pour avoir qu'une seule balle.
        if (instance == null)
        {
            instance = this;
            return;
        }
        Destroy(gameObject);

    }

    void Update()
    {
        if (!IsServer) return;

        if (!GameManager.instance.partieEnCours) return;

        //but client
        if (transform.position.x < -maxDistanceX)
        {
            LanceBalleMilieu();
        }

        //but serveur (h�te)
        if (transform.position.x > maxDistanceX)
        {
            LanceBalleMilieu();
        }
    }

    public void LanceBalleMilieu()
    {
        //On d�sactive l'interpolation du NetworkTransform pour �viter de voir l'interpolation de position de la balle
        GetComponent<NetworkTransform>().Interpolate = false;

        //On replace la balle au centre de la table et on remet � 0 sa v�locit�
        transform.position = new Vector3(0f, 0.5f, 0f);
        GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
        
        if (GameManager.instance.partieTerminee) return;

        //Lancement d'une coroutine qui replacera et relancera la balle seulement si la partie n'est pas termin�e
        StartCoroutine(NouvelleBalle());
    }

    IEnumerator NouvelleBalle()
    {
        yield return new WaitForSecondsRealtime(1f);
        GetComponent<NetworkTransform>().Interpolate = true;


        float aleaX = Random.Range(0, 2) == 0 ? -10 : 10; 
        float aleaZ = Random.Range(0, 2) == 0 ? -10 : 10;

        GetComponent<Rigidbody>().AddForce(aleaX, 0, aleaZ, ForceMode.Impulse);
    }

}
