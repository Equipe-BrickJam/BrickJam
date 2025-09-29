//using UnityEngine;
//using Unity.Netcode;
//using System.Collections.Generic;

//public class BlocManager : NetworkBehaviour
//{
//    public static BlocManager instance;

//    [Header("Préfabs des blocs")]
//    public GameObject prefabBlocGlace; // Pour le Joueur 1 (client)
//    public GameObject prefabBlocFeu;  // Pour le Joueur 2 (serveur)

//    // Nouvelles listes pour stocker les positions trouvées
//    private List<Vector3> positionsInitialesGlace = new List<Vector3>();
//    private List<Vector3> positionsInitialesFeu = new List<Vector3>();

//    private const string TAG_GLACE = "BlocGlace"; // Constante pour le tag Glace
//    private const string TAG_FEU = "BlocFeu";     // Constante pour le tag Feu

//    // ... (méthode Awake inchangée)

//    void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//            return;
//        }
//        Destroy(gameObject);
//    }

//    // écupérer les positions au lancement du Manager
//    void Start()
//    {
        
//        if (IsServer) // Seul le serveur a besoin de stocker ces positions pour la réinstanciation
//        {
//            EnregistrerPositionsInitiales();
//        }
//    }

//    private void EnregistrerPositionsInitiales()
//    {
//        // Trouver tous les blocs glace dans la scène
//        GameObject[] blocsGlace = GameObject.FindGameObjectsWithTag(TAG_GLACE);
//        foreach (GameObject bloc in blocsGlace)
//        {
//            positionsInitialesGlace.Add(bloc.transform.position);
//            // Détruire ces blocs initiaux si la 'NouvellePartie' les réinstancie
//            // bloc.GetComponent<NetworkObject>().Despawn(); 
//        }

//        // Trouver tous les blocs Feu dans la scène
//        GameObject[] blocsFeu = GameObject.FindGameObjectsWithTag(TAG_FEU);
//        foreach (GameObject bloc in blocsFeu)
//        {
//            positionsInitialesFeu.Add(bloc.transform.position);
//            // Détruire ces blocs initiaux
//            // bloc.GetComponent<NetworkObject>().Despawn(); 
//        }

//        Debug.Log($"Positions initiales enregistrées : Glace ({positionsInitialesGlace.Count}), Feu ({positionsInitialesFeu.Count})");
//    }


//    // Détruit localement, puis le serveur réinstancie en utilisant les positions enregistrées
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
//        // Réinstancier tous les blocs Glace
//        foreach (Vector3 pos in positionsInitialesGlace)
//        {
//            GameObject bloc = Instantiate(prefabBlocGlace, pos, Quaternion.identity);
//            bloc.GetComponent<NetworkObject>().Spawn();
//        }

//        // Réinstancier tous les blocs Feu
//        foreach (Vector3 pos in positionsInitialesFeu)
//        {
//            GameObject bloc = Instantiate(prefabBlocFeu, pos, Quaternion.identity);
//            bloc.GetComponent<NetworkObject>().Spawn();
//        }
//    }
//}