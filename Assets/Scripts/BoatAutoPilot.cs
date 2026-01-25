using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatAutoPilot : MonoBehaviour
{
    // This component fully defines how a boat behaves.
    // reference to a ScriptableObject so I can tweak behavior in Play mode
    // and keep the changes automatically, without touching the prefab itself.

    [SerializeField]
    private BoatBehaviorSettings behavior;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // On récupère notre vélocité initiale à partir de notre orientation dans le monde;
        velocity = transform.forward;
    }

    private void Update()
    {
        // detect nearby boats using a physics overlap sphere.
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            behavior.neighborhoodRadius
        );

        // extract BoatAutoPilot components from detected colliders.
        List<BoatAutoPilot> boats = colliders
            .Select(collider => collider.GetComponent<BoatAutoPilot>())
            .Where(b => b != null)
            .ToList();

        // remove from the list so it doesn't influence its own behavior.
        boats.Remove(this);

        Vector3 acceleration = ComputeAcceleration(boats);
        UpdateVelocity(acceleration);
        UpdatePosition(velocity);
        UpdateRotation(velocity);
    }

    private Vector3 ComputeAcceleration(IEnumerable<BoatAutoPilot> boats)
    {
        Vector3 acceleration = Vector3.zero;

        acceleration += ComputeAlignment(boats) * behavior.alignmentAmount;
        acceleration += ComputeSeparation(boats) * behavior.separationAmount;
        acceleration += ComputeCohesion(boats) * behavior.cohesionAmount;

        return acceleration;
    }

    private void UpdateVelocity(Vector3 acceleration)
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, behavior.maxSpeed);
    }

    private void UpdatePosition(Vector3 velocity)
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void UpdateRotation(Vector3 velocity)
    {
        //transform.forward = velocity;
        transform.forward = Vector3.RotateTowards(
            transform.forward,
            velocity,
            Time.deltaTime * behavior.steeringSpeed,
            float.MaxValue
        );
    }

    private Vector3 ComputeAlignment(IEnumerable<BoatAutoPilot> boats)
    {
        var velocitySum = Vector3.zero;
        if (!boats.Any()) return velocitySum;

        foreach (var boat in boats)
        {
            velocitySum += boat.velocity;
        }

        velocitySum /= boats.Count();
        return Steer(velocitySum.normalized * behavior.maxSpeed);
    }

    private Vector3 ComputeCohesion(IEnumerable<BoatAutoPilot> boats)
    {
        if (!boats.Any()) return Vector3.zero;

        var sumPositions = Vector3.zero;
        foreach (var boat in boats)
        {
            sumPositions += boat.transform.position;
        }

        var average = sumPositions / boats.Count();
        var direction = average - transform.position;
        return Steer(direction.normalized * behavior.maxSpeed);
    }

    private Vector3 ComputeSeparation(IEnumerable<BoatAutoPilot> boats)
    {
        var direction = Vector3.zero;

        boats = boats.Where(
            boat => Vector3.Distance(transform.position, boat.transform.position)
                    <= behavior.separationRadius
        );

        if (!boats.Any()) return direction;

        foreach (var boat in boats)
        {
            Vector3 difference = transform.position - boat.transform.position;
            direction += difference.normalized;
        }

        direction /= boats.Count();
        return Steer(direction.normalized * behavior.maxSpeed);
    }

    private Vector3 Steer(Vector3 desiredVelocity)
    {
        var steer = desiredVelocity - velocity;
        steer = LimitMagnitude(steer, behavior.maxForce);
        return steer;
    }

    private Vector3 LimitMagnitude(Vector3 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }

        return baseVector;
    }

    private void OnDrawGizmosSelected()
    {
        if (behavior == null) return;

        // Neighborhood radius.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, behavior.neighborhoodRadius);

        // Separation radius.
        Gizmos.color = Color.salmon;
        Gizmos.DrawWireSphere(transform.position, behavior.separationRadius);
    }
    // my brain is burning rn and i havent recovered from the shitty day i had at work yesterday yet
}
