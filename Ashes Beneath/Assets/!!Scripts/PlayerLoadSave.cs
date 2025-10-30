using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLoadSave : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public GameObject flashlightObject;
    public GameObject flashlightUIPanel;
    public GameObject flashlightPickupObject;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private FlashlightScript flashlightScript;
    private BatteryUI batteryUI;
    
    void Start()
    {
        int shouldLoad = PlayerPrefs.GetInt("ShouldLoadSave", 0);
        
        if (showDebugLogs)
        {
            Debug.Log($"[PlayerLoadSave] ShouldLoadSave flag: {shouldLoad}");
            Debug.Log($"[PlayerLoadSave] Save exists: {SaveManager.SaveExists()}");
        }
        
        if (shouldLoad == 1 && SaveManager.SaveExists())
        {
            // Small delay to ensure scene is fully loaded
            StartCoroutine(LoadGameDelayed());
            PlayerPrefs.SetInt("ShouldLoadSave", 0);
            PlayerPrefs.Save();
        }
        else
        {
            if (showDebugLogs) Debug.Log("[PlayerLoadSave] Starting new game");
        }
    }
    
    IEnumerator LoadGameDelayed()
    {
        // Wait one frame to ensure all objects are initialized
        yield return new WaitForEndOfFrame();
        LoadGame();
    }
    
    void LoadGame()
    {
        SaveData data = SaveManager.LoadPlayerData();
        
        if (data == null)
        {
            Debug.LogWarning("[PlayerLoadSave] No save data found!");
            return;
        }
        
        if (showDebugLogs)
        {
            Debug.Log("[PlayerLoadSave] === LOADING GAME ===");
            Debug.Log($"Has Flashlight: {data.hasFlashlight}");
            Debug.Log($"Flashlight On: {data.flashlightOn}");
            Debug.Log($"Battery %: {data.batteryPercentage}");
            Debug.Log($"Spare Batteries: {data.spareBatteries}");
        }
        
        // Load player position
        if (playerTransform != null && data.position != null && data.position.Length == 3)
        {
            Vector3 loadedPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            
            CharacterController controller = playerTransform.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                playerTransform.position = loadedPosition;
                controller.enabled = true;
            }
            else
            {
                playerTransform.position = loadedPosition;
            }
            
            if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Player position loaded");
        }
        
        // IMPORTANT: Remove pickup FIRST before loading flashlight data
        if (data.hasFlashlight)
        {
            RemoveFlashlightPickup();
        }
        
        LoadFlashlightData(data);
        LoadBatteryData(data);
        LoadCollectedItems(data);
        
        Debug.Log("[PlayerLoadSave] === GAME LOADED ===");
    }
    
    void LoadFlashlightData(SaveData data)
    {
        // Find the flashlight script if not assigned
        if (flashlightScript == null)
        {
            flashlightScript = FindObjectOfType<FlashlightScript>();
        }
        
        if (flashlightScript == null)
        {
            Debug.LogError("[PlayerLoadSave] FlashlightScript not found!");
            return;
        }
        
        if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Found FlashlightScript on: {flashlightScript.gameObject.name}");
        
        if (data.hasFlashlight)
        {
            // Find or use assigned flashlight object
            if (flashlightObject == null)
            {
                flashlightObject = flashlightScript.gameObject;
            }
            
            // Activate flashlight GameObject
            flashlightObject.SetActive(true);
            
            // Activate UI
            if (flashlightUIPanel != null)
            {
                flashlightUIPanel.SetActive(true);
            }
            
            // Set flashlight pickup status FIRST
            flashlightScript.PickedFlashlight = true;
            
            // Set battery percentage
            flashlightScript.batteryPercentage = Mathf.Clamp(data.batteryPercentage, 0f, 100f);
            
            // CRITICAL: Set the on/off state
            flashlightScript.on = data.flashlightOn;
            
            // Find the Light component
            Light flashlightLight = flashlightScript.GetComponent<Light>();
            if (flashlightLight == null)
            {
                flashlightLight = flashlightScript.GetComponentInChildren<Light>();
            }
            
            if (flashlightLight != null)
            {
                // Force the light to match the saved state
                flashlightLight.enabled = data.flashlightOn;
                if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Light component set to: {data.flashlightOn}");
            }
            else
            {
                Debug.LogWarning("[PlayerLoadSave] Could not find Light component on flashlight!");
            }
            
            // Force update the battery UI sprite immediately
            StartCoroutine(UpdateBatteryUIDelayed(data.batteryPercentage));
            
            if (showDebugLogs)
            {
                Debug.Log($"[PlayerLoadSave] Flashlight loaded:");
                Debug.Log($"  - GameObject: {flashlightObject.name}");
                Debug.Log($"  - Picked: {flashlightScript.PickedFlashlight}");
                Debug.Log($"  - On: {flashlightScript.on}");
                Debug.Log($"  - Battery: {flashlightScript.batteryPercentage}%");
                Debug.Log($"  - Light enabled: {(flashlightLight != null ? flashlightLight.enabled.ToString() : "null")}");
            }
        }
        else
        {
            // Player doesn't have flashlight yet
            if (flashlightObject != null) flashlightObject.SetActive(false);
            if (flashlightUIPanel != null) flashlightUIPanel.SetActive(false);
            flashlightScript.PickedFlashlight = false;
            
            if (showDebugLogs) Debug.Log("[PlayerLoadSave] Player doesn't have flashlight yet");
        }
    }
    
    IEnumerator UpdateBatteryUIDelayed(float batteryPercentage)
    {
        // Wait a frame for UI to initialize
        yield return null;
        
        // Force update the battery sprite
        if (flashlightScript != null && flashlightScript.FlashlightSprite != null)
        {
            Image batterySprite = flashlightScript.FlashlightSprite.GetComponent<Image>();
            if (batterySprite != null)
            {
                // Manually set the sprite based on battery percentage
                BatterySpritesClass sprites = flashlightScript.BatterySprites;
                float barPercentage = 100f / 19f;
                
                if (batteryPercentage > 18 * barPercentage)
                    batterySprite.sprite = sprites.battery_full;
                else if (batteryPercentage > 17 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_18;
                else if (batteryPercentage > 16 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_17;
                else if (batteryPercentage > 15 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_16;
                else if (batteryPercentage > 14 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_15;
                else if (batteryPercentage > 13 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_14;
                else if (batteryPercentage > 12 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_13;
                else if (batteryPercentage > 11 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_12;
                else if (batteryPercentage > 10 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_11;
                else if (batteryPercentage > 9 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_10;
                else if (batteryPercentage > 8 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_9;
                else if (batteryPercentage > 7 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_8;
                else if (batteryPercentage > 6 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_7;
                else if (batteryPercentage > 5 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_6;
                else if (batteryPercentage > 4 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_5;
                else if (batteryPercentage > 3 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_4;
                else if (batteryPercentage > 2 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_3;
                else if (batteryPercentage > 1 * barPercentage)
                    batterySprite.sprite = sprites.battery_charge_2;
                else if (batteryPercentage > 0.5f)
                    batterySprite.sprite = sprites.battery_charge_1;
                else
                    batterySprite.sprite = sprites.battery_charge_0;
                
                if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Battery UI sprite updated to match {batteryPercentage}%");
            }
        }
    }
    
    void RemoveFlashlightPickup()
    {
        bool removed = false;
        
        // Try assigned reference first
        if (flashlightPickupObject != null)
        {
            if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Destroying assigned pickup: {flashlightPickupObject.name}");
            Destroy(flashlightPickupObject);
            flashlightPickupObject = null; // Clear reference
            removed = true;
        }
        
        // Try to find by component
        if (!removed)
        {
            FlashlightPickup[] pickups = FindObjectsOfType<FlashlightPickup>();
            if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Found {pickups.Length} FlashlightPickup components");
            
            foreach (FlashlightPickup pickup in pickups)
            {
                if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Destroying pickup: {pickup.gameObject.name}");
                Destroy(pickup.gameObject);
                removed = true;
            }
        }
        
        if (removed && showDebugLogs)
        {
            Debug.Log("[PlayerLoadSave] Flashlight pickup removed from world");
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("[PlayerLoadSave] No flashlight pickup found to remove");
        }
    }
    
    void LoadBatteryData(SaveData data)
    {
        if (batteryUI == null)
        {
            batteryUI = FindObjectOfType<BatteryUI>();
        }
        
        if (batteryUI != null)
        {
            batteryUI.Batteries = data.spareBatteries;
            if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Batteries loaded: {data.spareBatteries * 100}");
        }
    }
    
    void LoadCollectedItems(SaveData data)
    {
        if (data.collectedBatteryIDs != null && data.collectedBatteryIDs.Length > 0)
        {
            BatteryPickup[] allBatteries = FindObjectsOfType<BatteryPickup>();
            int removedCount = 0;
            
            foreach (BatteryPickup battery in allBatteries)
            {
                CollectibleID idComponent = battery.GetComponent<CollectibleID>();
                if (idComponent != null)
                {
                    foreach (string collectedID in data.collectedBatteryIDs)
                    {
                        if (idComponent.uniqueID == collectedID)
                        {
                            Destroy(battery.gameObject);
                            removedCount++;
                            break;
                        }
                    }
                }
            }
            
            if (showDebugLogs) Debug.Log($"[PlayerLoadSave] Removed {removedCount} collected batteries");
        }
    }
}