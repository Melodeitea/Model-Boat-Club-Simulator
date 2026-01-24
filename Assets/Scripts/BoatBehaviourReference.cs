using UnityEngine;

/// <summary>
/// Tiny component to attach to boat prefabs.
/// Exposes a reference to the `BoatBehaviourConfig` ScriptableObject.
/// </summary>
[DisallowMultipleComponent]
public class BoatBehaviourReference : MonoBehaviour
{
    [Header("Behaviour (ScriptableObject)")]
    [Tooltip("Reference to the ScriptableObject that contains shared behaviour parameters for this boat/family.")]
    [SerializeField]
    private BoatBehaviourConfig behaviourConfig = null;

    // Public read-only accessor for runtime scripts (BoatAutoPilot will use this).
    public BoatBehaviourConfig BehaviourConfig => behaviourConfig;
}
