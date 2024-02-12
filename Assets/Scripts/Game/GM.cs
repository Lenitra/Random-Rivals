using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{
    [SerializeField] private GameObject dicePrefab; // Prefab du dé
    [SerializeField] private List<Dice> dices = new List<Dice>(); // Liste des dés
    [SerializeField] private List<Dice> playingQueue = new List<Dice>(); // Liste des dés

    [SerializeField] private GameObject[] tilePrefabs; // Liste des tiles
    [SerializeField] private GameObject mapContainer;  // Parent des tiles

    [SerializeField] private GameObject highlightPrefab;    // Prefab de surbrillance
    [SerializeField] private GameObject highlightContainer; // Parent des surbrillances

    [SerializeField] private GameObject endPanel; // Panel de fin de partie 
    [SerializeField] private Text endText;        // Texte de fin de partie dans le endPanel


    private int[,] map;
    private bool endGame = false;


    private void Start() {
        GenerateMap();
        SpawnPlayerDices();
        SpawnIADices();
    }


    // Génère une carte de jeu
    // 0 = vide
    // 1 = tile
    // 2 = tile + spawn joueur
    // 3 = tile + spawn IA
    private void GenerateMap(){
        map = new int[,]
        {
            {0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
            {1, 1, 0, 1, 1, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 1, 1, 0, 0, 0, 0},
            {1, 1, 0, 0, 0, 1, 0, 0, 0, 0},
            {1, 1, 0, 0, 1, 1, 0, 0, 0, 0},
            {0, 1, 0, 0, 1, 1, 0, 0, 0, 0},
            {0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
            {0, 2, 1, 1, 0, 3, 0, 0, 0, 0},
            {0, 2, 0, 0, 0, 3, 0, 0, 0, 0},
            {2, 0, 0, 0, 0, 3, 0, 0, 0, 0}
        };        
        
        // Placer les tiles
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++){
                if (map[x,y] == 0) continue; // Si la case est vide
                GameObject tile = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                tile = Instantiate(tile, new Vector3(x, y , 0), Quaternion.identity);
                tile.name = "Tile_" + x + "_" + y;
                tile.transform.parent = mapContainer.transform;
            }
        }
    }



    // spawn IA
    private void SpawnIADices(){
        while (true){
            int[] spawnPoint = GetSpawnPoint(true);
            if (spawnPoint[0] == -1) break;
            if (spawnPoint[1] == -1) break;
            GameObject dice = Instantiate(dicePrefab, new Vector3(spawnPoint[0], spawnPoint[1], -1), Quaternion.identity);
            dice.name = "IA_" + dices.Count;
            dice.GetComponent<Dice>().isAI = true;
            // random float between 1 and 3
            dice.GetComponent<Dice>().cooldown = Random.Range(1f, 3f);
            dice.GetComponent<Dice>().posX = spawnPoint[0];
            dice.GetComponent<Dice>().posY = spawnPoint[1];
            dices.Add(dice.GetComponent<Dice>());
        }
    }

    // spawn players
    private void SpawnPlayerDices(){
        GameData data = SaveSystem.LoadGameData();
        int tmpNbPlayerDices = data.dices.Count;
        while (tmpNbPlayerDices != 0){
            int[] spawnPoint = GetSpawnPoint();
            if (spawnPoint[0] == -1) break;
            if (spawnPoint[1] == -1) break;
            GameObject dice = Instantiate(dicePrefab, new Vector3(spawnPoint[0], spawnPoint[1], -1), Quaternion.identity);
            dice.name = "Player_" + (data.dices.Count-tmpNbPlayerDices).ToString();
            dice.GetComponent<Dice>().isAI = false;
            dice.GetComponent<Dice>().posX = spawnPoint[0];
            dice.GetComponent<Dice>().posY = spawnPoint[1];
            dice.GetComponent<Dice>().skills = data.dices[data.dices.Count-tmpNbPlayerDices].items;
            dices.Add(dice.GetComponent<Dice>());
            tmpNbPlayerDices--;
        }
    }


    private int[] GetSpawnPoint(bool isAI = false){
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);
        int value = isAI ? 3 : 2;
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                if (isFree(x, y, value))
                {
                    return new int[] {x, y};
                }
            }
        }
        return new int[] {-1, -1};
    }


    // Vérifier si une case est vide et non occupée
    private bool isFree(int x, int y, int value = -1){
        // Vérifier si la case est dans la map
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            return false;
        }

        // Vérifier est égale à la valeur demandée (si demandée) sinon vérifier si la case pas égale a 0
        if (value != -1){
            if (map[x, y] != value)
            {
                return false;
            }
        } else if (map[x, y] == 0) {
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
        // Fait jouer les IA et définit la liste des dés en attente de jouer
        List<Dice> tempDices = new List<Dice>(dices); // Créez une copie temporaire de la liste dices
        List<Dice> tempPlayingQueue = new List<Dice>(playingQueue); // Créez une copie temporaire de la liste playingQueue
        
        foreach (Dice d in tempDices)
        {
            if (d.isDead) {
                // remove d from the playing queue
                playingQueue.Remove(d);
                dices.Remove(d);
                d.anim.SetBool("Selected", false);
                continue;
            }

            if (d.state == "loading") {
                playingQueue.Remove(d);
                continue;
            }

            if (d.state == "waiting"){

                if (!d.isAI){
                    // if d is not in the playing queue
                    if (!playingQueue.Contains(d)){
                        playingQueue.Add(d);
                    }
                }

                if (d.isAI){
                    d.playIA(dices, getActions(d));
                }
            }

            // Si il y a une erreur sur l'annimation, ça veut dire que le dé a été détruit 
            try{
                d.anim.SetBool("Selected", false);
            } catch {
                dices.Remove(d);
                continue;
            }
        }



        // si au moins un dé est en attente du joueur
        if (playingQueue.Count > 0){
            // Si le premier dé de la liste est un joueur
            Dice d = playingQueue[0];
            d.anim.SetBool("Selected", true);
            List<string> actions = getActions(d);
            highlightTiles(actions);

            if (Input.GetMouseButtonDown(0)){
                // raycast 
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null){
                    if (hit.collider.gameObject.tag == "Tile"){
                        string[] tilePos = hit.collider.gameObject.name.Split('_');
                        int x = int.Parse(tilePos[1]);
                        int y = int.Parse(tilePos[2]);
                        if (actions.Contains(x + ";" + y)){
                            d.moveTo(x, y, dices);
                            playingQueue.RemoveAt(0);
                            clearHighlights();
                            d.anim.SetBool("Selected", false);
                        }
                        
                    }

                    // Si on clique sur le dé qui doit jouer, ça skip son tour
                    if (hit.collider.gameObject.tag == "Dice"){
                        if (d == hit.collider.gameObject.GetComponent<Dice>()){
                            d.moveTo(d.posX, d.posY, dices);
                            playingQueue.RemoveAt(0);
                            clearHighlights();
                            d.anim.SetBool("Selected", false);
                        }
                    }
                }
            }   
        }
    }

    // Détruit tous les highlights
    private void clearHighlights(){
        foreach (Transform child in highlightContainer.transform){
            Destroy(child.gameObject);
        }
    }

    // Place les surbrillances sur une liste de cases
    private void highlightTiles(List<string> actions){
        // Supprimer les anciennes surbrillances
        foreach (Transform child in highlightContainer.transform){
            Destroy(child.gameObject);
        }
        foreach (string action in actions){
            string[] pos = action.Split(';');
            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);
            GameObject highlight = Instantiate(highlightPrefab, new Vector3(x, y, -2), Quaternion.identity);
            highlight.name = "Highlight_" + x + "_" + y;
            highlight.transform.parent = highlightContainer.transform;
        }
    }


    private List<string> getActions(Dice dice){
        int x = dice.posX;
        int y = dice.posY;
        List<string> actions = new List<string>();
        if (isFree(x, y+1)) actions.Add(x + ";" + (y+1));
        if (isFree(x+1, y+1)) actions.Add((x+1) + ";" + (y+1));
        if (isFree(x+1, y)) actions.Add((x+1) + ";" + y);
        if (isFree(x+1, y-1)) actions.Add((x+1) + ";" + (y-1));
        if (isFree(x, y-1)) actions.Add(x + ";" + (y-1));
        if (isFree(x-1, y-1)) actions.Add((x-1) + ";" + (y-1));
        if (isFree(x-1, y)) actions.Add((x-1) + ";" + y);
        if (isFree(x-1, y+1)) actions.Add((x-1) + ";" + (y+1));
        return actions;        
    }



}
