using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private View view; // Référence à la vue
    [SerializeField] private GameObject dicePrefab; // Liste des dés
    
    public int nbIA = 3; // Nombre d'IA
    public int nbPlayer = 2; // Nombre dés joueurs

    // Liste des joueurs
    public List<Dice> dices = new List<Dice>();
    public int playerTurn = 0;
    private int[,] map = new int[10,10];

    private bool endGame = false;
    public Transform camera;
    public Text endText;
    public GameObject endPanel;



    // Quand le joueur charge une partie (initialisation)
    public void initGame(){
        // Générer la map
        GenerateMap(1);
        // Générer les dés
        GenerateDice();
        camera.position = new Vector3(map.GetLength(0)/2, map.GetLength(1)/2, -10);
    }


    // Génère les dés
    // Place les dés sur la map
    // Les affiche dans la vue
    private void GenerateDice(){

        // Génère les joueurs
        for (int i = 0; i < nbPlayer; i++)
        {
            // Créer un dé
            GameObject dice = Instantiate(dicePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // Changer le nom du dé
            dice.name = "P_" + i;

            // Trouver une case vide non occupée
            int x = 0, y = 0;
            bool isOccupied;
            do
            {
                isOccupied = false;
                x = Random.Range(0, map.GetLength(0));
                y = Random.Range(0, map.GetLength(1));
                
                // Vérifier si la case est vide et non occupée
                if (map[x, y] != 1)
                {
                    isOccupied = true;
                    continue;
                }

                // Vérifier si un autre dé occupe déjà cette position
                foreach (Dice d in dices)
                {
                    if (d.posX == x && d.posY == y)
                    {
                        isOccupied = true;
                        break;
                    }
                }
            } while (isOccupied);

            // gérer les attributs de la classe Dice
            dice.GetComponent<Dice>().posX = x;
            dice.GetComponent<Dice>().posY = y;       
            dice.GetComponent<Dice>().isAI = false;     

            // Ajouter le dé à la liste des joueurs
            dices.Add(dice.GetComponent<Dice>());
        }

        // Génère les IA
        for (int i = 0; i < nbIA; i++)
        {
            // Créer un dé
            GameObject dice = Instantiate(dicePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // Changer le nom du dé
            dice.name = "AI_" + i;

            // Trouver une case vide non occupée
            int x = 0, y = 0;
            bool isOccupied;
            do
            {
                isOccupied = false;
                x = Random.Range(0, map.GetLength(0));
                y = Random.Range(0, map.GetLength(1));
                
                // Vérifier si la case est vide et non occupée
                if (map[x, y] != 1)
                {
                    isOccupied = true;
                    continue;
                }

                // Vérifier si un autre dé occupe déjà cette position
                foreach (Dice d in dices)
                {
                    if (d.posX == x && d.posY == y)
                    {
                        isOccupied = true;
                        break;
                    }
                }
            } while (isOccupied);

            // gérer les attributs de la classe Dice
            dice.GetComponent<Dice>().posX = x;
            dice.GetComponent<Dice>().posY = y;       
            dice.GetComponent<Dice>().isAI = true;     

            // Ajouter le dé à la liste des joueurs
            dices.Add(dice.GetComponent<Dice>());
        }


        view.PlaceDices(dices);
    }


    // Génère une map 
    // Affiche la map dans la vue
    private void GenerateMap(int id = 1){
        if (id == 1){

            map = new int[,]
            {
                {1, 1, 1, 0, 1, 1, 1, 0, 1, 1},
                {1, 0, 1, 1, 1, 0, 1, 1, 1, 1},
                {1, 1, 0, 1, 1, 1, 0, 1, 1, 1},
                {1, 1, 1, 1, 0, 1, 1, 1, 0, 1},
                {0, 1, 1, 1, 1, 1, 1, 0, 1, 1},
                {1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
                {1, 1, 1, 0, 1, 1, 1, 1, 0, 1},
                {1, 1, 1, 1, 1, 0, 1, 1, 1, 1},
                {1, 0, 1, 1, 1, 1, 1, 0, 1, 1},
                {1, 1, 1, 1, 0, 1, 1, 1, 1, 1}
            };
        } else if (id == 2){
            map = new int[,] {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
                {1, 0, 0, 1, 1, 1, 0, 0, 1, 1}, 
                {1, 1, 0, 0, 1, 1, 0, 0, 1, 1}, 
                {1, 1, 1, 0, 0, 1, 1, 1, 1, 1}, 
                {1, 1, 1, 1, 0, 0, 1, 0, 0, 1}, 
                {1, 0, 1, 1, 1, 1, 1, 0, 0, 1}, 
                {1, 0, 0, 1, 1, 0, 1, 1, 1, 1}, 
                {1, 1, 0, 0, 1, 1, 0, 1, 1, 1}, 
                {1, 1, 1, 1, 1, 1, 0, 0, 1, 1}, 
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            };
        }

        // Afficher la map dans la vue
        view.PlaceTiles(map);
    }




    private bool isFree(int x, int y){
        // Vérifier si la case est dans la map
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            return false;
        }

        // Vérifier si la case est vide
        if (map[x, y] != 1)
        {
            return false;
        }

        // Vérifier si un autre dé occupe déjà cette position
        foreach (Dice d in dices)
        {
            if (d.posX == x && d.posY == y)
            {
                return false;
            }
        }

        return true;
    }


    void Update(){
        CheckEnd();
        if (endGame) return;
        // check si un element de la liste est en missing ou null
        for (int i = 0; i < dices.Count; i++)
        {
            if (dices[i] == null)
            {
                dices.RemoveAt(i);
                if (playerTurn >= dices.Count)
                {
                    playerTurn = 0;
                }
                return;
            }
        }


        // GESTION DES TOURS
        if (dices[playerTurn].isDead) {
            return;
        }

        // lister les actions possibles du dices[playerTurn]
        string[] actions = new string[8];
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = "";
        }
        int x = dices[playerTurn].posX;
        int y = dices[playerTurn].posY;
        if (isFree(x, y+1)) actions[0] = "0;1";
        if (isFree(x+1, y+1)) actions[1] = "1;1";
        if (isFree(x+1, y)) actions[2] = "1;0";
        if (isFree(x+1, y-1)) actions[3] = "1;-1";
        if (isFree(x, y-1)) actions[4] = "0;-1";
        if (isFree(x-1, y-1)) actions[5] = "-1;-1";
        if (isFree(x-1, y)) actions[6] = "-1;0";
        if (isFree(x-1, y+1)) actions[7] = "-1;1";

        if (dices[playerTurn].isAI) {
            if (!dices[playerTurn].isTurn && !dices[playerTurn].hasPlayed){
                dices[playerTurn].isTurn = true;
                dices[playerTurn].turn(actions, dices);
            } 

            if (!dices[playerTurn].hasPlayed && dices[playerTurn].isTurn) return;

            dices[playerTurn].hasPlayed = false;
            dices[playerTurn].isTurn = false;
            dices[playerTurn].makeEffect(dices);
            playerTurn++;
            if (playerTurn >= dices.Count)
            {
                playerTurn = 0;
            }
            view.PlaceDices(dices);
        } else {
            // Si c'est le tour d'un joueur
            
            dices[playerTurn].isTurn = true;
            dices[playerTurn].turn(actions);

            if (dices[playerTurn].hasPlayed){
                dices[playerTurn].makeEffect(dices);
                dices[playerTurn].isTurn = false;
                dices[playerTurn].hasPlayed = false;
                playerTurn++;
                if (playerTurn >= dices.Count)
                {
                    playerTurn = 0;
                }
                view.PlaceDices(dices);
            }
        }
    }



    public Dice getDicePlaying(){
        return dices[playerTurn];
    }


    public void CheckEnd(){
        // Vérifier si un joueur a gagné
        int nbPlayerAlive = 0;
        int nbIAAlive = 0;
        foreach (Dice d in dices)
        {
            if (d.isAI)
            {
                nbIAAlive++;
            } else {
                nbPlayerAlive++;
            }
        }

        if (nbPlayerAlive == 0)
        {
            endGame = true;
            endPanel.SetActive(true);
            endText.text = "Les IA ont gagné";
        } else if (nbIAAlive == 0)
        {
            endGame = true;
            endPanel.SetActive(true);
            endText.text = "Vous avez gagné !!";
        }
    }

    public void restart(){
        // reload the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

}
