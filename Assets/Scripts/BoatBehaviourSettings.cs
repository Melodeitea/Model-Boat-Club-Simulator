using UnityEngine;

[CreateAssetMenu(
    fileName = "BoatBehaviorSettings",
    menuName = "Model Boat Club / Boat Behavior Settings"
)]
public class BoatBehaviorSettings : ScriptableObject
{
    // I use this ScriptableObject to store all behavior parameters.
    // This lets me tweak values in Play mode and keep them saved automatically.
    // Boats will reference this asset directly instead of storing their own values.

    [Header("Movement")]
    [Range(0, 10)]
    public float maxSpeed = 6f;

    [Range(0.1f, 45f)]
    public float steeringSpeed = 4.5f;

    [Range(.01f, .5f)]
    public float maxForce = .03f;

    [Header("Flocking")]
    [Range(1, 10)]
    public float neighborhoodRadius = 4f;

    [Range(0.1f, 10f)]
    public float separationRadius = 2.4f;

    [Range(0, 3)]
    public float separationAmount = 1.1f;

    [Range(0, 3)]
    public float cohesionAmount = 0.3f;

    [Range(0, 3)]
    public float alignmentAmount = 0.5f;
}
