using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData {
    public int money;
    public List<Dices> dices = new List<Dices>();
}

[Serializable]
public class Dices {
    public string[] items = new string[6];

    // constructeur
    public Dices() {
        for (int i = 0; i < items.Length; i++) {
            items[i] = "_";
        }
    }

    public void SetItem(int index, string item) {
        items[index] = item;
    }
}

public static class SaveSystem {

    private static string SavePath => Application.persistentDataPath + "/gameSave.json";

    public static void SaveGameData(GameData gameData) {
        string json = JsonUtility.ToJson(gameData, true);
        System.IO.File.WriteAllText(SavePath, json);
    }

    public static GameData LoadGameData() {
        if (System.IO.File.Exists(SavePath)) {
            string json = System.IO.File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null; // Retourne des données de jeu vierges si le fichier n'existe pas
    }
}
