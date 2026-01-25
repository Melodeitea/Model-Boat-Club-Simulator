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

    // Weighted prefab entries
    // lost some braincells there
    [System.Serializable]
    public struct BoatEntry
    {
        public GameObject prefab;
        public float weight; // define how likely this boat is to spawn relative to others
    }

    [SerializeField] private List<BoatEntry> boatPrefabs = new List<BoatEntry>();

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
        GameObject boatToInstantiate = GetRandomBoat();
        if (boatToInstantiate == null) return;

        GameObject boatInstance = Instantiate(boatToInstantiate, worldPosition, worldRotation, transform);
        boatsInstances.Add(boatInstance);
    }

    // Weighted random selection
    // see the braincells drop
    private GameObject GetRandomBoat()
    {
        if (boatPrefabs.Count == 0) return null;

        // Compute total weight
        float totalWeight = 0f;
        foreach (var entry in boatPrefabs) totalWeight += entry.weight;

        // Pick a random value in [0, totalWeight)
        float randomValue = Random.value * totalWeight;

        // iterate and find the first prefab where cumulative weight > randomValue
        float cumulative = 0f;
        foreach (var entry in boatPrefabs)
        {
            cumulative += entry.weight;
            if (randomValue <= cumulative) return entry.prefab;
        }

        // fallback
        return boatPrefabs[boatPrefabs.Count - 1].prefab;
    }

    private void LateUpdate()
    {
        BorderPatrol();
    }

    private void BorderPatrol()
    {
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
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0f, length));
    }

    // thank you stackoverflow and thank you unity documentations
    // #rtfm forever
}
