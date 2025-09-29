//using UnityEngine;
//using Unity.Netcode;
//using System.Collections.Generic;

//public class BlocManager : NetworkBehaviour
//{
//    public static BlocManager instance;

//    [Header("Pr�fabs des blocs")]
//    public GameObject prefabBlocGlace; // Pour le Joueur 1 (client)
//    public GameObject prefabBlocFeu;  // Pour le Joueur 2 (serveur)

//    // Nouvelles listes pour stocker les positions trouv�es
//    private List<Vector3> positionsInitialesGlace = new List<Vector3>();
//    private List<Vector3> positionsInitialesFeu = new List<Vector3>();

//    private const string TAG_GLACE = "BlocGlace"; // Constante pour le tag Glace
//    private const string TAG_FEU = "BlocFeu";     // Constante pour le tag Feu

//    // ... (m�thode Awake inchang�e)

//    void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//            return;
//        }
//        Destroy(gameObject);
//    }

//    // �cup�rer les positions au lancement du Manager
//    void Start()
//    {
        
//        if (IsServer) // Seul le serveur a besoin de stocker ces positions pour la r�instanciation
//        {
//            EnregistrerPositionsInitiales();
//        }
//    }

//    private void EnregistrerPositionsInitiales()
//    {
//        // Trouver tous les blocs glace dans la sc�ne
//        GameObject[] blocsGlace = GameObject.FindGameObjectsWithTag(TAG_GLACE);
//        foreach (GameObject bloc in blocsGlace)
//        {
//            positionsInitialesGlace.Add(bloc.transform.position);
//            // D�truire ces blocs initiaux si la 'NouvellePartie' les r�instancie
//            // bloc.GetComponent<NetworkObject>().Despawn(); 
//        }

//        // Trouver tous les blocs Feu dans la sc�ne
//        GameObject[] blocsFeu = GameObject.FindGameObjectsWithTag(TAG_FEU);
//        foreach (GameObject bloc in blocsFeu)
//        {
//            positionsInitialesFeu.Add(bloc.transform.position);
//            // D�truire ces blocs initiaux
//            // bloc.GetComponent<NetworkObject>().Despawn(); 
//        }

//        Debug.Log($"Positions initiales enregistr�es : Glace ({positionsInitialesGlace.Count}), Feu ({positionsInitialesFeu.Count})");
//    }


//    // D�truit localement, puis le serveur r�instancie en utilisant les positions enregistr�es
//    [ClientRpc]
//    public void ReinitialiserBlocsClientRpc()
//    {
//        DetruireBlocsLocaux();

//        if (IsHost)
//        {
//            ReinstancierBlocsSurServeur();
//        }
//    }

//    private void DetruireBlocsLocaux()
//    {
//        GameObject[] blocsGlace = GameObject.FindGameObjectsWithTag(TAG_GLACE);
//        foreach (GameObject bloc in blocsGlace)
//        {
//            Destroy(bloc);
//        }

//        GameObject[] blocsFeu = GameObject.FindGameObjectsWithTag(TAG_FEU);
//        foreach (GameObject bloc in blocsFeu)
//        {
//            Destroy(bloc);
//        }
//    }

//    private void ReinstancierBlocsSurServeur()
//    {
//        // R�instancier tous les blocs Glace
//        foreach (Vector3 pos in positionsInitialesGlace)
//        {
//            GameObject bloc = Instantiate(prefabBlocGlace, pos, Quaternion.identity);
//            bloc.GetComponent<NetworkObject>().Spawn();
//        }

//        // R�instancier tous les blocs Feu
//        foreach (Vector3 pos in positionsInitialesFeu)
//        {
//            GameObject bloc = Instantiate(prefabBlocFeu, pos, Quaternion.identity);
//            bloc.GetComponent<NetworkObject>().Spawn();
//        }
//    }
//}