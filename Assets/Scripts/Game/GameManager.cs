using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // [SerializeField] private View view; // Référence à la vue
    [SerializeField] private GameObject dicePrefab; // Liste des dés
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject mapContainer;
    [SerializeField] private GameObject highlightTilesContainer;
    [SerializeField] private GameObject tileSurbrillance;
    [SerializeField] private Transform camera;
    [SerializeField] private Text endText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject detailPanel;

    
    public int nbIA = 3; // Nombre d'IA
    public int nbPlayer = 2; // Nombre dés joueurs

    // Liste des joueurs
    private List<Dice> dices = new List<Dice>();
    private int playerTurn = 0;
    private int[,] map = new int[10,10];
    private bool endGame = false;
    

    void Start()
    {
        initGame();
    }

    // Quand le joueur charge une partie (initialisation)
    public void initGame(){
        // Générer la map
        GenerateMap();
        // Générer les dés
        GenerateDice();
        camera.position = new Vector3(map.GetLength(0)/2, map.GetLength(1)/2, -10);
    }


    private void GenerateDice() {
        // Génère les joueurs
        GenerateDiceForEntities(nbPlayer, false);

        // Génère les IA
        GenerateDiceForEntities(nbIA, true);
    }

    private void GenerateDiceForEntities(int nbEntities, bool isAI) {
        for (int i = 0; i < nbEntities; i++) {
            // Créer un dé
            GameObject dice = Instantiate(dicePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // Changer le nom du dé en fonction du type (Joueur ou IA)
            dice.name = isAI ? $"AI_{i}" : $"P_{i}";

            // Trouver une case vide non occupée
            (int x, int y) = FindEmptyPosition();

            // Configurer les attributs de la classe Dice
            var diceComponent = dice.GetComponent<Dice>();
            diceComponent.posX = x;
            diceComponent.posY = y;       
            diceComponent.isAI = isAI;   
            diceComponent.place(x, y); 

            // Ajouter le dé à la liste des dés
            dices.Add(diceComponent);
        }
    }


    // Trouver une position aléatoire vide non occupée
    private (int, int) FindEmptyPosition() {
        int x, y;
        bool isOccupied;
        do {
            x = Random.Range(0, map.GetLength(0));
            y = Random.Range(0, map.GetLength(1));
            isOccupied = !isFree(x, y);
        } while (isOccupied);

        return (x, y);
    }


    // Génère une map et l'affiche
    private void GenerateMap(){

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



    // Vérifier si une case est vide et non occupée
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

        // on left click
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPosition = Input.mousePosition; // Position de la souris en coordonnées de l'écran
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition); // Conversion en coordonnées du monde
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit)
            {
                if (hit.transform.gameObject.tag == "Dice")
                {
                    detailPanel.SetActive(true);
                    Dice dice = hit.transform.gameObject.GetComponent<Dice>();
                    detailPanel.transform.GetChild(0).GetComponent<Text>().text = dice.getDetails();

                    // Conversion pour UI en utilisant Screen Space - Camera ou World Space
                    RectTransform canvasRectTransform = detailPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                    Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(screenPosition); // Conversion en position de viewport
                    Vector2 uiPosition = new Vector2(
                        ((viewportPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
                        ((viewportPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f))
                    );
                    detailPanel.transform.localPosition = uiPosition; // Appliquer la position transformée
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            detailPanel.SetActive(false);
        }



        CheckEnd();
        if (endGame) return;

        // Vérifier si on donne le tour à un dé qui a été tué
        if (getDicePlaying() == null) {
            // remove the element from the list
            dices.RemoveAt(playerTurn);
            if (playerTurn >= dices.Count)
            {
                playerTurn = 0;
            }
        }
    
        

        switch (getDicePlaying().state)
        {
            case "":
                getDicePlaying().eventIsTurn();
                highlightTiles(getActions(getDicePlaying()));
                break;

            case "wait":
                // Si c'est un dé IA, on applique la méthode iaTurn (calcul de l'action à faire) 
                if (getDicePlaying().isAI) {
                    getDicePlaying().iaTurn(getActions(getDicePlaying()), dices);
                }
                
                // Si c'est un dé du joueur, on attend un clic
                else {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
                        if (hit)
                        {
                            if (hit.transform.gameObject.tag == "Tile"){
                                int clic_x, clic_y;
                                string[] tmp = hit.transform.gameObject.name.Split('_');
                                clic_x = int.Parse(tmp[1]);
                                clic_y = int.Parse(tmp[2]);
                                // if the tile is in the list of actions
                                if (getActions(getDicePlaying()).Contains(clic_x + ";" + clic_y))
                                {
                                    StartCoroutine(getDicePlaying().moving(clic_x, clic_y));
                                }
                            }
                        }
                    }    
                }

            break;

            case "moving":
                removeHighlightTiles();
            break;

            case "effect":
                getDicePlaying().makeEffect(dices);
                break;

            case "end":
                getDicePlaying().state = "";
                playerTurn++;
                if (playerTurn >= dices.Count)
                {
                    playerTurn = 0;
                }
                break;

        }
    }
        







    // Retourne les cases où le dé peut se déplacer
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

    // Affiche les cases où le dé peut se déplacer 
    private void highlightTiles(List<string> actions){
        foreach (string action in actions)
        {
            string[] tmp = action.Split(';');
            int x = int.Parse(tmp[0]);
            int y = int.Parse(tmp[1]);
            GameObject tile = Instantiate(tileSurbrillance, new Vector3(x, y , 0), Quaternion.identity);
            tile.name = "Tile_" + x + "_" + y;
            tile.transform.parent = highlightTilesContainer.transform;
        }
    }

    // Supprime la surbrillance des cases
    private void removeHighlightTiles(){
        foreach (Transform child in highlightTilesContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }





    public Dice getDicePlaying(){
        return dices[playerTurn];
    }

    // Check si c'est la fin de la partie et affiche le panel de fin
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
