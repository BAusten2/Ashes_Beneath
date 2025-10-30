//Jasmit Gosal 21137879

using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float[] position;
}

public static class SaveManager
{
    private static string savePath => Application.persistentDataPath + "/save.json";

    //save player location
    public static void SavePlayer(Vector3 position)
    {
        SaveData data = new SaveData();
        data.position = new float[] { position.x, position.y, position.z };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);
    }

    //load player position
    public static Vector3? LoadPlayer()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return new Vector3(data.position[0], data.position[1], data.position[2]);
        }
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}
