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

        





    }
}
