using System.Collections.Generic;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    private static BoatManager singleton = null;

    public static BoatManager Singleton
    {
        get => singleton;
        private set => singleton = value;
    }

    [SerializeField] private float width = 16f;
    [SerializeField] private float length = 9f;
    [SerializeField] private int SpawningCount = 20;

    // replaced hard-coded references with a list of prefabs
    // can now add/remove variants in the Inspector without touching the code
    [SerializeField] private List<GameObject> boatPrefabs = new List<GameObject>();

    private List<GameObject> boatsInstances = new List<GameObject>();

    private void Awake()
    {
        singleton = this;
    }

    private void OnDestroy()
    {
        if (singleton == this) singleton = null;
    }

    private void Start()
    {
        // On génère un nombre de bateau au départ
        for (int i = 0; i < SpawningCount; i++)
        {
            // On choisi une position et une orientation au hasard dans la zone de jeu
            Vector3 randomPosition = new Vector3(
                (Random.value - 0.5f) * width,
                0f,
                (Random.value - 0.5f) * length
            );

            Quaternion randomRotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
            SpawnBoat(randomPosition, randomRotation);
        }
    }

    private void SpawnBoat(Vector3 worldPosition, Quaternion worldRotation)
    {
        // pick a random prefab from the list instead of hard-coded A/B/C
        GameObject boatToInstantiate = GetRandomBoat();
        if (boatToInstantiate == null) return;

        // I instantiate the boat as a child of BoatManager
        GameObject boatInstance = Instantiate(boatToInstantiate, worldPosition, worldRotation, transform);

        // add it to my instances list for border patrol
        boatsInstances.Add(boatInstance);
    }

    // select a random prefab dynamically from the list
    private GameObject GetRandomBoat()
    {
        if (boatPrefabs.Count == 0) return null;

        int index = Random.Range(0, boatPrefabs.Count);
        return boatPrefabs[index];
    }

    private void LateUpdate()
    {
        BorderPatrol();
    }

    private void BorderPatrol()
    {
        // On vérifie que nos bateaux sont dans la zone de jeu
        // On les téléporte au côté opposé s'ils en sortent
        for (int i = 0; i < boatsInstances.Count; i++)
        {
            GameObject boatInstance = boatsInstances[i];
            Vector3 localPosition = boatInstance.transform.localPosition;
            bool positionHasChanged = false;

            if (localPosition.x < -width * 0.5f)
            {
                localPosition.x += width;
                positionHasChanged = true;
            }
            else if (localPosition.x > width * 0.5f)
            {
                localPosition.x -= width;
                positionHasChanged = true;
            }

            if (localPosition.z > length * 0.5f)
            {
                localPosition.z -= length;
                positionHasChanged = true;
            }
            else if (localPosition.z < -length * 0.5f)
            {
                localPosition.z += length;
                positionHasChanged = true;
            }

            if (positionHasChanged)
                boatInstance.transform.localPosition = localPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Draw top border
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0f, length));
    }
}
