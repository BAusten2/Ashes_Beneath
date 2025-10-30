using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float[] position;
    
    // Flashlight data
    public bool hasFlashlight;
    public bool flashlightOn;
    public float batteryPercentage;
    
    // Battery inventory
    public float spareBatteries;
    
    // Collected items tracking
    public string[] collectedBatteryIDs;
}

public static class SaveManager
{
    private static string savePath => Application.persistentDataPath + "/save.json";
    private static HashSet<string> collectedBatteryIDs = new HashSet<string>();

    public static void RegisterCollectedBattery(string batteryID)
    {
        if (!string.IsNullOrEmpty(batteryID))
        {
            collectedBatteryIDs.Add(batteryID);
            Debug.Log($"[SaveManager] Registered collected battery: {batteryID}");
        }
    }

    public static void SavePlayer(Vector3 position)
    {
        SaveData data = new SaveData();
        data.position = new float[] { position.x, position.y, position.z };
        
        // Find and save flashlight data - only look for ACTIVE flashlight scripts
        FlashlightScript[] allFlashlights = GameObject.FindObjectsOfType<FlashlightScript>();
        FlashlightScript flashlight = null;
        
        // Find the active, picked-up flashlight
        foreach (FlashlightScript fs in allFlashlights)
        {
            if (fs.gameObject.activeInHierarchy && fs.PickedFlashlight)
            {
                flashlight = fs;
                break;
            }
        }
        
        if (flashlight != null)
        {
            data.hasFlashlight = flashlight.PickedFlashlight;
            data.flashlightOn = flashlight.on;
            data.batteryPercentage = Mathf.Clamp(flashlight.batteryPercentage, 0f, 100f);
            
            Debug.Log($"[SaveManager] Saved flashlight: On={data.flashlightOn}, Battery={data.batteryPercentage}%, GameObject={flashlight.gameObject.name}");
        }
        else
        {
            data.hasFlashlight = false;
            data.flashlightOn = false;
            data.batteryPercentage = 100f;
            Debug.Log($"[SaveManager] No flashlight picked up yet");
        }
        
        // Find and save battery inventory
        BatteryUI batteryUI = GameObject.FindObjectOfType<BatteryUI>();
        if (batteryUI != null)
        {
            data.spareBatteries = batteryUI.Batteries;
        }
        
        // Save collected battery IDs
        data.collectedBatteryIDs = new string[collectedBatteryIDs.Count];
        collectedBatteryIDs.CopyTo(data.collectedBatteryIDs);
        
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[SaveManager] Game saved! Collected batteries: {collectedBatteryIDs.Count}");
        Debug.Log($"[SaveManager] Save file: {savePath}");
    }

    public static SaveData LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            // Restore collected battery IDs
            collectedBatteryIDs.Clear();
            if (data.collectedBatteryIDs != null)
            {
                foreach (string id in data.collectedBatteryIDs)
                {
                    collectedBatteryIDs.Add(id);
                }
            }
            
            Debug.Log($"[SaveManager] Loaded save data: HasFlashlight={data.hasFlashlight}, On={data.flashlightOn}, Battery={data.batteryPercentage}%");
            
            return data;
        }
        return null;
    }

    public static Vector3? LoadPlayer()
    {
        SaveData data = LoadPlayerData();
        if (data != null)
        {
            return new Vector3(data.position[0], data.position[1], data.position[2]);
        }
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("[SaveManager] Save file deleted");
        }
        collectedBatteryIDs.Clear();
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
    
    public static void ClearCollectedItems()
    {
        collectedBatteryIDs.Clear();
    }
}