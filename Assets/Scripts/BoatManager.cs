using System.Collections.Generic;
using UnityEngine;

/*
•	Is a singleton that spawns boats, keeps a list of instances, and teleports boats across borders.
•	Right now it holds specific prefab fields (boatHouseA, boatHouseB, boatHouseC) and a bunch of editable parameters (maxSpeed, steeringSpeed, etc.).
•	Problems you need to solve from the README:
•	Runtime edits of behavior aren’t saved easily.
•	Prefab setup is repetitive (must scale child model every time).
•	Prefab references are hardcoded (A/B/C fields), which doesn’t scale.
•	Random selection of prefabs is simplistic (equal chance and if/else structure).
*/

public class BoatManager : MonoBehaviour
{
    private static BoatManager singleton = null;

    public static BoatManager Singleton
    {
        get
        {
            return singleton;
        }

        private set
        {
            singleton = value;
        }
    }

    [SerializeField]
    private float width = 16f;

    [SerializeField]
    private float length = 9f;

    [Range(0, 300)]
    [SerializeField]
    private int SpawningCount;

    [SerializeField]
    private GameObject boatHouseA = null;

    [SerializeField]
    private GameObject boatHouseB = null;

    [SerializeField]
    private GameObject boatHouseC = null;

    

    private List<GameObject> boatsInstances = new List<GameObject>();

    private void Awake()
    {
        singleton = this;
    }

    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    private void Start()
    {
        // On génère un nombre de bateau au départ.
        for (int i = 0; i < SpawningCount; i++)
        {
            // On choisi une position et une orientation au hasard dans la zone de jeu.
            Vector3 randomPosition = new Vector3((Random.value - 0.5f) * width, 0f, (Random.value - 0.5f) * length);
            Quaternion randomRotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
            SpawnBoat(randomPosition, randomRotation);
        }
    }

    private void SpawnBoat(Vector3 worldPosition, Quaternion worldRotation)
    {
        // Le bateau qu'on va instancier
        GameObject boatToInstanciate = GetRandomBoat();

        // Créer une instance (qu'on attache directement en enfant de notre transform)
        GameObject boatInstance = Instantiate(boatToInstanciate, worldPosition, worldRotation, transform);

        // Rajouter cette instance à notre liste d'instances
        boatsInstances.Add(boatInstance);
    }

    private GameObject GetRandomBoat()
    {
        GameObject randomBoat = null;

        // On prend une variable aléatoire entre 0.000 et 1.000
        float randomValue = Random.value;
        if (randomValue < 0.333f)
        {
            randomBoat = boatHouseA;
        }
        else if (randomValue < 0.666f)
        {
            randomBoat = boatHouseB;
        }
        else
        {
            randomBoat = boatHouseC;
        }
        // TODO Je ne vais pas rajouter un "if..else" pour chaque nouveau bateau ?!
        // Il devrait y avoir un moyen de réunir mes Prefab dans une liste et
        // d'en choisir un selon sa place dans la liste...

        return randomBoat;
    }

    private void LateUpdate()
    {
        BorderPatrol();
    }

    private void BorderPatrol()
    {
        // On vérifie que nos bateaux sont dans la zone de jeu
        // On les téléporte au côté opposé s'ils en sortent
        int boatCount = boatsInstances.Count;
        for (int i = 0; i < boatCount; i++)
        {
            GameObject boatInstance = boatsInstances[i];
            Vector3 localPosition = boatInstance.transform.localPosition;
            bool positionHasChanged = false;

            // Left border?
            if (localPosition.x < -width * 0.5f)
            {
                localPosition.x += width;
                positionHasChanged = true;
            }
            // Right border?
            else if (localPosition.x > width * 0.5f)
            {
                localPosition.x -= width;
                positionHasChanged = true;
            }

            // Top border?
            if (localPosition.z > length * 0.5f)
            {
                localPosition.z -= length;
                positionHasChanged = true;
            }
            // Bottom border?
            else if (localPosition.z < -length * 0.5f)
            {
                localPosition.z += length;
                positionHasChanged = true;
            }

            if (positionHasChanged)
            {
                boatInstance.transform.localPosition = localPosition;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Draw top border
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0f, length));
    }
}