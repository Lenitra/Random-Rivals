using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    [SerializeField] private GameManager gameManager; // Référence au game manager

    // Quand le joueur charge une partie (initialisation)
    void Start()
    {
        // Générer la map
        gameManager.initGame();
    }



    void Update()
    {

        // On click on the mouse send a raycast and debug the hit
        if (Input.GetMouseButtonDown(0))
        {
            // Convertir la position de la souris en point dans le monde 2D
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            // Si le raycast touche quelque chose
            if (hit)
            {
                if (hit.transform.gameObject.tag == "Tile"){
                    int x, y;
                    string[] tmp = hit.transform.gameObject.name.Split('_');
                    x = int.Parse(tmp[1]);
                    y = int.Parse(tmp[2]);
                    Debug.Log("Tile : " + x + ";" + y);
                    gameManager.getDicePlaying().lisenInput(x, y);
                }

                
            } 
            else 
            {
                Debug.Log("No hit");
            }
        }





    }
}
