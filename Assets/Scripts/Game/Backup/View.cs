using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    private float tileSize = 1; // Taille d'une tile en unité Unity

    // afficher la map
    public void PlaceTiles(int[,] map){
        // Parcourir la map
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++){
                // Récupérer l'id de la tile
                int id = map[x,y];
                if (id == 0) continue;
                // Récupérer une tile aléatoire
                GameObject tile = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                // Placer la tile
                tile = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                // Changer le nom de la tile
                tile.name = "Tile_" + x + "_" + y;
                // Parenter la tile
                tile.transform.parent = transform;
            }
        }
    }


    public void PlaceDices(List<Dice> dices){
        // Parcourir la liste des dés
        foreach (Dice dice in dices)
        {
            // Récupérer la position du dé
            int x = dice.posX;
            int y = dice.posY;
            // Récupérer le dé, c'est le parent du Component Dice
            Transform diceObject = dice.transform;
            // Placer le dé
            diceObject.transform.position = new Vector3(x * tileSize -0.025f, y * tileSize +0.025f, -1);
            
        }
    }

}
