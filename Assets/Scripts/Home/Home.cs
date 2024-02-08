using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{

    public void btnplay()
    {
        SceneManager.LoadScene("Game");
    }

    public void btnInv()
    {
        SceneManager.LoadScene("Customisation");
    }

    void Start()
    {   
        // Si aucune gameData n'est sauvegardée, on en crée une
        if (SaveSystem.LoadGameData() == null)
        {
            GameData gameData = new GameData();
            gameData.money = 100;

            Dices defaultDice = new Dices();
            defaultDice.SetItem(0, "att_1");
            defaultDice.SetItem(1, "att_1");
            defaultDice.SetItem(3, "def_1");
            defaultDice.SetItem(4, "def_1");
            gameData.dices.Add(defaultDice);
            gameData.dices.Add(defaultDice);

            SaveSystem.SaveGameData(gameData);
        }
    }

}
