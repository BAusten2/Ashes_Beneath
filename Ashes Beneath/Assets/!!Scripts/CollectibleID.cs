using UnityEngine;

/// <summary>
/// Add this component to any pickup item (battery, key, etc.) to track if it's been collected.
/// Each item needs a unique ID to be tracked across save/load.
/// </summary>
public class CollectibleID : MonoBehaviour
{
    [Header("Unique ID")]
    [Tooltip("Must be unique for each collectible in the scene!")]
    public string uniqueID = "";
    
    [Header("Auto-Generate")]
    [Tooltip("Click this button in the Inspector to auto-generate a unique ID")]
    public bool generateNewID = false;
    
    void OnValidate()
    {
        // Auto-generate ID in editor if requested
        if (generateNewID)
        {
            uniqueID = System.Guid.NewGuid().ToString();
            generateNewID = false;
            Debug.Log($"Generated new ID for {gameObject.name}: {uniqueID}");
        }
        
        // Warn if ID is empty
        if (string.IsNullOrEmpty(uniqueID))
        {
            Debug.LogWarning($"CollectibleID on {gameObject.name} has no unique ID! Click 'Generate New ID' in Inspector.");
        }
    }
    
    void Start()
    {
        // Generate ID at runtime if still empty
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = $"{gameObject.name}_{transform.position.x}_{transform.position.y}_{transform.position.z}";
            Debug.LogWarning($"Auto-generated ID for {gameObject.name}: {uniqueID}");
        }
    }
}